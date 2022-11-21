using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Diagnostics;
using System.Text.Json;

namespace HazelcastDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RedisController : ControllerBase
    {
        private readonly IDatabase _database;
        private readonly IConnectionMultiplexer _redis;
        private readonly int _databaseNumber;
        private string _tempValue;
        private string _tempKey;

        public RedisController(IConnectionMultiplexer redis)
        {
            _redis = redis;
            _databaseNumber = 9;
            _database = _redis.GetDatabase(_databaseNumber);
        }

        [HttpGet("InsertRedis")]
        public IActionResult SetRecordRedis(int number)
        {
            Random random = new Random();
            Stopwatch sw = new();
            try
            {
                var randomRecord = CashLogFactory.GenerateCollection(1);
                var valueRedis = JsonSerializer.Serialize(randomRecord);
                sw.Start();
                for (int i = 0; i < number; i++)
                {
                    var cacheKey = "keyRedisDatNT" + random.Next(1, number * 9563) + i;
                    _database.StringSet(cacheKey, valueRedis);
                    if (i == number - 1)
                    {
                        _tempValue = valueRedis;
                        _tempKey = cacheKey;
                        Console.WriteLine($"Value: {_tempValue} ------ Key: {_tempKey} \n");
                    }
                }
                sw.Stop();
                Console.WriteLine(CheckLastKey().ToString());
            }
            catch (Exception)
            {
                throw;
            }
            return Ok("excecute " + number + " estimate " + sw.ElapsedMilliseconds + " milliseconds");
        }

        private bool CheckLastKey()
        {
            var tempValue = _database.StringGetAsync(_tempKey).Result.ToString();
            Console.WriteLine(tempValue);
            if (tempValue == _tempValue)
                return true;
            else return false;
        }

        [HttpGet("UpdateRedis")]
        public IActionResult UpdateRecordRedis(int number)
        {
            Stopwatch stopwatch = new Stopwatch();
            Random random = new Random();
            var keys = GetServer(0).Keys(database: _databaseNumber);
            var keysArr = keys.Select(key => (RedisKey)key).Take(number).ToList();
            var valueRedis = JsonSerializer.Serialize(CashLogFactory.GenerateCashLog());
            int i = 0;
            stopwatch.Start();
            foreach (RedisKey key in keysArr)
            {
                i++;
                _database.StringSet(key, valueRedis);
                if (i == keysArr.Count)
                {
                    _tempKey = key;
                    _tempValue = valueRedis;
                }
            }

            stopwatch.Stop();
            Console.WriteLine(CheckLastKey().ToString());
            return Ok($"execute {number} record in {stopwatch.ElapsedMilliseconds} milliseconds");
        }

        [HttpGet("DeleteRedis")]
        public IActionResult DeleteRecordRedis(int number)
        {
            Stopwatch stopwatch = new Stopwatch();

            var keys = GetServer(0).Keys(database: _databaseNumber);
            var keysBefor = keys.Select(key => (RedisKey)key).Count();
            var keysArr = keys.Select(key => (RedisKey)key).Take(number).ToList();

            stopwatch.Start();
            foreach (RedisKey key in keysArr)
            {
                _database.KeyDelete((RedisKey)key);
            }

            stopwatch.Stop();
            var keyAfter = keys.Select(key => (RedisKey)key).Count();

            return Ok($"execute {number} record in {stopwatch.ElapsedMilliseconds} milliseconds ---- Amount of Keys was deleted: {keysBefor - keyAfter}");
        }


        [HttpGet("ReadRedis")]
        public IActionResult GetAllKey(int number)
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                var keys = GetServer(0).Keys(database: _databaseNumber);
                var keysArr = keys.Select(key => (RedisKey)key).Take(number).ToList();
                stopwatch.Start();

                foreach (var key in keysArr)
                {
                    var redisValue = _database.StringGet(key);
                }
                stopwatch.Stop();

                return Ok($"execute {number} record in {stopwatch.ElapsedMilliseconds} milliseconds or {stopwatch.Elapsed} seconds");
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// index = 0: 121, index = 1:122, index = 2:123
        /// </summary>
        private IServer GetServer(int index)
        {
            try
            {
                var endpoint = _redis.GetEndPoints();
                return _redis.GetServer(endpoint[index]);
                //end point = 172.16.0.122:6379
            }
            catch
            {
                return null;
            }
        }
    }
}

