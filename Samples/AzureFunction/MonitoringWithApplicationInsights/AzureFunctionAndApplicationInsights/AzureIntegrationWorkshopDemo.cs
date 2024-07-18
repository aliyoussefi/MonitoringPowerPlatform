using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureFunctionAndApplicationInsights
{
    public static class AzureIntegrationWorkshopDemo
    {
        [Function("AzureIntegrationWorkshopDemo")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,FunctionContext executionContext)
        {
            var log = executionContext.GetLogger("AzureIntegrationWorkshopDemo");
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";
            executionContext.GetHttpResponseData()?.Headers.Add("InvocationId", executionContext.InvocationId.ToString());
            return new OkObjectResult(responseMessage);
        }
    }
}
