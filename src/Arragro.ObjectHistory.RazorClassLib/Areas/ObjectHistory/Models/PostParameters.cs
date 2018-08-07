using Microsoft.WindowsAzure.Storage.Table;

namespace Arragro.ObjectHistory.RazorClassLib.Areas.ObjectHistory.Models
{
    public class ObjectLogsPostParameters
    {
        public string PartitionKey { get; set; } = null;
        public TableContinuationToken TableContinuationToken { get; set; } = null;
    }

    public class PostParameters
    {
        public string Folder { get; set; }
    }
}
