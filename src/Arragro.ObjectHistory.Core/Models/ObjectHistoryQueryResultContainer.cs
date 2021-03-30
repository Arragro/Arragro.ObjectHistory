using System.Collections.Generic;
using System.Linq;

namespace Arragro.ObjectHistory.Core.Models
{
    public class ObjectHistoryQueryResultContainer
    {
        public string PartitionKey { get; set; }
        public IEnumerable<ObjectHistoryQueryResult> Results { get; set; }
        public PagingToken PagingToken { get; set; }

        public ObjectHistoryQueryResultContainer(
            IEnumerable<ObjectHistoryEntity> entities,
            PagingToken pagingToken,
            string partitionKey)
        {
            PagingToken = pagingToken;
            PartitionKey = partitionKey;
            //fixt
            Results = entities.Select(entity => new ObjectHistoryQueryResult(entity));
        }

        public ObjectHistoryQueryResultContainer(
            IEnumerable<ObjectHistoryGlobalEntity> entities,
            PagingToken pagingToken,
            string partitionKey)
        {
            PagingToken = pagingToken;
            PartitionKey = partitionKey;
            //fixt
            Results = entities.Select(entity => new ObjectHistoryQueryResult(entity));
        }

        public ObjectHistoryQueryResultContainer(
            IEnumerable<ObjectHistoryDeletedEntity> entities,
            PagingToken pagingToken)
        {
            PagingToken = pagingToken;
            //fixt
            Results = entities.Select(entity => new ObjectHistoryQueryResult(entity));
        }
    }
}
