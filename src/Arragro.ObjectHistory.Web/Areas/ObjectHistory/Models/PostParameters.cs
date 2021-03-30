using Arragro.ObjectHistory.Core.Models;

namespace Arragro.ObjectHistory.Web.Areas.ObjectHistory.Models
{
    public class ObjectLogsPostParameters
    {
        public string PartitionKey { get; set; } = null;
        public PagingToken PagingToken { get; set; } = null;
    }
}
