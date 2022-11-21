using StackExchange.Redis;

namespace HazelcastDemo.Extensions
{
    public class ConfigurationOptionsSetting
    {
        public static string ServiceName { get; set; }
        public static bool AllowAdmin { get; set; }
        public static Dictionary<string, string> EndPoints { get; set; }
        public static int ConnectRetry { get; set; }
        public static int SyncTimeout { get; set; }
        public static int ConnectTimeout { get; set; }
        public static bool AbortOnConnectFail { get; set; }
        public static int DatabaseNumber { get; set; }

        public static ConfigurationOptions MapToConfigurationOptions()
        {
            return new ConfigurationOptions()
            {
                ServiceName = ServiceName,
                AllowAdmin = AllowAdmin,
                EndPoints =
                    {
                        EndPoints["EndPoint_01"],
                        EndPoints["EndPoint_02"],
                        EndPoints["EndPoint_03"],
                    },
                ConnectRetry = ConnectRetry,
                SyncTimeout = SyncTimeout,
                ConnectTimeout = ConnectTimeout,
                AbortOnConnectFail = AbortOnConnectFail
            };
        }
    }
}
