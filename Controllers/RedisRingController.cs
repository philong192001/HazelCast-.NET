using Disruptor;
using Disruptor.Dsl;
using HazelcastDemo.Models;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json;
#nullable disable
namespace HazelcastDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RedisRingController : ControllerBase
    {
        private readonly IDatabase _database;
        private readonly IConnectionMultiplexer _redis;
        private readonly int _databaseNumber;

        public RedisRingController(IConnectionMultiplexer redis)
        {
            _redis = redis;
            _databaseNumber = 2;
            _database = redis.GetDatabase(_databaseNumber);
        }
        [HttpGet("RUD")]
        public IActionResult read(string type,int amount, int thread = 16)
        {
            int l = 1;
            var ringBuffer = BuildCashLogRingBuffer(8192, thread);
            var valueRedis =  CashLogFactory.GenerateCollection(1);
            Stopwatch sw = new Stopwatch();
            CashLogEventHandler.keysBeforDelete = GetKeyRedis(0).Count();// 0 == get all keys
            sw.Start();
            var keys = GetKeyRedis(amount);
            foreach(var key in keys)
            {
                long sequence = ringBuffer.Next();
                var data = ringBuffer[sequence];
                data.key = key;
                data.HandlerId = l;
                data.ActionType = type;
                data.Amount = amount;

                ringBuffer.Publish(sequence);

                l++;
                if (l > thread)
                {
                    l = 1;
                }
            }
            sw.Stop();
            return Ok($"execute ${amount} in {sw.ElapsedMilliseconds} milliseconds");

        }

        [HttpGet("Insert")]
        public IActionResult ExecuteRingRedis(int amount, string actionType, int thread = 8)
        {
            try
            {
                var i = 0;
                Random random = new Random();
                var ringBuffer = BuildCashLogRingBuffer(8192, thread);
                Stopwatch sw = new Stopwatch();
                int l = 1;
                for (int k = 0; k < amount; k++)
                {
                    var cacheKey = "keyRedisDatNT" + Guid.NewGuid();
                    var randomRecord = CashLogFactory.GenerateCollection(1);

                    long sequence = ringBuffer.Next();
                    var data = ringBuffer[sequence];
                    data.key = cacheKey;
                    data.HandlerId = l;
                    data.CashLogEntity = randomRecord;
                    data.ActionType = actionType;
                    data.Amount = amount;


                    if(k == 0)
                    {
                        sw.Start();

                    }

                    ringBuffer.Publish(sequence);

                    l++;
                    if (l > thread)
                    {
                        l = 1;
                    }

                    if(k == amount - 1)
                    {
                        sw.Stop();
                    }
                }

                return Ok($"execute ${amount} in {sw.ElapsedMilliseconds} milliseconds");
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

       

        private RingBuffer<CashLogEvent> BuildCashLogRingBuffer(int bufferSize, int numberOfThread)
        {
            var disruptor = new Disruptor<CashLogEvent>(() => new CashLogEvent(), bufferSize);
            CashLogEventHandler[] arr = new CashLogEventHandler[numberOfThread];

            for (int i = 0; i < numberOfThread; i++)
            {
                arr[i] = new CashLogEventHandler(i + 1, _database, _redis);
            }

            disruptor.HandleEventsWith(arr).Then(new EndHandler(_database, _redis));
            return disruptor.Start();
        }

        private List<string> GetKeyRedis(int amount)
        {
            var listKeys = new List<string>();
            if (amount == 0)
            {
                var keysAll = GetServer(0).Keys(database: _databaseNumber);
                listKeys = keysAll.Select(key => (string)key).ToList();
            }
            else
            {
                var keys = GetServer(0).Keys(database: _databaseNumber);
                listKeys = keys.Select(key => (string)key).Take(amount).ToList();
            }

            return listKeys;
        }

        private IServer GetServer(int index)
        {
            try
            {
                var endpoint = _redis.GetEndPoints();
                return _redis.GetServer(endpoint[index]);
            }
            catch
            {
                return null;
            }
        }
    }

    public class CashLogEventHandler : IEventHandler<CashLogEvent>
    {
        public static double keysBeforDelete;
        public static double keysAfterDelete;
        private readonly int _handlerId;
        private readonly IDatabase _database;
        private readonly IConnectionMultiplexer _redis;
        private readonly int _databaseNumber;
        private int i = 0, k = 0;
        private ConcurrentDictionary<string, string> _tempResult = new ConcurrentDictionary<string, string>();
        private string _tempKey;
        List<string> _keysArr;
        public CashLogEventHandler(int handlerId, IDatabase database, IConnectionMultiplexer redis)
        {
            _handlerId = handlerId;
            _database = database;
            _redis = redis;
            _databaseNumber = 2;
            _keysArr = new List<string>(); 
            
        }
        public void OnEvent(CashLogEvent cashLogEvent, long sequence, bool endOfBatch)
        {
            var type = cashLogEvent.ActionType;
            if (cashLogEvent.HandlerId == _handlerId)
            {
                i++;
                var valueRedis = JsonSerializer.Serialize(cashLogEvent.CashLogEntity);
               

                switch (cashLogEvent.ActionType)
                {

                    case "c":
                        _database.StringSet(cashLogEvent.key, valueRedis);
                        break;

                    case "r":
                         _database.StringGet(cashLogEvent.key);
                        
                        break;

                    case "u":
                        _database.StringSet(cashLogEvent.key, valueRedis);
                        break;

                    case "d":
                        _database.KeyDelete(cashLogEvent.key);

                        break;

                    default:
                        Console.WriteLine("null actionType");
                        break;

                }
                //chỉ áp dụng cho case 10000 record :))
                if (i == 625 && _handlerId == 16)// 10000 record đẩy qua 8 theard, mỗi thread sẽ nhận được 1250 record
                {
                    keysAfterDelete = GetKeyRedis(0).Count();
                    _tempResult.TryAdd(cashLogEvent.key, valueRedis);
                    _tempKey = cashLogEvent.key;
                    if(type == "c" || type == "u")
                    {
                        CheckLastKey();
                    }
                    if(type == "d")
                        CheckRecordWasDeleted();
                }
            }
        }

        public void CheckLastKey()
        {
            var valueRedis = _database.StringGet(_tempKey);
            string valueLastRecord;

            _tempResult.TryGetValue(_tempKey, out valueLastRecord);

            if (valueRedis == valueLastRecord)
                Console.WriteLine($"Last record inserted \n key is: {_tempKey}");
            else Console.WriteLine("Last record did not inserted\n");
        }

        public void CheckRecordWasDeleted()
        {
            Console.WriteLine($"Records were deleted: {keysBeforDelete - keysAfterDelete}");
        }

        private List<string> GetKeyRedis(int amount)
        {
            var listKeys = new List<string>();
            if (amount == 0)
            {
                var keysAll = GetServer(0).Keys(database: _databaseNumber);
                listKeys = keysAll.Select(key => (string)key).ToList();
            }
            else
            {
                var keys = GetServer(0).Keys(database: _databaseNumber);
                listKeys = keys.Select(key => (string)key).Take(amount).ToList();
            }

            return listKeys;
        }

        private IServer GetServer(int index)
        {
            try
            {
                var endpoint = _redis.GetEndPoints();
                return _redis.GetServer(endpoint[index]);
            }
            catch
            {
                return null;
            }
        }

    }

    public class EndHandler : IEventHandler<CashLogEvent>
    {
        private readonly IDatabase _database;
        private readonly IConnectionMultiplexer _redis;

        private int i = 1;
        public EndHandler(IDatabase database, IConnectionMultiplexer redis)
        {
            _redis = redis;
            _database = database;
        }
        public void OnEvent(CashLogEvent cashLogEvent, long sequence, bool endOfBatch)
        {
          
        }
    }
}
