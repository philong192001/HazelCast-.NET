using Hazelcast.DistributedObjects;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

namespace HazelcastDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CashLogController : ControllerBase
    {
        private readonly HazelcastFactory _factory;
        private const string MapReplicatedIdentity = "longpv2_test_map_replicated_thanhvd2";
        private const string MapIdentity = "longpv2_test_map_1";

        public CashLogController(HazelcastFactory factory)
        {
            _factory = factory;
        }

        [HttpGet]
        public async Task<IActionResult> SetAsync(int startIndex, int count)
        {
            var client = await _factory.StartClientAsync();
            var map = await client.GetMapAsync<string, string>(MapIdentity);
            var collection = CashLogFactory.GenerateCollection(count, startIndex);
            var dic = collection.ToDictionary(x => x.ClientCode+"_"+x.StockCode, x => JsonSerializer.Serialize(x));
            Stopwatch sw = new();
            sw.Start();
            await map.SetAllAsync(dic);
            sw.Stop();
            Console.WriteLine($"set {collection.Count()} record cost {sw.Elapsed} second");
            return Ok(dic.Count());
        }

        [HttpGet("loop")]
        public async Task<IActionResult> LoopAsync(int index = 0, int interval = 0)
        {
            var client = await _factory.StartClientAsync();
            var map = await client.GetMapAsync<string, string>("INF");
            
            if(interval < 1)
            {
                while (true)
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    index = await SetDataToHazelcastAsync(map, index);
                    sw.Stop();
                    Console.WriteLine(" time execute " + sw.ElapsedMilliseconds + " millisecond");
                }
            }
            else
            {
                for (int i = 0; i < interval; i++)
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    index = await SetDataToHazelcastAsync(map, index);
                    sw.Stop();
                    Console.WriteLine($"time execute {index}" + "----------" +  sw.ElapsedMilliseconds + " millisecond");
                }
            }

            return Ok();
        }

        private async Task<int> SetDataToHazelcastAsync(IHReplicatedMap<string, string> map, int index)
        {
            var collection = CashLogFactory.GenerateCollection(100000, index);
            var dic = collection.ToDictionary(x => x.ClientCode + "_" + x.StockCode, x => JsonSerializer.Serialize(x));
            await map.SetAllAsync(dic);
            Console.WriteLine($"set success {dic.Count()}");
            index += 100000;
            return index;
        }

        private async Task<int> SetDataToHazelcastAsync(IHMap<string, string> map, int index)
        {
            var collection = CashLogFactory.GenerateCollection(500000, index);
            var dic = collection.ToDictionary(x => x.ClientCode + "_" + x.StockCode, x => JsonSerializer.Serialize(x));
            await map.SetAllAsync(dic);
            Console.WriteLine($"set success {dic.Count()}");
            index += 500000;
            return index;
        }

        [HttpGet("Destroy")]
        public async Task<IActionResult> Destroy()
        {
            var client = await _factory.StartClientAsync();
            var map = await client.GetReplicatedMapAsync<string, byte[]>(MapReplicatedIdentity);
            await client.DestroyAsync(map);
            return Ok("done");
        } 
        
        
        [HttpGet("TestCodeThanhvd2")]
        public async Task<IActionResult> PutAsyncThanhvd2()
        {
            var client = await _factory.StartClientAsync();
            var map = await client.GetReplicatedMapAsync<string, byte[]>(MapReplicatedIdentity);



            var semaphore = new SemaphoreSlim(100);
            Stopwatch sw = new();
            sw.Start();
            for (int i = 0; i < 10000; i++)
            {
                semaphore.Wait();
                await Task.Run(() =>
                {
                    map.PutAsync(i.ToString(), new byte[1024]);
                    semaphore.Release();
                });
            }
            sw.Stop();
            return Ok("done " + sw.ElapsedMilliseconds + " entry : " + await map.GetSizeAsync());
        }


    }
}
