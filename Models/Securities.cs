namespace HazelcastDemo.Models
{
    public class Securities
    {
        public string ClientCode { get; private set; }
        public string StockCode { get; private set; }
        public decimal? TradingReadyAvailableTotal { get; private set; }
        public decimal? MatchedBuyIntraday { get; private set; }
        public decimal? ReceivableSecuritiesT1 { get; private set; }
        public decimal? ReceivableSecuritiesT2 { get; private set; }
        public decimal? WaitingReceiveRightSecurities { get; private set; }
        public decimal? MortgageAtBank { get; private set; }
        public decimal? TransferRestricted { get; private set; }
        public decimal? TotalAmount { get; private set; }
        public decimal? MarketPrice { get; private set; }
        public decimal? MarketValue { get; private set; }
        public decimal? AveragePrice { get; private set; }
        public decimal? RootValue { get; private set; }
        public DateTime DateTime { get; private set; }

        public Securities(
            string clientCode,
            string stockCode,
            decimal? tradingReadyAvailableTotal,
            decimal? matchedBuyIntraday,
            decimal? receivableSecuritiesT1,
            decimal? receivableSecuritiesT2,
            decimal? waitingReceiveRightSecurities,
            decimal? mortgageAtBank,
            decimal? transferRestricted,
            decimal? totalAmount,
            decimal? marketPrice,
            decimal? marketValue,
            decimal? averagePrice,
            decimal? rootValue,
            DateTime dateTime)
        {
            ClientCode = clientCode;
            StockCode = stockCode;
            TradingReadyAvailableTotal = tradingReadyAvailableTotal;
            MatchedBuyIntraday = matchedBuyIntraday;
            ReceivableSecuritiesT1 = receivableSecuritiesT1;
            ReceivableSecuritiesT2 = receivableSecuritiesT2;
            WaitingReceiveRightSecurities = waitingReceiveRightSecurities;
            MortgageAtBank = mortgageAtBank;
            TransferRestricted = transferRestricted;
            TotalAmount = totalAmount;
            MarketPrice = marketPrice;
            MarketValue = marketValue;
            AveragePrice = averagePrice;
            RootValue = rootValue;
            DateTime = dateTime;
        }
    }
}
