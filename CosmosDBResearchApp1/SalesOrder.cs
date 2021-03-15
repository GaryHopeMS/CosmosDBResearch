using System;
using Newtonsoft.Json;

namespace CosmosDBResearchApp1
{
    public class SalesOrder
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "PONumber")]
        public string PurchaseOrderNumber { get; set; }

        // used to set expiration policy
        [JsonProperty(PropertyName = "ttl", NullValueHandling = NullValueHandling.Ignore)]
        public int? TimeToLive { get; set; }

        public DateTime OrderDate { get; set; }
        public DateTime ShippedDate { get; set; }
        public string AccountNumber { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal Freight { get; set; }
        public decimal TotalDue { get; set; }
        public SalesOrderDetail[] LineItems { get; set; }
    }

    public class SalesOrderDetail
    {
        public int OrderQty { get; set; }
        public int ProductId { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }

    public class SalesOrderResult
    {
        [JsonProperty(PropertyName = "id")]
        public string OrderId { get; set; }
        public string AccountNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public int ProductId { get; set; }
        public int OrderQty { get; set; }
     }

}