using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Net;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    internal sealed class RestPagedListResponseBuilder<TResponse, TResponseItem> : RestEntityListResponseBuilderBase<TResponse, TResponseItem>
        where TResponse : PagedTableauServerResponse<TResponseItem>, new()
    {
        private readonly IEntityResponsePager<TResponseItem> _pager;

        public RestPagedListResponseBuilder(
            TableauData data,
            IHttpContentSerializer serializer,
            Func<TableauData, HttpRequestMessage, ICollection<TResponseItem>> getEntities,
            bool requiresAuthentication)
            : base(data, serializer, getEntities, requiresAuthentication)
        {
            _pager = new RestEntityResponsePager<TResponseItem>();
        }

        protected override ValueTask<(TResponse Response, HttpStatusCode ResponseCode)> BuildResponseAsync(HttpRequestMessage request, CancellationToken cancel)
        {
            var filteredData = GetEntities(Data, request);
            var pageOptions = _pager.GetPageOptions(request);

            var response = new TResponse();

            response.Pagination.PageNumber = pageOptions.PageNumber;
            response.Pagination.PageSize = pageOptions.PageSize;
            response.Pagination.TotalAvailable = filteredData.Count;

            var page = _pager.GetPage(filteredData, pageOptions);
            response.Items = page.ToArray();

            return ValueTask.FromResult((response, HttpStatusCode.OK));
        }
    }
}
