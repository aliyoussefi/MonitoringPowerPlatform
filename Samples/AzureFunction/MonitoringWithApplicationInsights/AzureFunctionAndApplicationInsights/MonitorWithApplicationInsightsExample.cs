using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.JsonPatch.Operations;
using System.Net.Http;
using System.Net;

namespace Dynamics365.Monitoring.AzureFunctions
{
    public class MonitorWithApplicationInsightsExample
    {
        private readonly TelemetryClient telemetryClient;

        /// Using dependency injection will guarantee that you use the same configuration for telemetry collected automatically and manually.
        //public MonitorWithApplicationInsightsExample(TelemetryConfiguration telemetryConfiguration)
        //{
        //    this.telemetryClient = new TelemetryClient(telemetryConfiguration);
        //    this.telemetryClient.TrackTrace("MonitorWithApplicationInsightsExample constructor called. Using injected TelemetryConfiguration");
        //}

        public MonitorWithApplicationInsightsExample()
        {
            string key = System.Environment.GetEnvironmentVariable(
                "APPINSIGHTS_INSTRUMENTATIONKEY", EnvironmentVariableTarget.Process);
            this.telemetryClient = new TelemetryClient();
            this.telemetryClient.InstrumentationKey = key;
            this.telemetryClient.TrackTrace("MonitorWithApplicationInsightsExample constructor called. Using Environment Variable APPINSIGHTS_INSTRUMENTATIONKEY to get Application Insights Key");
        }

        [FunctionName("MonitorWithApplicationInsightsExample")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ExecutionContext exCtx,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            telemetryClient.TrackTrace("MonitorWithApplicationInsightsExample Function called.");
            string name = req.Query["name"];
            foreach(var header in req.Headers){
                log.LogInformation(header.Key, header.Value);
            }
            telemetryClient.TrackTrace(exCtx.InvocationId.ToString());
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;
            telemetryClient.TrackTrace("MonitorWithApplicationInsightsExample Function ended. Returning " + $"Hello, {name}");
            return name != null
                ? (ActionResult)new OkObjectResult($"Hello, {name}")
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }

        [FunctionName("MonitorWithApplicationInsightsExampleInput")]
        public async Task<IActionResult> RunInput(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] string input, ExecutionContext exCtx,
        ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            telemetryClient.TrackTrace("MonitorWithApplicationInsightsExample Function called.");
            //string name = req.Query["name"];
            //foreach (var header in req.Headers)
            //{
            //    log.LogInformation(header.Key, header.Value);
            //}
                
            //string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            telemetryClient.TrackTrace("MonitorWithApplicationInsightsExample Function ended. Returning " + $"Hello, {input}");
            ActionResult rtnObject = new OkObjectResult(input);
            
            return input != null
                ? (ActionResult)new OkObjectResult($"Hello, {input}")
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }

        [FunctionName("MonitorWithApplicationInsightsExampleHttpResponseMessage")]
        public async Task<HttpResponseMessage> ReturnResponseMessage(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestMessage req, ExecutionContext exCtx,
        ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            telemetryClient.TrackTrace("MonitorWithApplicationInsightsExample Function called.");
            try
            {
                //string name = req.Query["name"];
                foreach (var header in req.Headers)
                {
                    log.LogInformation(header.Key, header.Value);
                }

                var content = req.Content;
                string jsonContent = content.ReadAsStringAsync().Result;
                dynamic data = JsonConvert.DeserializeObject(jsonContent);
                var test = data?.name;
                telemetryClient.TrackTrace("MonitorWithApplicationInsightsExample Function ended. Returning " + $"Hello, {test}");
                //Create object for partner Model to bind the response on it

                RequestModel objRequestModel = new RequestModel();

                objRequestModel.FirstName = data.FirstName;
                objRequestModel.LastName = data.LastName;

                //Return Request Model
                HttpResponseMessage rtnObject = req.CreateResponse(HttpStatusCode.OK, objRequestModel);
                rtnObject.Headers.Add("InvocationId", exCtx.InvocationId.ToString());
                return rtnObject;
                //return test != null
                //    ? (ActionResult)new OkObjectResult($"Hello, {test}")
                //    : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
            }
            catch (Exception ex)
            {

                return req.CreateResponse(HttpStatusCode.BadRequest, "Cannot Create Request! Reason: {0}", string.Format(ex.Message));
            }

        }

        public class RequestModel
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }

        }
    }
}
