using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Linq;

namespace Arragro.ObjectHistory.Core.Models
{
    public class ObjectHistoryGlobalQueryResultContainer
    {
        public string PartitionKey { get; set; } 
        public IEnumerable<ObjectHistoryGlobalQueryResult> Results { get; set; }
        public TableContinuationToken ContinuationToken { get; set; }


        public ObjectHistoryGlobalQueryResultContainer(
            IEnumerable<ObjectHistoryGlobalEntity> entities,
            TableContinuationToken continuationToken,
            string partitionKey)
        {
            ContinuationToken = continuationToken;
            PartitionKey = partitionKey;
            //fixt
            Results = entities.Select(x => new ObjectHistoryGlobalQueryResult(x));
        }
    }
}
