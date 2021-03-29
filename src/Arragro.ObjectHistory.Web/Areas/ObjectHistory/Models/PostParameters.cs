using Microsoft.Azure.Cosmos.Table;

namespace Arragro.ObjectHistory.Web.Areas.ObjectHistory.Models
{
    public class ObjectLogsPostParameters
    {
        public string PartitionKey { get; set; } = null;
        public TableContinuationToken TableContinuationToken { get; set; } = null;
    }
}
