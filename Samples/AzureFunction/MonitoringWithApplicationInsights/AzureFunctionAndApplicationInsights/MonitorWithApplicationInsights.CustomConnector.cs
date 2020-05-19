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
using Microsoft.ApplicationInsights.DataContracts;
using System.Collections.Generic;

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

        #region trace
        [FunctionName("TrackTrace")]
        public async Task<HttpResponseMessage> TrackTrace(
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

                HttpResponseMessage rtnObject = req.CreateResponse(HttpStatusCode.OK, objRequestModel);
                rtnObject.Headers.Add("InvocationId", exCtx.InvocationId.ToString());
                return rtnObject;
            }
            catch (Exception ex)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "Cannot Create Request! Reason: {0}", string.Format(ex.Message));
            }

        }
        #endregion
        #region event


        [FunctionName("TrackEvent")]
        public async Task<HttpResponseMessage> TrackEvent(
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

                var properties = JsonConvert.DeserializeObject<Dictionary<string, string>>(data?.properties);
                telemetryClient.TrackEvent("MonitorWithApplicationInsightsExample Function ended. Returning " + $"Hello, {test}");

                RequestModel objRequestModel = new RequestModel();
                objRequestModel.name = data.name;
                objRequestModel.eventType = "customEvent";
                objRequestModel.success = true;

                HttpResponseMessage rtnObject = req.CreateResponse(HttpStatusCode.OK, objRequestModel);
                rtnObject.Headers.Add("InvocationId", exCtx.InvocationId.ToString());
                return rtnObject;
            }
            catch (Exception ex)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "Cannot Create Request! Reason: {0}", string.Format(ex.Message));
            }

        }
        #endregion
        #region pageView
        [FunctionName("TrackPageView")]
        public async Task<HttpResponseMessage> TrackPageView(
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
                telemetryClient.TrackPageView("MonitorWithApplicationInsightsExample Function ended. Returning " + $"Hello, {test}");

                RequestModel objRequestModel = new RequestModel();

                HttpResponseMessage rtnObject = req.CreateResponse(HttpStatusCode.OK, objRequestModel);
                rtnObject.Headers.Add("InvocationId", exCtx.InvocationId.ToString());
                return rtnObject;
            }
            catch (Exception ex)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "Cannot Create Request! Reason: {0}", string.Format(ex.Message));
            }

        }
        #endregion
        #region exception
        [FunctionName("TrackException")]
        public async Task<HttpResponseMessage> TrackException(
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
                ExceptionTelemetry exceptionTelemetry = new ExceptionTelemetry();
                telemetryClient.TrackException(exceptionTelemetry);

                RequestModel objRequestModel = new RequestModel();

                HttpResponseMessage rtnObject = req.CreateResponse(HttpStatusCode.OK, objRequestModel);
                rtnObject.Headers.Add("InvocationId", exCtx.InvocationId.ToString());
                return rtnObject;
            }
            catch (Exception ex)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "Cannot Create Request! Reason: {0}", string.Format(ex.Message));
            }

        }
        #endregion
        #region dependency
        [FunctionName("TrackDependency")]
        public async Task<HttpResponseMessage> TrackDependency(
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
                //telemetryClient.TrackDependency("MonitorWithApplicationInsightsExample Function ended. Returning " + $"Hello, {test}");

                RequestModel objRequestModel = new RequestModel();

                HttpResponseMessage rtnObject = req.CreateResponse(HttpStatusCode.OK, objRequestModel);
                rtnObject.Headers.Add("InvocationId", exCtx.InvocationId.ToString());
                return rtnObject;
            }
            catch (Exception ex)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "Cannot Create Request! Reason: {0}", string.Format(ex.Message));
            }

        }
        #endregion
        #region requests
        [FunctionName("TrackRequest")]
        public async Task<HttpResponseMessage> TrackRequest(
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
                DateTimeOffset timestamp = data?.timestamp;
                TimeSpan duration = data?.duration;
                string responseCode = data?.responseCode;
                bool success = data?.success;

                telemetryClient.TrackRequest("", timestamp, duration, responseCode, success);

                RequestModel objRequestModel = new RequestModel();

                HttpResponseMessage rtnObject = req.CreateResponse(HttpStatusCode.OK, objRequestModel);
                rtnObject.Headers.Add("InvocationId", exCtx.InvocationId.ToString());
                return rtnObject;
            }
            catch (Exception ex)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "Cannot Create Request! Reason: {0}", string.Format(ex.Message));
            }

        }
        #endregion
        public class RequestModel
        {
            public string name { get; set; }
            public string eventType { get; set; }
            public bool success { get; set; }
        }
    }
}
