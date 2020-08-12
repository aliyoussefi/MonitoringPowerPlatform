using System;
using System.IO;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace PowerApps.Monitoring.MonitorToolExtractor
{
    
    public class MonitorToolExtractor
    {
        private readonly TelemetryClient telemetryClient;
        public readonly bool LogPerformance;
        public readonly bool LogNetwork;
        public readonly bool LogKPI;
        public readonly bool LogUnexpected;
        public MonitorToolExtractor()
        {
            this.LogPerformance = Convert.ToBoolean(System.Environment.GetEnvironmentVariable(
                                    "LogPerformance", EnvironmentVariableTarget.Process));
            this.LogNetwork = Convert.ToBoolean(System.Environment.GetEnvironmentVariable(
                                    "LogNetwork", EnvironmentVariableTarget.Process));
            this.LogKPI = Convert.ToBoolean(System.Environment.GetEnvironmentVariable(
                                    "LogKPI", EnvironmentVariableTarget.Process));
            this.LogUnexpected = Convert.ToBoolean(System.Environment.GetEnvironmentVariable(
                                    "LogUnexpected", EnvironmentVariableTarget.Process));

            string key = System.Environment.GetEnvironmentVariable(
                "APPINSIGHTS_INSTRUMENTATIONKEY", EnvironmentVariableTarget.Process);
            this.telemetryClient = new TelemetryClient();
            this.telemetryClient.InstrumentationKey = key;
            this.telemetryClient.TrackTrace("MonitorWithApplicationInsightsExample constructor called. Using Environment Variable APPINSIGHTS_INSTRUMENTATIONKEY to get Application Insights Key");

        }
        [FunctionName("BlobTriggeredMonitorToolToApplicationInsights")]
        public void Run([BlobTrigger("powerapps-monitortool-outputs/{name}", Connection = "")]Stream myBlob, string name, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
            string jsonResults;
            using (StreamReader r = new StreamReader(myBlob, System.Text.Encoding.UTF8))
            {
                jsonResults = r.ReadToEnd();
            }
            dynamic monitorOutput = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonResults);

            

            var versionId = monitorOutput?.Version;
            var SessionId = monitorOutput?.SessionId;
            this.telemetryClient.Context.Session.Id = SessionId;
            this.telemetryClient.Context.Cloud.RoleInstance = name;
            this.telemetryClient.Context.Cloud.RoleName = "powerapps-monitortool-outputs";
            this.telemetryClient.Context.GlobalProperties.Add("fileName", name);
            var Messages = monitorOutput?.Messages;
            foreach(var message in Messages)
            {
                TimeSpan messageTime = message?.time;
                string messageCategory = message?.category;
                string messageName = message?.name;
                string messageDataSource = message?.dataSource;
                switch (messageCategory)
                {
                    case "Performance": 
                        if (LogPerformance)
                        {
                            EventTelemetry eventTelemetry = new EventTelemetry();
                            eventTelemetry.Name = messageName;
                            eventTelemetry.Properties.Add("Message", messageDataSource);
                            eventTelemetry.Properties.Add("category", messageCategory);
                            telemetryClient.TrackEvent(eventTelemetry);
                        }
                        break;
                    case "Network":
                        if (LogNetwork)
                        {
                            if (!String.IsNullOrEmpty(messageName))
                            {
                                switch (messageName.ToLower())
                                {
                                    case "xmlhttprequest":
                                        RequestTelemetry xhrRequestTelemetry = new RequestTelemetry();
                                        xhrRequestTelemetry.ResponseCode = message?.status;
                                        xhrRequestTelemetry.Name = message?.name;
                                        xhrRequestTelemetry.Url = message?.dataSource;
                                        xhrRequestTelemetry.Duration = new TimeSpan(0, 0, 0, 0, Convert.ToInt32(message?.duration));
                                        xhrRequestTelemetry.Properties.Add("data", message?.data.ToString());
                                        xhrRequestTelemetry.Properties.Add("category", messageCategory);
                                        telemetryClient.TrackRequest(xhrRequestTelemetry);
                                        break;
                                    case "fetch":
                                        RequestTelemetry fetchRequestTelemetry = new RequestTelemetry();
                                        fetchRequestTelemetry.ResponseCode = message?.status;
                                        fetchRequestTelemetry.Name = message?.name;
                                        fetchRequestTelemetry.Url = message?.dataSource;
                                        fetchRequestTelemetry.Duration = new TimeSpan(0, 0, 0, 0, Convert.ToInt32(message?.duration));
                                        fetchRequestTelemetry.Properties.Add("data", message?.data.ToString());
                                        fetchRequestTelemetry.Properties.Add("category", messageCategory);
                                        telemetryClient.TrackRequest(fetchRequestTelemetry);
                                        break;
                                    default:
                                        DependencyTelemetry dependencyTelemetry = new DependencyTelemetry();
                                        dependencyTelemetry.ResultCode = message?.status;
                                        dependencyTelemetry.Name = message?.name;
                                        dependencyTelemetry.Target = message?.dataSource;
                                        dependencyTelemetry.Duration = new TimeSpan(0, 0, 0, 0, Convert.ToInt32(message?.duration));
                                        dependencyTelemetry.Properties.Add("data", message?.data.ToString());
                                        dependencyTelemetry.Properties.Add("category", messageCategory);
                                        telemetryClient.TrackDependency(dependencyTelemetry);
                                        break;
                                }
                            }
                            else
                            {
                                RequestTelemetry fetchRequestTelemetry = new RequestTelemetry();
                                fetchRequestTelemetry.ResponseCode = message?.status;
                                fetchRequestTelemetry.Name = message?.name;
                                fetchRequestTelemetry.Url = message?.dataSource;
                                fetchRequestTelemetry.Duration = new TimeSpan(0, 0, 0, 0, Convert.ToInt32(message?.duration));
                                fetchRequestTelemetry.Properties.Add("data", message?.data.ToString());
                                fetchRequestTelemetry.Properties.Add("category", messageCategory);
                                telemetryClient.TrackRequest(fetchRequestTelemetry);
                            }
                            
                        }
                        break;
                    case "KPI":
                        if (LogKPI)
                        {
                            PageViewTelemetry kpiTelemetry = new PageViewTelemetry();
                            kpiTelemetry.Name = messageName;
                            kpiTelemetry.Properties.Add("Message", messageDataSource);
                            kpiTelemetry.Properties.Add("data", message?.data.ToString());
                            kpiTelemetry.Metrics.Add("duration", Convert.ToDouble(message?.duration));
                            kpiTelemetry.Properties.Add("category", messageCategory);
                            telemetryClient.TrackPageView(kpiTelemetry);
                        }
                        
                        break;
                    
                    default:
                        if (LogUnexpected)
                        {
                            EventTelemetry unexpectedTelemetry = new EventTelemetry();
                            unexpectedTelemetry.Name = messageCategory;
                            unexpectedTelemetry.Properties.Add("Message", messageDataSource);
                            unexpectedTelemetry.Properties.Add("data", message?.data.ToString());
                            
                            telemetryClient.TrackEvent(unexpectedTelemetry);
                        }
                            break;
                }
                
                //message messageRow = Newtonsoft.Json.JsonConvert.DeserializeObject<message>(message);
            }
            telemetryClient.Flush();
        }

        public class message
        {
            public DateTime time { get; set; }
            public string category { get; set; }
            public string name { get; set; }
            public string dataSource { get; set; }
        }

        //public class MonitoringOutput
        //{
        //    public string 
        //}
    }
}
