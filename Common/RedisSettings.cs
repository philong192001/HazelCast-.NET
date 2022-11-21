using StackExchange.Redis;

namespace HazelcastDemo.Common
{
    public class RedisSettings
    {
        public  Dictionary<string, string> EndPoints { get; set; }
        public string ServiceName { get; set; }
        public bool AllowAdmin { get; set; }
        public int ConnectRetry { get; set; }
        public int SyncTimeout { get; set; }
        public int ConnectTimeout { get; set; }
        public bool AbortOnConnectFail { get; set; }
        public int DatabaseNumber { get; set; }

        public ConfigurationOptions MapToConfigurationOptions()
        {
            return new ConfigurationOptions()
            {
                ServiceName = ServiceName,
                AllowAdmin = AllowAdmin,
                ConnectRetry = ConnectRetry,
                SyncTimeout = SyncTimeout,
                ConnectTimeout = ConnectTimeout,
                AbortOnConnectFail = AbortOnConnectFail,
                EndPoints = { EndPoints["EndPoint_01"]}
                   
            };
        }
    }
}
