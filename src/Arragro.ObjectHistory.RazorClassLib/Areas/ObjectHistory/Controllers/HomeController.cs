using Arragro.ObjectHistory.Client;
using Arragro.ObjectHistory.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Arragro.ObjectHistory.RazorClassLib.Areas.ObjectHistory.Controllers
{
    public class Temp
    {
        public string PartitionKey { get; set; } = null;
        public TableContinuationToken TableContinuationToken { get; set; } = null;
    }
    
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
            return View();
        }

        [HttpPost("get-object-logs")]
        public async Task<IActionResult> GetFileLogs([FromBody] Temp temp)
        {
            var entities =  await _objectHistoryClient.GetObjectHistoryAsync(temp.PartitionKey, temp.TableContinuationToken);

            return Ok(entities);
        }


        [HttpPost("get-global-logs")]
        public async Task<IActionResult> GetGlobalLogs(TableContinuationToken tableContinuationToken = null)
        {
           
            var entities = await _objectHistoryClient.GetGlobalObjectHistoryAsync(_objectHistorySettings.Value.ApplicationName, tableContinuationToken);

            return Ok(entities);
        }

    }
}
