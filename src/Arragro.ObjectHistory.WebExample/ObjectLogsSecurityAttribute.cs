using Arragro.ObjectHistory.Client;
using Arragro.ObjectHistory.Core;
using Arragro.ObjectHistory.Core.Models;
using Arragro.ObjectHistory.Web.Areas.ObjectHistory.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Arragro.ObjectHistory.WebExample
{
    public class ObjectLogsSecurityAttribute : ActionFilterAttribute, IObjectLogsSecurityAttribute
    {
        private readonly IObjectHistoryClient _objectHistoryClient;

        public ObjectLogsSecurityAttribute(
            IObjectHistoryClient objectHistoryClient)
        {
            _objectHistoryClient = objectHistoryClient;
        }

        private bool TestSecurityValidationToken(ActionExecutingContext context, ObjectHistoryDetailRaw objectHistoryDetailRaw)
        {
            // Some test logic goes here against record.SecurityValidationToken
            if (objectHistoryDetailRaw != null &&
                objectHistoryDetailRaw.SecurityValidationToken != null)
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                context.HttpContext.Response.Headers.Clear();

                context.Result = new EmptyResult();
                return false;
            }
            return true;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            base.OnActionExecuting(context);
            var type = typeof(ObjectLogsPostParameters);

            if (context.ActionArguments.ContainsKey("partitionKey") &&
                context.ActionArguments.ContainsKey("rowKey"))
            {
                var partitionKey = context.ActionArguments["partitionKey"].ToString();
                var rowKey = context.ActionArguments["rowKey"].ToString();
                var objectHistoryDetailRaw = await _objectHistoryClient.GetObjectHistoryDetailRawAsync(partitionKey, rowKey);
                if (!TestSecurityValidationToken(context, objectHistoryDetailRaw))
                    return;
            }
            else
            {
                foreach (var key in context.ActionArguments.Keys)
                {
                    if (type.IsAssignableFrom(context.ActionArguments[key].GetType()))
                    {
                        var partitionKey = (context.ActionArguments[key] as ObjectLogsPostParameters).PartitionKey;

                        try
                        {
                            var objectHistoryDetailRaw = await _objectHistoryClient.GetObjectHistoryDetailRawAsync(partitionKey);
                            if (!TestSecurityValidationToken(context, objectHistoryDetailRaw))
                                return;
                        }
                        catch (Exception ex)
                        {
                            return; 
                        }
                    }
                }
            }

            await next();
        }
    }
}
