using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Linq;

namespace Arragro.ObjectHistory.Core.Models
{
    public class ObjectHistoryQueryResultContainer
    {
        public string PartitionKey { get; set; }
        public IEnumerable<ObjectHistoryQueryResult> Results { get; set; }
        public TableContinuationToken ContinuationToken { get; set; }

        public ObjectHistoryQueryResultContainer(
            IEnumerable<ObjectHistoryEntity> entities,
            TableContinuationToken continuationToken,
            string partitionKey)
        {
            ContinuationToken = continuationToken;
            PartitionKey = partitionKey;
            //fixt
            Results = entities.Select(entity => new ObjectHistoryQueryResult(partitionKey, entity));
        }

        public ObjectHistoryQueryResultContainer(
            IEnumerable<ObjectHistoryGlobalEntity> entities,
            TableContinuationToken continuationToken,
            string partitionKey)
        {
            ContinuationToken = continuationToken;
            PartitionKey = partitionKey;
            //fixt
            Results = entities.Select(entity => new ObjectHistoryQueryResult(entity));
        }
    }
}
