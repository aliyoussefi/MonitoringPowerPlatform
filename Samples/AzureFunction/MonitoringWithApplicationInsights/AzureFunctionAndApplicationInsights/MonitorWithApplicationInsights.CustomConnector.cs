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
                    //log.LogInformation(header.Key, header.Value);
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
                    //log.LogInformation(header.Key, header.Value);
                }

                var content = req.Content;
                string jsonContent = content.ReadAsStringAsync().Result;
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
        private class error {
            public string code { get; set; }
            public string message { get; set; }
        }
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
                    //log.LogInformation(header.Key, header.Value);
                }

                var content = req.Content;
                string jsonContent = content.ReadAsStringAsync().Result;
                dynamic data = JsonConvert.DeserializeObject(jsonContent);
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
                    //log.LogInformation(header.Key, header.Value);
                }

                var content = req.Content;
                string jsonContent = content.ReadAsStringAsync().Result;
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
                    //log.LogInformation(header.Key, header.Value);
                }

                var content = req.Content;
                string jsonContent = content.ReadAsStringAsync().Result;
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
            public string correlationid { get; set; }
            public string name { get; set; }
            public string eventType { get; set; }
            public bool success { get; set; }
        }
    }
}
