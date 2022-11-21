using HazelcastDemo.Common;
using HazelcastDemo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text.Json;

namespace HazelcastDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OracleController : ControllerBase
    {
        private readonly DbContextOptions<HazelCastContext> dbContextOptions;
        private const string ClientCodePrefix = "058C";
        private readonly HazelCastContext _context;
        private const string ConnectionString = "USER ID=hazelcast01;Password=hazelcast01;DATA SOURCE=10.26.7.214:1521/hazelcast01";
        public OracleController(HazelCastContext context)
        {
            _context = context;
            dbContextOptions = new DbContextOptionsBuilder<HazelCastContext>().UseOracle(ConnectionString).Options;
        }

        [HttpGet("SetRecord")]
        public IActionResult SetRecord(int index,int thread)
        {
            var randomRecord = JsonSerializer.Serialize(CashLogFactory.GenerateCollection(1));
            Stopwatch sw = new Stopwatch();
            var rand = new Random();
            List<HazelcastModel> hazelcasts = new List<HazelcastModel>();

            for (int i = 0; i < index; i++)
            {
                HazelcastModel hazelcast = new HazelcastModel()
                {
                    HKey = ClientCodePrefix + rand.NextInt64().ToString("D6"),
                    HValue = randomRecord
                };

                hazelcasts.Add(hazelcast);
                //_context.Add(hazelcast);
                //_context.SaveChanges();
            }

            sw.Start();
            Parallel.ForEach(hazelcasts, new ParallelOptions() { MaxDegreeOfParallelism = thread }, item =>
            {
                var context = new HazelCastContext(dbContextOptions);
                context.Add(item);
                context.SaveChanges();
            });

            sw.Stop();

            return Ok(sw.ElapsedMilliseconds + " millisencond ----" + " --- record : " + index);
        }

        [HttpGet("UpdateRecord")]
        public IActionResult UpdateRecord(int number)
        {
            Stopwatch sw = new Stopwatch();
            var record = _context.hazelcasts.Take(number).ToList();
            var randomRecord = JsonSerializer.Serialize(CashLogFactory.GenerateCollection(1));

            sw.Start();
            foreach (var item in record)
            {
                item.HValue = randomRecord;

                _context.Update(item);
                _context.SaveChanges();

            }
            sw.Stop();

            return Ok(sw.ElapsedMilliseconds + " millisencond ----" + " --- record : " + number);
        }

        [HttpGet("DeleteRecord")]
        public IActionResult DeleteRecord(int number)
        {
            Stopwatch sw = new Stopwatch();
            var record = _context.hazelcasts.Take(number).ToList();
            sw.Start();
            foreach (var item in record)
            {
                _context.Remove(item);
                _context.SaveChanges();
            }
            sw.Stop();

            return Ok(sw.ElapsedMilliseconds + " millisencond ----" + " --- record : " + number);
        }

        [HttpGet("GetRecord")]
        public IActionResult GetRecord(int number)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var record = _context.hazelcasts.Take(number).ToList();
            sw.Stop();
            return Ok(record.Count() + " record  ------ time execute : " + sw.ElapsedMilliseconds + "millisecond");
        }

    }
}
