# Monitoring the Power Platform: Dataverse - Plug-in Logging

## Summary

Dataverse represents a complete solution for line of business applications providing organizations a quick and secure way to access data. The Dataverse data model is customizable and extendable allowing makers and administrators the ability to structure and serve data to the pillars of the Power Platform. Dataverse also comes with an API empowring developers to incorporate business logic and extend the accessibility of organizational data across cloud and hybrid workloads.

This article will focus on the extensibility of the API, specifically how to implement tracing, also known as logging, for custom business logic. We will look at using Dynamics 365 tools as well as Azure hosted services such as Azure Application Insights. Before that, we'll begin by describing the Execution Context of an event working with the Dataverse API. This is important to understand where developers can introduce custom code that help achieve functionality not present in the platform. Next, we will reference step by step articles detailing how to turn on logging. From there we'll go into how to implement different types of logging patterns with this business logic. Finally, we will look at the trace logs delivered and how to query or better understand what's presented.

## How and Where to Add Custom Business Logic

### Common Dataverse Actions

As stated at the beginning of this article, the Dataverse provides the ability to access data to various pillars and services of the Power Platform as well as other services. For instance, Power Automate can be used to take query and send data to connected services such as Microsoft Outlook or Microsoft Teams. Power Apps Canvas Apps take advantage of the Dataverse connector to display and create data in a pixel perfect presentation. Power BI can provide powerful business intelligence capabilities complete with artificial intelligence helping users drill down into Dataverse data.

These events include, but not limited to, standard data operations such as:

- creating or updating data
- reading or associating different types of data
- removing data

Each of these events are used as part of an entry way into the Dataverse API when accessing the data within. Likewise, each event, provide an opportunity for developers to insert business logic specific to an organization's needs. The custom business logic created by developers are encapsulated into what's called a Plug-in. The remainder of this article will focus on Plug-ins and how developers can implement a monitoring strategy.

### What's a Plug-In

Plug-ins encapsulate custom business logic. This logic can be used to restrict, enrich, validate data within the Dataverse. Plug-ins are created, packaged and deployed to Dataverse environments. Once deployed, they can be configured to work with virtually any event or interaction a user or even a program performs.

### Plug-In Execution Context

Interactions with the Dataverse include multiple stages, most of which can be extended using Plug-ins. These stages represent the logical execution path of an interaction. When a user want to read data stored within Dataverse, the interaction, or request and response transaction, back to the user are customizable. 

 An example could be accessing an account's information within Dataverse. When a user asks Dataverse, the request goes to the data source and returns the account information. During this trip, developers can add in business logic to validate the user has the appropriate security, format data, call external sources (e.g. post service to provide up to date address), etc. [More information about the execution context can be found here](https://docs.microsoft.com/en-us/powerapps/developer/data-platform/understand-the-data-context#:~:text=%20Understand%20the%20execution%20context%20%201%20For,any%20operation%20that%20triggers%20the%20plug-in...%20More%20).

<insert execution context>

## How to Implement Tracing

Plug-ins provide the ability to insert custom business logic but there is a catch...they must run in a sandbox or partial trust environment. This help protect malicious code from accessing or manipulating settings, services, etc on the backend infrastructure. With that said, developers can not simply bring a logging mechanism along but they can implement an interface to help surface tracing. These interfaces come in two distinct forms: one allowing for telemetry that can be captured and used in Azure Application Insights and one surfacing tracing to the Dataverse platform.

### ITracingService

