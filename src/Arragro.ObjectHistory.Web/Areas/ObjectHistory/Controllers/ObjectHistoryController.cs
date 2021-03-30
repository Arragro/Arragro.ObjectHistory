using Arragro.ObjectHistory.Core;
using Arragro.ObjectHistory.Core.Models;
using Arragro.ObjectHistory.Web.Areas.ObjectHistory.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Arragro.ObjectHistory.Web.Areas.ObjectHistory.Controllers
{
    [Authorize(Policy = "ArragroObjectHistoryPolicy")]
    [Area("ObjectHistory")]
    [IgnoreAntiforgeryToken]
    [Route("arragro-object-history")]
    public class ObjectHistoryController : Controller
    {
        private readonly IObjectHistoryClient _objectHistoryClient;

        public ObjectHistoryController(IObjectHistoryClient objectHistoryClient)
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
        public async Task<IActionResult> GetGlobalLogs([FromBody] PagingToken pagingToken = null)
        {
            var entities = await _objectHistoryClient.GetObjectHistoryRecordsByApplicationNamePartitionKeyAsync(pagingToken);

            return Ok(entities);
        }

        [HttpPost("get-object-logs")]
        [ServiceFilter(typeof(IObjectLogsSecurityAttribute))]
        public async Task<IActionResult> GetObjectLogs([FromBody] ObjectLogsPostParameters postParameters)
        {
            var entities = await _objectHistoryClient.GetObjectHistoryRecordsByObjectNamePartitionKeyAsync(postParameters.PartitionKey, postParameters.PagingToken);

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
