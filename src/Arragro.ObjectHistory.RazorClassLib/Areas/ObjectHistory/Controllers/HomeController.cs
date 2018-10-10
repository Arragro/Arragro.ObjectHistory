using Arragro.ObjectHistory.Client;
using Arragro.ObjectHistory.Core.Models;
using Arragro.ObjectHistory.RazorClassLib.Areas.ObjectHistory.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Arragro.ObjectHistory.RazorClassLib.Areas.ObjectHistory.Controllers
{    
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
            // return View("IndexKnockout");
        }

        [HttpPost("get-global-logs")]
        public async Task<IActionResult> GetGlobalLogs([FromBody] TableContinuationToken tableContinuationToken = null)
        {
            var entities = await _objectHistoryClient.GetObjectHistoryRecordsByApplicationNamePartitionKey(tableContinuationToken);

            return Ok(entities);
        }

        [HttpPost("get-object-logs")]
        public async Task<IActionResult> GetObjectLogs([FromBody] ObjectLogsPostParameters postParameters)
        {
            var entities = await _objectHistoryClient.GetObjectHistoryRecordsByObjectNamePartitionKey(postParameters.PartitionKey, postParameters.TableContinuationToken);

            return Ok(entities);
        }

        [HttpGet("get-object-log/{folder}")]
        public async Task<IActionResult> GetLog(Guid folder)
        {
            var entities = await _objectHistoryClient.GetObjectHistoryDetailRaw(folder);

            return Ok(entities);
        }

        [HttpGet("download-log-file")]
        public async Task<IActionResult> Download(string folder)
        {
            if (folder != null)
            {
                var file = await _objectHistoryClient.GetObjectHistoryFile(folder);
                return Ok(file);
            }

            return Ok();
        }
    }
}
