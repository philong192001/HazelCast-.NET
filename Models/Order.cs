using Hazelcast.Serialization;

namespace HazelcastDemo.Models
{
    public class Order : IPortable
    {
        public Order()
        {
        }

        public Order(string? clientCode, int orderId, string? actionType, string? symbol, int price, int quantity, int status, string? dateTime)
        {
            ClientCode = clientCode;
            OrderId = orderId;
            ActionType = actionType;
            Symbol = symbol;
            Price = price;
            Quantity = quantity;
            Status = status;
            DateTime = dateTime;
        }

        public string? ClientCode { get;  set; }
        public int OrderId { get;  set; }
        public string? ActionType { get;  set; }
        public string? Symbol { get;  set; }
        public int Price { get;  set; }
        public int Quantity { get;  set; }
        public int Status { get; set; }
        public string? DateTime { get;  set; }

        public const int ClassId = 1;
        int IPortable.ClassId => ClassId;
        int IPortable.FactoryId => FactoryId;
        public int FactoryId => 1;

        public void ReadPortable(IPortableReader reader)
        {
            ClientCode = reader.ReadString("clientCode");
            OrderId = reader.ReadInt("orderId");
            ActionType = reader.ReadString("actionType");
            Symbol = reader.ReadString("symbol");
            Price = reader.ReadInt("price");
            Quantity = reader.ReadInt("quantity");
            Status = reader.ReadInt("status");
            DateTime = reader.ReadString("DateTime");
        }

        public void WritePortable(IPortableWriter writer)
        {
            writer.WriteString("clientCode", ClientCode);
            writer.WriteInt("orderId", OrderId);
            writer.WriteString("actionType", ActionType);
            writer.WriteString("symbol", Symbol);
            writer.WriteInt("price", Price);
            writer.WriteInt("status",Status);
            writer.WriteInt("quantity", Quantity);
            writer.WriteString("DateTime", DateTime);
        }
    }
}