The [ITracingService](https://docs.microsoft.com/en-us/dotnet/api/microsoft.xrm.sdk.itracingservice?view=dynamics-general-ce-9) provides the ability to log run-time trace data within plug-ins. The service is derived from the service provider passed into every plug-in. Once the service has been implemented, developers can simply use the **[Trace](https://docs.microsoft.com/en-us/dotnet/api/microsoft.xrm.sdk.itracingservice.trace?view=dynamics-general-ce-9#Microsoft_Xrm_Sdk_ITracingService_Trace_System_String_System_Object___)** method to write and extract logs.

The implementation is as shown below:

```
//Extract the tracing service for use in debugging sandboxed plug-ins. 
ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
```

Once the *tracingService* object above has been instantiated, developers can leverage the Trace method referenced above. I would recommend using the service early and often within business logic. For instance when other services or objects are first created or sub routines are entered or exited. Helper methods can be used to concatenate time stamps or other supplemental information. This can help debug issues dealing with performance for example.

```
public void Trace(ITracingService tracingService, string message){
	tracingService.Trace(message + " " + DateTime.Now.ToString());
}


```

Trace data is persisted for 24 hours, however if its needed to keep this data for a longer duration or to run a trend analysis, the ILogger interface can be used in conjuction.

### ILogger

Another option available to developers is the ILogger interface as detailed by [Jim Daly](https://github.com/JimDaly) in his article "[Log plug-in telemetry to your Application Insights resource](https://powerapps.microsoft.com/en-us/blog/log-plug-in-telemetry-to-your-application-insights-resource/)". "*When you opt-in to the Application Insights integration described in the earlier [Announcing Monitor model-driven apps using Application Insights (Preview)](https://powerapps.microsoft.com/en-us/blog/announcing-monitor-model-driven-apps-using-application-insights-preview/) post, you can also use the new*
*[Microsoft.Xrm.Sdk.PluginTelemetry.ILogger Interface](https://docs.microsoft.com/en-us/dotnet/api/microsoft.xrm.sdk.plugintelemetry.ilogger?view=dynamics-general-ce-9) in your plug-in code.*"

The below code shows how to implement the ILogger interface:

```
ILogger logger = (ILogger)serviceProvider.GetService(typeof(ILogger));
```



## How to Turn on Tracing

ddd



## How to Explain the Trace Data

dd

## Additional Content and Source Code

ddd

## Next Steps

In this article we have covered getting started with the **Power Automate Management and Admin Connectors**. Each offers unique actions to help administrators report and taken action on environment flow. Continue exploring the capabilities of these connectors and actions through usage within **Power Automate Flows** or **Canvas Apps**. 

We have also covered the what activities are currently captured for **Power Automate**. Discussed in [**Part 2**](https://community.dynamics.com/crm/b/crminthefield/posts/monitoring-the-power-platform-power-automate---auditing-and-activity-logs-part-2) are techniques to view the audit logs within the **Office 365 Security and Compliance Portal** as well as **automated techniques**. 

This article is designed to supplement the article on **Power Automate Analytics**, which provides more of an all up view of **Power Automate** usage. Combining these two documents, an administrator can now <u>track analytic metrics as well as the events that define those metrics</u>.

If you are interested in learning more about [**specialized guidance and training** for monitoring or other areas of the Power Platform](https://community.dynamics.com/crm/b/crminthefield/posts/pfe-dynamics-365-service-offerings), which includes a [monitoring workshop](https://community.dynamics.com/cfs-file/__key/communityserver-blogs-components-weblogfiles/00-00-00-17-38/WorkshopPLUS-_2D00_-Dynamics-365-Customer-Engagement-Monitoring-with-Application-lnsights-1-Day-with-Lab_2D00_FA5D599F_2D00_20E4_2D00_4087_2D00_A713_2D00_39FBD14DF7E5.pdf), please contact your **Technical Account Manager** or **Microsoft representative** for further details. 

Your feedback is **<u>extremely valuable</u>** so please leave a comment below and I'll be happy to help where I can! Also, if you find any inconsistencies, omissions or have suggestions, [please go here to submit a new issue](https://github.com/aliyoussefi/MonitoringPowerPlatform/issues).

## Index

[Monitoring the Power Platform: Introduction and Index](https://community.dynamics.com/crm/b/crminthefield/posts/monitoring-the-power-platform-introduction)