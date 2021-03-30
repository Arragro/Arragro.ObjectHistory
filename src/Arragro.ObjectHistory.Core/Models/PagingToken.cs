using Microsoft.Azure.Cosmos.Table;

namespace Arragro.ObjectHistory.Core.Models
{
    public class PagingToken
    {
        public TableContinuationToken TableContinuationToken { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }

        public PagingToken()
        {
        }

        public PagingToken(TableContinuationToken tableContinuationToken)
        {
            TableContinuationToken = tableContinuationToken;
        }

        public PagingToken(int page, int pageSize)
        {
            Page = page;
            PageSize = pageSize;
        }
    }
}
