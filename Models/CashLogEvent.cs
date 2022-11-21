using StackExchange.Redis;

namespace HazelcastDemo.Models
{
    public class CashLogEvent
    {
        public int HandlerId { get; set; }
        public IEnumerable<CashLog>? CashLogEntity { get; set; }
        public RedisKey key { get; set; }
        public string ActionType { get; set; }
        public int Amount { get; set; }
        public CashLogEvent()
        {

        }
    }
}
