using Hazelcast;
using Hazelcast.Transactions;
using Microsoft.AspNetCore.Mvc;


namespace HazelcastDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionHazalCastController : ControllerBase
    {
        private readonly IHazelcastClient _client;
        private readonly OrderingHazelcastContext _context;

        public TransactionHazalCastController(IHazelcastClient client, OrderingHazelcastContext context)
        {
            _client = client;
            _context = context;
        }

        [HttpGet("Trans")]
        public async Task<IActionResult> TTransaction()
        {
            var trans = await _context.BeginTransactionAsync();


            try
            {
                var transactionMap = await trans.GetMapAsync<string, string>("txn-map");
                var clientNew = await HazelcastClientFactory.StartNewClientAsync(); 
                await transactionMap.PutIfAbsentAsync("key", "value");
                await _context.CommitTransactionAsync();
            }
            catch
            {
                throw;
                
            }

            return Ok();
        }
    }
}
