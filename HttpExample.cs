using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace My.Functions
{
    public static class HttpExample
    {
        [FunctionName("HttpExample")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [CosmosDB(databaseName: "my-database", collectionName: "my-container",
                ConnectionStringSetting = "mongodb://cosmosdbhender:AeWl97eZqqpoam60wRmqXxgxpSbph7K6aEWjZCTGGFF4c8BfyStj69p4tP5rVDI3kNMaB6oDiO1CsWWRAablyw==@cosmosdbhender.mongo.cosmos.azure.com:10255/?ssl=true&replicaSet=globaldb&retrywrites=false&maxIdleTimeMS=120000&appName=@cosmosdbhender@"
                )]IAsyncCollector<dynamic> documentsOut,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            if (!string.IsNullOrEmpty(name))
            {
                // Add a JSON document to the output container.
                await documentsOut.AddAsync(new
                {
                    // create a random ID
                    _id = System.Guid.NewGuid().ToString(),
                    name = name
                });
            }

            string responseMessage = string.IsNullOrEmpty(name)
                ? "empty name query"
                : name;

            return new OkObjectResult(responseMessage);
        }
    }
}