/*
# This Sample Code is provided for the purpose of illustration only and is not intended to be used in a production environment. 
# THIS SAMPLE CODE AND ANY RELATED INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, 
# INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE. 
# We grant You a nonexclusive, royalty-free right to use and modify the Sample Code and to reproduce and distribute the object code form of the Sample Code, provided that. 
# You agree: 
# (i) to not use Our name, logo, or trademarks to market Your software product in which the Sample Code is embedded; 
# (ii) to include a valid copyright notice on Your software product in which the Sample Code is embedded; 
# and (iii) to indemnify, hold harmless, and defend Us and Our suppliers from and against any claims or lawsuits, including attorneys’ fees, that arise or result from the use or distribution of the Sample Code 
*/
using Azure;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.Threading;

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
        [Function("TrackTrace")]
        public async Task<HttpResponseData> TrackTrace(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestData req, FunctionContext exCtx)
        {
            var log = exCtx.GetLogger(nameof(TrackTrace));
            log.LogInformation(String.Format("{0} Function called.", exCtx.FunctionDefinition.Name));
            try
            {
                foreach (var header in req.Headers)
                {
                    log.LogInformation(header.Key, header.Value);
                }

                //var content = req.Content;
                string jsonContent = req.ReadAsStringAsync().Result;
                dynamic data = JsonConvert.DeserializeObject(jsonContent);
                var test = data?.name;
                telemetryClient.TrackTrace("MonitorWithApplicationInsightsExample Function ended. Returning " + $"Hello, {test}");

                RequestModel objRequestModel = new RequestModel();

                HttpResponseData rtnObject = req.CreateResponse(HttpStatusCode.OK);
                await rtnObject.WriteAsJsonAsync(objRequestModel);
                await rtnObject.Body.WriteAsync(System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(objRequestModel)));
                rtnObject.Headers.Add("InvocationId", exCtx.InvocationId.ToString());
                return rtnObject;
            }
            catch (Exception ex)
            {
                await req.Body.WriteAsync(System.Text.Encoding.UTF8.GetBytes(ex.Message));
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }

        }
        #endregion
        #region event
        [Function("TrackEvent")]
        public async Task<HttpResponseData> TrackEvent(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestData req, FunctionContext exCtx)
        {
            var log = exCtx.GetLogger(nameof(TrackTrace));
            log.LogInformation(String.Format("{0} Function called.", exCtx.FunctionDefinition.Name));
            try
            {
                foreach (var header in req.Headers)
                {
                    //log.LogInformation(header.Key, header.Value);
                }

                //var content = req.Content;
                string jsonContent = req.ReadAsStringAsync().Result;
                dynamic data = JsonConvert.DeserializeObject(jsonContent);
                string correlationId = data?.correlationid;
                var name = data?.name;
                var duration = data?.duration;
                var dependencyId = data?.id;
                var actionTarget = data?.actionName;
                var Type = data?.type;
                var sequence = data?.sequence;

                string customDimensions = JsonConvert.SerializeObject(data?.properties);

                EventTelemetry eventTelemetry = new EventTelemetry();
                eventTelemetry.Name = name;
                eventTelemetry.Sequence = sequence;

                var d = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(customDimensions);

                foreach (var key in d.Keys)
                {
                    // check if the value is not null or empty.
                    if (!string.IsNullOrEmpty(d[key]))
                    {
                        var value = d[key];
                        // code to do something with 
                        eventTelemetry.Properties.Add(key, value);
                    }
                }

                telemetryClient.TrackEvent(eventTelemetry);

                RequestModel objRequestModel = new RequestModel();
                objRequestModel.name = data.name;
                objRequestModel.eventType = "customEvent";
                objRequestModel.success = true;

                HttpResponseData rtnObject = req.CreateResponse(HttpStatusCode.OK);
                await rtnObject.WriteAsJsonAsync(objRequestModel);
                rtnObject.Headers.Add("InvocationId", exCtx.InvocationId.ToString());
                return rtnObject;
            }
            catch (Exception ex)
            {
                await req.Body.WriteAsync(System.Text.Encoding.UTF8.GetBytes(ex.Message));
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }

        }
        #endregion
        #region pageView
        [Function("TrackPageView")]
        public async Task<HttpResponseData> TrackPageView(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestData req, FunctionContext exCtx)
        {
            var log = exCtx.GetLogger(nameof(TrackTrace));
            log.LogInformation(String.Format("{0} Function called.", exCtx.FunctionDefinition.Name));
            try
            {
                foreach (var header in req.Headers)
                {
                    log.LogInformation(header.Key, header.Value);
                }

                //var content = req.Content;
                string jsonContent = req.ReadAsStringAsync().Result;
                dynamic data = JsonConvert.DeserializeObject(jsonContent);
                var test = data?.name;
                telemetryClient.TrackPageView("MonitorWithApplicationInsightsExample Function ended. Returning " + $"Hello, {test}");

                RequestModel objRequestModel = new RequestModel();

                HttpResponseData rtnObject = req.CreateResponse(HttpStatusCode.OK);
                await rtnObject.WriteAsJsonAsync(objRequestModel);
                rtnObject.Headers.Add("InvocationId", exCtx.InvocationId.ToString());
                return rtnObject;
            }
            catch (Exception ex)
            {
                await req.Body.WriteAsync(System.Text.Encoding.UTF8.GetBytes(ex.Message));
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }

        }
        #endregion
        #region exception
        private class error {
            public string code { get; set; }
            public string message { get; set; }
        }
        [Function("TrackException")]
        public async Task<HttpResponseData> TrackException(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestData req, FunctionContext exCtx)
        {
            var log = exCtx.GetLogger(nameof(TrackTrace));
            log.LogInformation(String.Format("{0} Function called.", exCtx.FunctionDefinition.Name));
            try
            {
                foreach (var header in req.Headers)
                {
                    //log.LogInformation(header.Key, header.Value);
                }

                var content = req.ReadAsStringAsync().Result;
                //string jsonContent = content.ReadAsync().Result;
                dynamic data = JsonConvert.DeserializeObject(content);
                var test = data?.name;
                var correlationId = data?.correlationid;
                string exceptionMessage = data?.exceptionMessage;

                string workflowName = data?.workflowName;
                string environmentName = data?.environmentName;
                //Run Info
                string workflowRunName = data?.workflowRunName;
                string actionName = data?.actionName;
                string severityLevel = data?.severityLevel;
                string exceptionCode = data?.exceptionCode;
                //Newtonsoft.Json.Linq.JObject error = data?.error;
                //error workflowDataData = error.ToObject<error>();
                //var workflowData = data?.workflow;
                //error workflowDataData = JsonConvert.DeserializeObject<error>(error);
                ExceptionTelemetry exceptionTelemetry = new ExceptionTelemetry();
                //SeverityLevel
                //Critical, Error, informational, warning, verbose
                exceptionTelemetry.SeverityLevel = SeverityLevel.Critical;


                exceptionTelemetry.Exception = new Exception(exceptionMessage);
                exceptionTelemetry.ProblemId = exceptionCode;
                exceptionTelemetry.Message = exceptionMessage;
                StackFrame stackFrame = new StackFrame(workflowName, workflowRunName, 0, 0, actionName);

                //Send Exception
                telemetryClient.TrackException(exceptionTelemetry);

                RequestModel objRequestModel = new RequestModel();

                HttpResponseData rtnObject = req.CreateResponse(HttpStatusCode.OK);
                rtnObject.Headers.Add("InvocationId", exCtx.InvocationId.ToString());
                return rtnObject;
            }
            catch (Exception ex)
            {
                await req.Body.WriteAsync(System.Text.Encoding.UTF8.GetBytes(ex.Message));
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }

        }
        #endregion
        #region dependency
        [Function("TrackDependency")]
        public async Task<HttpResponseData> TrackDependency(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestData req, FunctionContext exCtx)
        {
            var log = exCtx.GetLogger(nameof(TrackTrace));
            log.LogInformation(String.Format("{0} Function called.", exCtx.FunctionDefinition.Name));
            try
            {

                IEnumerable<string> values;
                string session = string.Empty;
                if (req.Headers.TryGetValues("X-BB-SESSION", out values))
                {
                    session = values.FirstOrDefault();
                }

                //var content = req.Content;
                string jsonContent = req.ReadAsStringAsync().Result;
                dynamic data = JsonConvert.DeserializeObject(jsonContent);
                string correlationId = data?.correlationid;
                var name = data?.name;
                var duration = data?.duration;
                var dependencyId = data?.id;
                var actionTarget = data?.actionName;
                var Type = data?.type;

                DependencyTelemetry dependencyTelemetry = new DependencyTelemetry();
                dependencyTelemetry.Data = name;
                dependencyTelemetry.Duration = duration;
                dependencyTelemetry.Id = dependencyId;
                dependencyTelemetry.Name = name;
                dependencyTelemetry.Target = actionTarget;
                dependencyTelemetry.Type = Type;
                telemetryClient.TrackDependency(dependencyTelemetry);

                RequestModel objRequestModel = new RequestModel();
                objRequestModel.success = true;
                objRequestModel.correlationid = correlationId;
                objRequestModel.eventType = Type;
                objRequestModel.name = name;
                HttpResponseData rtnObject = req.CreateResponse(HttpStatusCode.OK);
                rtnObject.Headers.Add("InvocationId", exCtx.InvocationId.ToString());
                rtnObject.Headers.Add("test", "test");
                await rtnObject.WriteAsJsonAsync(objRequestModel);

                return rtnObject;
            }
            catch (Exception ex)
            {
                req.CreateResponse(HttpStatusCode.BadRequest);
                await req.CreateResponse().WriteAsJsonAsync(ex.Message);
                throw ex;
            }

        }
        #endregion
        #region requests
        [Function("TrackRequest")]
        public async Task<HttpResponseData> TrackRequest(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestData req, FunctionContext exCtx)
        {
            var log = exCtx.GetLogger(nameof(TrackTrace));
            log.LogInformation(String.Format("{0} Function called.", exCtx.FunctionDefinition.Name));
            try
            {
                foreach (var header in req.Headers)
                {
                    //log.LogInformation(header.Key, header.Value);
                }

                //var content = req.Content;
                string jsonContent = req.ReadAsStringAsync().Result;
                dynamic data = JsonConvert.DeserializeObject(jsonContent);
                string test = data?.name;
                DateTimeOffset timestamp = DateTime.Now;
                TimeSpan duration = data?.duration;
                string responseCode = data?.responseCode;
                bool success = data?.success;
                log.LogInformation(exCtx.InvocationId.ToString() + " with name " + test);
                telemetryClient.TrackRequest(test, timestamp, duration, responseCode, success);

                RequestModel objRequestModel = new RequestModel();
                objRequestModel.correlationid = data?.correlationId ?? exCtx.InvocationId.ToString();
                HttpResponseData rtnObject = req.CreateResponse(HttpStatusCode.OK);
                rtnObject.Headers.Add("InvocationId", exCtx.InvocationId.ToString());
                return rtnObject;
            }
            catch (Exception ex)
            {
                await req.Body.WriteAsync(System.Text.Encoding.UTF8.GetBytes(ex.Message));
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }

        }
        #endregion
        public class RequestModel
        {
            public string correlationid { get; set; }
            public string name { get; set; }
            public string eventType { get; set; }
            public bool success { get; set; }
        }
    }
}
