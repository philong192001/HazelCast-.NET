using Hazelcast;
using Hazelcast.Core;
using Hazelcast.NearCaching;
using HazelcastDemo.Models;
using Microsoft.Extensions.Options;

namespace HazelcastDemo
{
    public class SecuritiesFactory
    {
        private const string ClientCodePrefix = "058C";
        private const string StockCodePrefix = "STOCK";

        public static IEnumerable<Securities> GenerateCollection(int count, int startIndex = 0)
        {
            var collection = Enumerable
                .Range(startIndex, count)
                .Select(x => new Securities(
                    clientCode: ClientCodePrefix+x.ToString("D6"),
                    stockCode: StockCodePrefix+x.ToString("D3"),
                    tradingReadyAvailableTotal: x,
                    matchedBuyIntraday: x,
                    receivableSecuritiesT1: x,
                    receivableSecuritiesT2: x,
                    waitingReceiveRightSecurities: x,
                    mortgageAtBank: x,
                    transferRestricted: x,
                    totalAmount: x,
                    marketPrice: x,
                    marketValue: x,
                    averagePrice: x,
                    rootValue: x,
                    dateTime: DateTime.Now
                    ));

            return collection;
        }
    }

    public class OrderFactory
    { 
        private const string ClientCodePrefix = "058C000002";

        public static IEnumerable<Order> GenerateCollection(int count, int startIndex = 0)
        {
            Random random = new Random();   

            var collection = Enumerable
                .Range(startIndex, count)
                .Select(x => new Order(
                    clientCode : ClientCodePrefix,
                    symbol:  x.ToString("D3"),
                    orderId: x,
                    actionType: "B",
                    price: x,
                    quantity: x,
                    status: random.Next(0, 1),
                    dateTime: DateTime.Now.ToString()
                    ));

            return collection;
        }

        public static List<Order> GenerateListOrder(int count)
        {
            List<Order> orders = new();

            for (int x = 0; x < count; x++)
            {
                Order order = new();

                order.ClientCode = "";
                order.Symbol = x.ToString("D3");
                order.OrderId = x;
                order.ActionType = "B";
                order.Price = x;
                order.Quantity= x;
                order.DateTime = DateTime.Now.ToString();

                orders.Add(order);
            }

            return orders;
        }

    }

    public class CashLogFactory
    {
        private const string ClientCodePrefix = "058C";
        private const string StockCodePrefix = "STOCK";
        public static IEnumerable<CashLog> GenerateCollection(int count, int startIndex = 0)
        {
            var now = DateTime.Now;
            var collection = Enumerable
                .Range(startIndex, count)
                .Select(x => new CashLog()
                {
                    UpdateTime = now,
                    CreateDate = now,
                    TranDate = now,    
                    RefId = x+"_ref_id",
                    SubId = x+"_sub_id",
                    Status = Guid.NewGuid(),
                    UpdateType = Guid.NewGuid(),
                    ClientCode = ClientCodePrefix + x.ToString("D6"),
                    StockCode = StockCodePrefix + x.ToString("D3"),
                    BusinessType = x,
                    CashAmount = x,
                    Advance = x,
                    RemainSecuritiesPower = x,
                    Adhoc = x,
                    MatchedBuy = x,
                    PendingBuy = x,
                    MatchedBuyFee = x,
                    PendingBuyFee = x,
                    IntradayDebt = x,
                    InternalTransfer = x,
                    ExternalTransfer = x,
                    SMSfee = x,
                    AdvisorSelectFee = x,
                    VSDFee = x,
                    RemainDebt = x,
                    DebtInterest = x,
                    CreditLine = x,
                    ReceivableT0 = x,
                    ReceivableT1 = x,
                    ReceivableT2 = x,
                    ReceivableDividend = x,
                    ReceivableMatureCW = x,
                    ReceivableOddlot = x,
                    DVSD = x,
                    UnpaidVM = x,
                    TransferDFPTSToDVSD = x,
                    TransferDVSDToDFPTS = x,
                    FUTradingTax = x,
                    FSaving = x,
                    FSavingPower = x,
                    FSavingForPower = x,
                    BankSaving = x,
                    Desc = now.ToString()+Guid.NewGuid().ToString()
                });

            return collection;
        }

        public static CashLog GenerateCashLog()
        {
            var now = DateTime.Now;
            Random rnd = new Random();
            var x = rnd.Next(0, 1000);
            return new CashLog()
            {
                UpdateTime = now,
                CreateDate = now,
                TranDate = now,
                RefId = x + "_ref_id",
                SubId = x + "_sub_id",
                Status = Guid.NewGuid(),
                UpdateType = Guid.NewGuid(),
                ClientCode = ClientCodePrefix + x.ToString("D6"),
                StockCode = StockCodePrefix + x.ToString("D3"),
                BusinessType = x,
                CashAmount = x,
                Advance = x,
                RemainSecuritiesPower = x,
                Adhoc = x,
                MatchedBuy = x,
                PendingBuy = x,
                MatchedBuyFee = x,
                PendingBuyFee = x,
                IntradayDebt = x,
                InternalTransfer = x,
                ExternalTransfer = x,
                SMSfee = x,
                AdvisorSelectFee = x,
                VSDFee = x,
                RemainDebt = x,
                DebtInterest = x,
                CreditLine = x,
                ReceivableT0 = x,
                ReceivableT1 = x,
                ReceivableT2 = x,
                ReceivableDividend = x,
                ReceivableMatureCW = x,
                ReceivableOddlot = x,
                DVSD = x,
                UnpaidVM = x,
                TransferDFPTSToDVSD = x,
                TransferDVSDToDFPTS = x,
                FUTradingTax = x,
                FSaving = x,
                FSavingPower = x,
                FSavingForPower = x,
                BankSaving = x,
                Desc = now.ToString() + Guid.NewGuid().ToString()
            };
        }
    }

    public class HazelcastFactory
    {
        private readonly HazelcastOptions _options;

        public HazelcastFactory(IOptions<HazelcastOptions> options)
        {
            _options = options.Value;
            _options.NearCaches.Add("longpv2-test", new NearCacheOptions()
            {
                MaxSize = 5999999,
                InvalidateOnChange = true,
                EvictionPolicy = EvictionPolicy.Lru,
                InMemoryFormat = InMemoryFormat.Object,
                TimeToLiveSeconds = 60,
                MaxIdleSeconds = 3600,
            });
        }

        public ValueTask<IHazelcastClient> StartClientAsync()
            => HazelcastClientFactory.StartNewClientAsync(_options);
    }
}
