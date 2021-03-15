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
            var timeCounter = 0;
            var itemCounter = 0;
            var totalRUCost = 0.0;

            Console.WriteLine("Retrieving sales order details");
          
            QueryDefinition queryDefinition = new QueryDefinition(queryString)
                .WithParameter("@ProductId", productId)
                .WithParameter("@AccountNumber", accountNumber);

            QueryRequestOptions queryOptions = new QueryRequestOptions();
            // --->  // queryOptions.PartitionKey = new PartitionKey(accountNumber);
            // --->  // queryOptions.MaxItemCount = 100000;


            using (FeedIterator<SalesOrderResult> salesOrders = container.GetItemQueryIterator<SalesOrderResult>(queryDefinition, null, queryOptions))
            {
                // ---> //  while (salesOrders.HasMoreResults)
                while (salesOrders.HasMoreResults)
                {
                    DateTime startTime = System.DateTime.Now;
                    FeedResponse<SalesOrderResult> salesOrderResponse = await salesOrders.ReadNextAsync();
                    timeCounter = timeCounter + System.DateTime.Now.Subtract(startTime).Milliseconds;
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

            Console.WriteLine($"Total sales order items retuned : {itemCounter} record");
            Console.WriteLine($"Total Request Units consumed : {totalRUCost} RU");
            Console.WriteLine($"Time taken : {timeCounter} ms");
        }
        
    }
}