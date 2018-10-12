using Arragro.ObjectHistory.Client;
using Arragro.ObjectHistory.Core;
using Arragro.ObjectHistory.Web.Areas.ObjectHistory.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;

namespace Arragro.ObjectHistory.Web.Areas.ObjectHistory.Controllers
{
    [Authorize(Policy = "ArragroObjectHistoryPolicy")]
    [Area("ObjectHistory")]
    [IgnoreAntiforgeryToken]
    [Route("arragro-object-history")]
    public class HomeController : Controller
    {
        private readonly ObjectHistoryClient _objectHistoryClient;

        public HomeController(ObjectHistoryClient objectHistoryClient)
        {
            _objectHistoryClient = objectHistoryClient;
        }

        [HttpGet("{id?}")]
        public IActionResult Index(string id)
        {
            return View("Index");
        }

        [HttpPost("get-global-logs")]
        [Authorize(Policy = "ArragroObjectHistoryGlobalLogPolicy")]
        public async Task<IActionResult> GetGlobalLogs([FromBody] TableContinuationToken tableContinuationToken = null)
        {
            var entities = await _objectHistoryClient.GetObjectHistoryRecordsByApplicationNamePartitionKeyAsync(tableContinuationToken);

            return Ok(entities);
        }

        [HttpPost("get-object-logs")]
        [ServiceFilter(typeof(IObjectLogsSecurityAttribute))]
        public async Task<IActionResult> GetObjectLogs([FromBody] ObjectLogsPostParameters postParameters)
        {
            var entities = await _objectHistoryClient.GetObjectHistoryRecordsByObjectNamePartitionKeyAsync(postParameters.PartitionKey, postParameters.TableContinuationToken);

            return Ok(entities);
        }

        [HttpGet("get-object-log/{partitionKey}/{rowKey}")]
        [ServiceFilter(typeof(IObjectLogsSecurityAttribute))]
        public async Task<IActionResult> GetLog(string partitionKey, string rowKey)
        {
            var entities = await _objectHistoryClient.GetObjectHistoryDetailRawAsync(partitionKey, rowKey);

            return Ok(entities);
        }

        //[HttpGet("download-log-file")]
        //public async Task<IActionResult> Download(string folder)
        //{
        //    if (folder != null)
        //    {
        //        var file = await _objectHistoryClient.GetObjectHistoryFile(folder);
        //        return Ok(file);
        //    }

        //    return Ok();
        //}
    }
}
