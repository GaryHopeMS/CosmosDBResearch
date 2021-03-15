using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Cosmos;

namespace CosmosDBResearchApp1
{
    partial class Program
    {
        private const string queryString = "SELECT o.AccountNumber, o.id, o.OrderDate, i.ProductId, i.OrderQty, i.UnitPrice FROM Orders o JOIN i IN o.LineItems WHERE o.AccountNumber = @AccountNumber AND i.ProductId = @ProductId";
        private const int productId = 1238;
        private const string accountNumber = "AC1";

        private static async Task QueryContainer(Container container)
        {
            DateTime startTime = System.DateTime.Now;
            Console.WriteLine("Retrieving sales order details");
            var itemCounter = 0;
            var totalRUCost = 0.0;
            QueryDefinition queryDefinition = new QueryDefinition(queryString)
                .WithParameter("@ProductId", productId)
                .WithParameter("@AccountNumber", accountNumber);

            QueryRequestOptions queryOptions = new QueryRequestOptions();
            // queryOptions.PartitionKey = new PartitionKey("AC1");
            // --->  // queryOptions.MaxItemCount = 100000;


            using (FeedIterator<SalesOrderResult> salesOrders = container.GetItemQueryIterator<SalesOrderResult>(queryDefinition, null, queryOptions))
            {
                // ---> //  while (salesOrders.HasMoreResults)
                while (salesOrders.HasMoreResults)
                {
                    FeedResponse<SalesOrderResult> salesOrderResponse = await salesOrders.ReadNextAsync();
                    totalRUCost += salesOrderResponse.RequestCharge;
                    foreach (var salesOrder in salesOrderResponse)
                    {
                        {
                            Console.WriteLine($"Item {salesOrder.OrderId} ({salesOrder.OrderDate}) - Product:{salesOrder.ProductId} Quantity:{salesOrder.OrderQty} ");
                            itemCounter++;
                        }
                    }

                }
            }

            DateTime endTime = System.DateTime.Now;
            Console.WriteLine($"Total sales order items retuned {itemCounter}");
            Console.WriteLine($"Total Request Units (RU) consumed {totalRUCost} ");
            Console.WriteLine($"Time take {endTime.Subtract(startTime).Milliseconds} ms");
        }
        
    }
}