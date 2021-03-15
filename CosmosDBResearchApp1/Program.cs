using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Cosmos;

namespace CosmosDBResearchApp1
{
    partial class Program
    {
        private static readonly string databaseName = "DemoDB";
        private static readonly string containerName = "SalesOrders";
        private static readonly string containerPartitionKey = "/AccountNumber";

        private static CosmosClient client;

        public static async Task Main(string[] args)
        {
            Console.WriteLine($"Hello Cosmos DB World! {System.AppDomain.CurrentDomain.FriendlyName}");

            var configuration = new ConfigurationBuilder()
                  .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                  .Build();

            string endpoint = configuration["EndPointUrl"];
            if (string.IsNullOrEmpty(endpoint))
            {
                throw new ArgumentNullException("Please specify a valid endpoint in the appSettings.json");
            }

            string authKey = configuration["AuthorizationKey"];
            if (string.IsNullOrEmpty(authKey) || string.Equals(authKey, "Super secret key"))
            {
                throw new ArgumentException("Please specify a valid AuthorizationKey in the appSettings.json");
            }

            CosmosClientOptions options = new CosmosClientOptions() { AllowBulkExecution = true };

            client = new CosmosClient(endpoint, authKey, options);

            DatabaseResponse databaseResponse = await client.CreateDatabaseIfNotExistsAsync(databaseName);
            Database database = databaseResponse;

            ContainerProperties containerProperties = new ContainerProperties(containerName, containerPartitionKey);
            ContainerRequestOptions containerRequestOptions = new ContainerRequestOptions { PopulateQuotaInfo = true };
            ContainerResponse containerResponse = await database.CreateContainerIfNotExistsAsync(
                containerProperties: containerProperties,
                throughputProperties: ThroughputProperties.CreateAutoscaleThroughput(autoscaleMaxThroughput: 4000),
                containerRequestOptions
            );
            Container container = containerResponse.Container;
            
            long indexTransformationProgress = long.Parse(containerResponse.Headers["x-ms-documentdb-collection-index-transformation-progress"]);
            if (indexTransformationProgress==100)
            {
                Console.WriteLine($" Index policy up to date");
            }
            else
            {
                Console.WriteLine($" Indexing in progress");
                Console.WriteLine($" Index status {indexTransformationProgress}");
                Console.WriteLine("Press Enter to run query");
            }
   
            string input = Console.ReadLine();

            // await InsertSalesOrders(container, 1000000);
            await QueryContainer(container);
            Console.WriteLine("Done!");

        }

    }
}
