using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Cosmos;

namespace CosmosDBResearchApp1
{
    partial class Program

    { private static async Task InsertSalesOrders(Container container, int numberOfSalesOrders)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Inserting {numberOfSalesOrders} into {container.Id}");

            int maxDegreeOfParallelismPerWorker = 50;
            bool useBulk = true;

            int totalItems = 0;
            int batches = 0;
            int batchCounter = 0;
            for (int items = 1; items <= numberOfSalesOrders; items++)
            {
                
                List<Task> concurrentTasks = new List<Task>(maxDegreeOfParallelismPerWorker);
                SalesOrder salesOrder = GetSalesOrderSample(items);
                if (useBulk)
                {
                    concurrentTasks.Add(container.CreateItemAsync(salesOrder));
                }
                else
                {
                   await container.CreateItemAsync(salesOrder);
                }
                totalItems++;
                batchCounter++;

                if (batchCounter >= maxDegreeOfParallelismPerWorker)
                {
                    batchCounter = 0;
                    await Task.WhenAll(concurrentTasks);
                    Console.WriteLine($" inserted {items} docs in {batches} batches using bulk={useBulk}");
                    concurrentTasks.Clear();
                    batches++;
                }

            }

            Console.WriteLine($"Inserted {totalItems} items into {container.Id}");

        }
        static SalesOrder GetSalesOrderSample(int recordNumber)
        {
            var x = new Random().Next(10000, 99999);
            double basePrice = 41.63;

            var productId1 = Convert.ToInt32(1234 + (recordNumber%10));
            var productId2 = Convert.ToInt32(1234 + (recordNumber%3)-1);

            SalesOrder salesOrder = new SalesOrder
            {
                Id = (x * 10000 + recordNumber).ToString(),
                AccountNumber = "AC" + Convert.ToString(recordNumber % 10),
                PurchaseOrderNumber = "PO" + (recordNumber + productId1).ToString(),
                OrderDate = new DateTime(2005, 7, (1+x%30)),
                SubTotal = decimal.Round(Convert.ToDecimal(((basePrice-(x%10)) * ((x%10)+6)) + (basePrice * (x%10)+1)),2),
                TaxAmount = decimal.Round(Convert.ToDecimal((((basePrice-(x%10)) * ((x%10)+6)) + (basePrice * (x % 10) + 1)) * 0.14),2),
                Freight = decimal.Round(Convert.ToDecimal(1.0* x / 1000),2),
                TotalDue = decimal.Round(Convert.ToDecimal((((basePrice - (x % 10)) * ((x % 10) + 6)) + (basePrice * (x % 10) + 1)) * 1.14) + Convert.ToDecimal(1.0 * x / 1000),2),
                LineItems = new SalesOrderDetail[]
                {
                     new SalesOrderDetail
                    {

                        OrderQty = (x%10)+6,
                        ProductId = productId1,
                        UnitPrice = decimal.Round(Convert.ToDecimal((basePrice-(x%10))),2),
                        LineTotal = decimal.Round(Convert.ToDecimal((basePrice-(x%10))*((x%10)+6)),2)
                    },

                    new SalesOrderDetail
                    {
                        OrderQty = (x % 10)+1,
                        ProductId = productId2,
                        UnitPrice = decimal.Round(Convert.ToDecimal(basePrice),2),
                        LineTotal = decimal.Round(Convert.ToDecimal(basePrice*(x % 10)+1),2)
                    }
                }
            };

            return salesOrder;
        }
    }
}