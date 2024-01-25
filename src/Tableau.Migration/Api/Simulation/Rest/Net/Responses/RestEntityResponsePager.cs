using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Http;
using System.Web;
using Tableau.Migration.Net.Rest.Paging;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    internal sealed class RestEntityResponsePager<TEntity> : IEntityResponsePager<TEntity>
    {
        private const int _defaultPageNumber = 1; //Pages are one-indexed (boo).
        private const int _defaultPageSize = 100;

        public Page GetPageOptions(HttpRequestMessage request)
        {
            int pageNumber = _defaultPageNumber;
            int pageSize = _defaultPageSize;

            if (request.RequestUri is not null)
            {
                var query = HttpUtility.ParseQueryString(request.RequestUri.Query);

                var pageNumberValue = query[PageBuilder.PageNumberKey];
                if (!string.IsNullOrEmpty(pageNumberValue))
                    pageNumber = int.Parse(pageNumberValue);

                var pageSizeValue = query[PageBuilder.PageSizeKey];
                if (!string.IsNullOrEmpty(pageSizeValue))
                    pageSize = int.Parse(pageSizeValue);
            }

            return new(pageNumber, pageSize);
        }

        public ImmutableArray<TEntity> GetPage(IEnumerable<TEntity> entities, Page pageOptions)
        {
            return entities.Skip((pageOptions.PageNumber - 1) * pageOptions.PageSize) //Pages are one-indexed.
                .Take(pageOptions.PageSize)
                .ToImmutableArray();
        }
    }
}
