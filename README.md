## Arragro Object History

This solution provides the ability to track object diff's.  It utilizes [JsonDiffPatch.Net](https://github.com/wbish/jsondiffpatch.net) to build the differences on the server and [jsondiffpatch](https://github.com/benjamine/jsondiffpatch) on the client for presentation.

There are several moving parts, with the idea of offloading as much as possible to an Azure Function for the bulk of the heavy lifting:

1. Arragro.ObjectHistory.Core core models and helpers.
2. Arragro.ObjectHistory.Client is used to allow your code to add history after you have persisted your data, it also provides read functions for retrieving the data.
   i. This history is a raw dump into a blob and a queue message.
3. Arragro.ObjectHistory.AzureFunctions Then picks up the queue and blob and processes the history to Table storage and the diff to another container.
4. Arragro.ObjectHistory.Web provides razor pages and a controller for you to integrate the front end.
5. @arragro/object-history-web is an npm package with a react client to interact with Arragro.ObjectHistory.Web and present the history.

Within the solution Arragro.ObjectHistory.WebExample provides a working example that allows you to work with as a demo.  This project is also used to generate the npm for the react spa app.

To configure this within your web project you will need to do the following:

1. Install the nuget package Arragro.ObjectHistory.Web
2. Install the npm package @arragro/object-history-web
3. [Configure MVC App](#Configure-MVC-App)


### Configure Mvc App
To configure the MVC app you need to setup a security policy and wire up a ActionFilterAttribute.  The security policy is called **ArragroObjectHistoryPolicy**.  In the example we use the following:

#### Security Policy
``` csharp
services.AddAuthorization(options =>
{
    options.AddPolicy("ArragroObjectHistoryPolicy", policy => policy.RequireAssertion(context => true));
});
```

As there is no identity or authentication to validate against.  If using Asp.Net Identity, then you can ensure an administrator can have access:

``` csharp
services.AddAuthorization(options =>
{
    options.AddPolicy("ArragroObjectHistoryPolicy", policy => 
    {
        policy.RequireClaim(ClaimTypes.Role, "Administrator");
    }
});
```
This policy is required and you will get an error if you do not add it.