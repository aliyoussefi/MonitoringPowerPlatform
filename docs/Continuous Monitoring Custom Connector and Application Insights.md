# Continuous Monitoring: Custom Connector and Application Insights

## Contents

1. Summary
2. Overview of Open API Definition
3. Overview of Application Insights
4. Overview of Custom Connectors
5. Building a Custom Connector
   1. Open API Spec
   2. Postman
6. Securing a Custom Connector
7. Testing a Custom Connector
8. Deploying a Custom Connector
9. Using a Custom Connector
10. Conclusion

## Summary

The goal of this article is to describe how to implement a monitoring strategy for both Power Automate and Power Apps. The goal here is to show how a custom connector can be built to deliver custom events and messages to an Application Insights resource. While this article focuses on a connector for Application Insights the concepts can be applied to virtually any readially consumable API. 

## Overview of Custom Connectors

Custom Connectors allow developers to supply custom actions and triggers that can be used by Microsoft Power Automate and Microsoft Power Apps. These connectors provide a reusable no or low code approach to integrating with an Application Programmable Interface otherwise known as an API. The complexity of interacting and implementing a connection and call to the API is hidden from makers, allowing them to focus on finding a solution to whatever business objective is at hand.

The custom connector can be thought of as a solution to a no cliffs approach to empowering makers. If a connector doesn't exist for your particular need, for instance an in house API, a custom connector can be used to bridge the gap. No longer are we having to build and send HTTP requests or manage connection flows as the connector will fill the gap.

## Overview of the Open API Specification

Open API is a specification built on the desire and need to standarize how we describe API endpoints. Built from Swagger, the Open API dictates how an API should be used. Everything from security to required fields are detailed allowing integrators to focus on developing and not chasing down API specs.

This specification is used by Power Platform Custom Connectors to build out the various triggers and actions that will be made available. This will be covered in more detail in the Building a Custom Connector section.

For more information regarding Open API please refer to this reference.

## Overview of Application Insights

Application Insights is an extensible Application Performance Management (APM) service that can be used to monitor applications, tests, etc. Source: [Application Insights Overview](https://docs.microsoft.com/en-us/azure/azure-monitor/app/app-insights-overview) Debugging any application is helped by instrumentation and logging and Application Insights provides a mechanism to centralize log storage as well as interoperability across all platforms.

Application Insights can be configured natively across many Azure resources as well as some Power Platform modules such as Canvas Apps. However, as with Azure resources, while having the configuration available it is up to the maker or developer to determine how best to utilize Application Insights. Determining when and where to use Application Insights is out of scope here but we will explore how to provide an easy to use capability for makers.

For help on setting up refer to [this article](https://docs.microsoft.com/en-us/azure/azure-monitor/learn/nodejs-quick-start) detailing enabling and configuring Application Insights.

## Building a Custom Connector

As described above in the Overview of Open API Specification section, custom connectors can be built using predefined definitions. That said, they can also be built with no definition as well as utilizing industry standard tools that work with APIs such as Postman.

### Building from an Open API Specification

If an Open API specification schema is available for use I would recommend going this route as this will help build out essentially everything that can be used by an API. This spec can be loaded from a json file or from an URL and imported as a custom connector definition. If this API is specific to your organization, for instance a custom home grown API, consider collaborating with the owners of the API for this definition. Tools such as Swagger IO can help build out this Open API spec file.

Alternatively, as is the case with Application Insights, I'd suggest searching online as many public APIs expose a definition that can be consumed via URL.

### Building from Postman

Postman is a tool that allows for developing and testing of service APIs as well as other features. For use with the custom connector, we will want to build a definition or capture web traffic which can be used by the Custom Connector build wizard. Capturing web traffic will require using a proxy or a browser plugin to route traffic to Postman.

## Securing a Custom Connector

### Choosing a security protocol

OAuth, Box, IdP, etc.



### The Power Automate PowerShell Cmdlets

**<u>Get-FlowEnvironment</u>** - This allows an administrator to get an environment or list of environments based on specified criteria such as the default environment, environment name or a filter. This will return the environment name, the display name and other properties.

**<u>Get-AdminFlow</u>** - Using the environment an administrator can search for existing flows. This will prove helpful to get the flow name and definition of the flow and other properties.

![](C:\Users\alyousse\AppData\Roaming\Typora\typora-user-images\image-20191216161314775.png)

**<u>Get-FlowRun</u>** - This can show the individual run of a specific flow including the status, the start and end times, a correlationId and trigger information.

![](C:\Users\alyousse\Desktop\PowerAutomate\PowerAutomate-PowerShell-Get-Flow.JPG)

### The Power Automate Web API

https://docs.microsoft.com/en-us/power-automate/web-api#list-flows

This allows an administrator to list flows, update flows, see what flows are shared with others and import and export flows with solutions. Use this as you would the Dynamics 365 API meaning utilize the security model for authentication and authorization and standard operations for CRUD and Import/Export actions.

## Adding Application Performance Monitoring to Power Automate

### Adding a custom Correlation Id

<img src="C:\Users\alyousse\Desktop\PowerAutomate\PowerAutomate-Triggers-HTTP.JPG" alt="PowerAutomate-Triggers-HTTP" style="zoom:50%;" />

Custom Tracking Id

### Utilizing a system provided Correlation Id

### Sending an event to Application Insights

