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
    [Route("arragro-object-history")]
    public class HomeController : Controller
    {
        private readonly ObjectHistoryClient _objectHistoryClient;
        private readonly IOptions<ObjectHistorySettings> _objectHistorySettings;

        public HomeController(ObjectHistoryClient objectHistoryClient, IOptions<ObjectHistorySettings> myAppSettings)
        {
            _objectHistoryClient = objectHistoryClient;
            _objectHistorySettings = myAppSettings;
        }

        public IActionResult Index()
        {
            return View("IndexKnockout");
        }

        [HttpPost("get-global-logs")]
        public async Task<IActionResult> GetGlobalLogs(TableContinuationToken tableContinuationToken = null)
        {

            var entities = await _objectHistoryClient.GetGlobalObjectHistoryAsync(_objectHistorySettings.Value.ApplicationName, tableContinuationToken);

            return Ok(entities);
        }

        [HttpPost("get-object-logs")]
        public async Task<IActionResult> GetObjectLogs([FromBody] ObjectLogsPostParameters postParameters)
        {

            var entities = await _objectHistoryClient.GetObjectHistoryAsync(postParameters.PartitionKey, postParameters.TableContinuationToken);

            return Ok(entities);
        }

        [HttpPost("get-object-log")]
        public async Task<IActionResult> GetLog(ObjectLogsPostParameters postParameters)
        {
            var entities = await _objectHistoryClient.GetObjectHistoryDetailRaw(postParameters.PartitionKey);

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
