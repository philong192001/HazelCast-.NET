using Hazelcast;
using HazelcastDemo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Text.Json;

namespace HazelcastDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly HazelcastFactory _factory;
        private const string MapReplicatedIdentity = "longpv2_test_map_replicated_1";
        private const string MapIdentity = "longpv2_test_map_1";

        public HomeController(HazelcastFactory factory)
        {
            _factory = factory;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return Ok(SecuritiesFactory.GenerateCollection(20));
        }

        [HttpGet("Set")]
        public async Task<IActionResult> SetAsync()
        {
            var client = await _factory.StartClientAsync();
            var map = await client.GetMapAsync<string, string>(MapIdentity);
            var collection = SecuritiesFactory.GenerateCollection(200000);
            Stopwatch sw = new();
            sw.Start();
            var dic = collection.ToDictionary(x => x.ClientCode, x => JsonSerializer.Serialize(x));
            var demo = JsonSerializer.Serialize(dic.Take(3));
            await map.SetAllAsync(dic);
            sw.Stop();
            Console.WriteLine($"set {collection.Count()} record cost {sw.Elapsed} second");
            return Ok(dic.Count());
        }

        [HttpGet("SetRep")]
        public async Task<IActionResult> SetRepAsync()
        {
            var client = await _factory.StartClientAsync();
            var map = await client.GetReplicatedMapAsync<string, string>(MapReplicatedIdentity);
            var collection = SecuritiesFactory.GenerateCollection(50000);
            Stopwatch sw = new();
            sw.Start();
            var dic = collection.ToDictionary(x => x.ClientCode, x => JsonSerializer.Serialize(x));
            await map.SetAllAsync(dic);
            sw.Stop();
            Console.WriteLine($"set {collection.Count()} record cost {sw.Elapsed} second");
            return Ok(dic.Count());
        }

        [HttpGet("Load")]
        public async Task<IActionResult> LoadAsync()
        {
            var client = await _factory.StartClientAsync();
            var map = await client.GetMapAsync<string, string>(MapIdentity);
            var collection = SecuritiesFactory.GenerateCollection(200000);
            await map.LoadAllAsync(collection.Select(x => x.ClientCode).ToArray(), false);
            return Ok();
        }

        [HttpGet("Get")]
        public async Task<IActionResult> GetMapAsync()
        {
            var client = await _factory.StartClientAsync();
            var map = await client.GetMapAsync<string, string>(MapIdentity);
            var collection = SecuritiesFactory.GenerateCollection(1);
            Stopwatch sw = new();
            sw.Start();
            var keys = collection.Select(x => x.ClientCode).ToArray();
            var data = await map.GetAllAsync(keys);
            sw.Stop();
            Console.WriteLine($"get {data.Count()} record cost {sw.Elapsed} second");
            
            sw.Restart();
            var data2 = await map.GetAllAsync(keys);
            sw.Stop();
            Console.WriteLine($"get {data2.Count()} record cost {sw.Elapsed} second");

            var collection2 = SecuritiesFactory.GenerateCollection(startIndex: 200001, count: 100000);
            var dic = collection2.ToDictionary(x => x.ClientCode, x => JsonSerializer.Serialize(x));
            keys = keys.Concat(dic.Keys).ToArray();
            await map.SetAllAsync(dic);

            Console.WriteLine("Start delay 10s");
            Task.Delay(TimeSpan.FromSeconds(10)).Wait();

            sw.Restart();
            var data3 = await map.GetAllAsync(keys);
            sw.Stop();
            Console.WriteLine($"get {data3.Count()} record cost {sw.Elapsed} second");

            return Ok(data.Count());
        }

        [HttpGet("GetLongpV2")]
        public async Task<IActionResult> GetMapAsyncT(int record)
        {
            var client = await _factory.StartClientAsync();
            var map = await client.GetMapAsync<string, string>(MapIdentity);


            var collection = CashLogFactory.GenerateCollection(record);
            Stopwatch sw = new();
            sw.Start();

            var dic = collection.ToDictionary(x => x.ClientCode, x => JsonSerializer.Serialize(x));
            await map.SetAllAsync(dic);
            sw.Stop();
            Console.WriteLine($"---------- set {record} record cost {sw.Elapsed} second");
            //var keys = collection.Select(x => x.ClientCode).ToArray().Concat(dic.Keys).ToArray();
            //sw.Restart();
            //var a  = await map.GetAsync("058C000000");
            //sw.Stop();
            //Console.WriteLine($"-----------> get 1 record cost {sw.ElapsedMilliseconds} milisecond " + a);

            return Ok();
        }

        [HttpGet("Destroy")]
        public async Task<IActionResult> Destroy()
        {
            var client = await _factory.StartClientAsync();
            var map = await client.GetMapAsync<string, string>(MapIdentity);
            await client.DestroyAsync(map);
            return Ok("done");
        }
    }
}
