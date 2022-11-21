using Hazelcast;
using Hazelcast.Core;
using HazelcastDemo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HazelcastDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        //private readonly HazelcastFactory _factory;
        private const string MapReplicatedIdentity = "longpv2_test_map_replicated_order";
        private const string MapIdentity = "longpv2_test_map_order";
        private const string Map01 = "longpv2_058C000001";
        private const string Map02 = "longpv2_058C000002";
        private const string MapNearCache = "longpv_map_nearcache";
        private readonly IHazelcastClient _client;

        public OrderController(IHazelcastClient client)
        {
            _client = client;
        }

        [HttpGet("WriteRecord")]
        public async Task<IActionResult> WriteDataAcc(int record)
        {
            var map = await _client.GetMapAsync<string, Order>(MapNearCache);
            Random random = new Random();
            IDictionary<string, Order> orders = new Dictionary<string, Order>();

            for (int i = 0; i < record; i++)
            {
                var order = new Order
                {
                    ClientCode = "058C" + random.Next(0,999999999).ToString("D6"),
                    OrderId = i * 10 / 2,
                    Quantity = i * 1000 / 4,
                    ActionType = "B",
                    Price = i * 568790 / 4,
                    Symbol = "FTS" + i,
                    DateTime = DateTime.Now.ToString()
                };
                orders.Add(order.ClientCode + "_" + order.OrderId  , order);
            }
            _ = map.SetAllAsync(orders);

            return Ok();
        }

        

        [HttpGet("SQLHazelGetAll")]
        public async Task<IActionResult> SQLHazelCastGetAll()
        {
         
                Stopwatch sw = new();
                sw.Start();
                var sqlResult = await _client.Sql.ExecuteQueryAsync($"SELECT * FROM {MapNearCache}");
                sw.Stop();
                int sum = 0;

                await foreach (var item in sqlResult)
                {
                    sum++;
                }

                Console.WriteLine(" log "  + sw.ElapsedMilliseconds + " SUM " + sum );
                
            
            return Ok();
        }

        [HttpGet("OrderList")]
        public async Task<IActionResult> OrderList()
        {
          
            var map = await _client.GetListAsync<Order>("ListOrderLongPv2");
            var orderRecord = OrderFactory.GenerateListOrder(100);
            Order order = new Order();

            order.ClientCode = "058C092001";
            order.Quantity = 10000;
            order.DateTime = (DateTime.Now).ToString();
            order.Price = 4556;
            order.OrderId = 1;
            order.Status = 0;
            order.ActionType = "B";


            _ = map.AddAsync(10, order);

            var listOrder = map.GetAllAsync().Result.ToList<Order>;
            var a = 100;
            //var a = JsonSerializer.Deserialize<Order>(listOrder);
                //.Where(x=> x.ClientCode == "058C092001").FirstOrDefault();
            return Ok(a);

        }


        [HttpGet("CreateMapping")]
        public async Task<IActionResult> CreateMapping()
        {
            await _client.Sql.ExecuteCommandAsync($"CREATE OR REPLACE " +
               $"MAPPING {MapNearCache}(" +
                   $"__key varchar, " +
                   $"clientCode varchar," +
                   $"orderId int, " +
                   $"actionType varchar, " +
                   $"symbol varchar, " +
                   $"quantity int, " +
                   $"DateTime varchar, " +
                   $"price int) " +
               $"TYPE IMap OPTIONS (" +
                   $"'keyFormat'='varchar', " +
                   $"'valueFormat' = 'portable', " +
                   $"'valuePortableFactoryId' = '1',    " +
                   $"'valuePortableClassId' = '1'" +
               $")");

            var map = await _client.GetMapAsync<string, Order>(MapNearCache);

            return Ok();
        }
    }
}
