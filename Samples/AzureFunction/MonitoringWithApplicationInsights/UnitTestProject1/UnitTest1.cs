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
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
           
            string jsonContent = "{\n\t\"correlationid\":\"testcorrelation\",\n\t\"name\": \"test app insight\",\n\t\"properties\":\n\t\t{\n\t\t\t\"user\":\"Ali\",\n\t\t\t\"userTwo\":\"John\"\n\t\t}\n}\n\n";
            dynamic data = JsonConvert.DeserializeObject(jsonContent);
            var name = data?.name;
            var sequence = data?.sequence;
            var dimensions = data?.dimensions;
            var metrics = data?.metrics;
            string customDimensions = JsonConvert.SerializeObject(data?.properties);
            //data?.properties
            dynamic properties = JsonConvert.DeserializeObject(data?.properties);
        }

        [TestMethod]
        public void TestCustomDimensions()
        {
            string properties = "{\"user\":\"Ali\",\"userTwo\":\"John\"}";
            EventTelemetry eventTelemetry = new EventTelemetry();
            eventTelemetry.Name = "From Unit Test";
            eventTelemetry.Sequence = "1";
            var d = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(properties);

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
            

            TelemetryClient telemetryClient = new TelemetryClient();
            telemetryClient.InstrumentationKey = "";
            telemetryClient.TrackEvent(eventTelemetry);
        }
    }
}
