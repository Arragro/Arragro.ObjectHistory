using Microsoft.Azure.Cosmos.Table;

namespace Arragro.ObjectHistory.Core.Models
{
    public class PagingToken
    {
        public TableContinuationToken TableContinuationToken { get; set; }
        public int Page { get; set; } = 1;
        public int? NextPage { get; set; } = null;
        public int PageSize { get; set; } = 10;

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
