using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Linq;

namespace Arragro.ObjectHistory.Core.Models
{
    public class ObjectHistoryQueryResultContainer
    {
        public IEnumerable<ObjectHistoryQueryResult> Results { get; set; }
        public TableContinuationToken ContinuationToken { get; set; }


        public ObjectHistoryQueryResultContainer(
            IEnumerable<ObjectHistoryEntity> entities,
            TableContinuationToken continuationToken)
        {
            ContinuationToken = continuationToken;
            //fixt
            Results = entities.Select(x => new ObjectHistoryQueryResult(x));
        }
    }
}
