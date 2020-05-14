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
    public class MonitorWithApplicationInsightsCustomConnector
    {
        private readonly TelemetryClient telemetryClient;

        public MonitorWithApplicationInsightsCustomConnector()
        {
            string key = System.Environment.GetEnvironmentVariable(
                "APPINSIGHTS_INSTRUMENTATIONKEY", EnvironmentVariableTarget.Process);
            this.telemetryClient = new TelemetryClient();
            this.telemetryClient.InstrumentationKey = key;
            this.telemetryClient.TrackTrace("MonitorWithApplicationInsightsExample constructor called. Using Environment Variable APPINSIGHTS_INSTRUMENTATIONKEY to get Application Insights Key");
        }


        [FunctionName("TrackTelemetry")]
        public async Task<HttpResponseMessage> TrackTelemetry(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestMessage req, ExecutionContext exCtx,
        ILogger log)
        {
            log.LogInformation(String.Format("{0} Function called.", exCtx.FunctionName));
            try
            {
                foreach (var header in req.Headers)
                {
                    log.LogInformation(header.Key, header.Value);
                }

                var content = req.Content;
                string jsonContent = content.ReadAsStringAsync().Result;
                dynamic data = JsonConvert.DeserializeObject(jsonContent);
                var test = data?.name;
                telemetryClient.TrackTrace("MonitorWithApplicationInsightsExample Function ended. Returning " + $"Hello, {test}");

                RequestModel objRequestModel = new RequestModel();
                objRequestModel.FirstName = data.FirstName;
                objRequestModel.LastName = data.LastName;

                HttpResponseMessage rtnObject = req.CreateResponse(HttpStatusCode.OK, objRequestModel);
                rtnObject.Headers.Add("InvocationId", exCtx.InvocationId.ToString());
                return rtnObject;
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
