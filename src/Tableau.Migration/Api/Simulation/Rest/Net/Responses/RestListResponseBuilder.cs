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
    internal sealed class RestListResponseBuilder<TResponse, TResponseItem> : RestEntityListResponseBuilderBase<TResponse, TResponseItem>
        where TResponse : TableauServerListResponse<TResponseItem>, new()
    {
        public RestListResponseBuilder(
            TableauData data,
            IHttpContentSerializer serializer,
            Func<TableauData, HttpRequestMessage, ICollection<TResponseItem>> getEntities,
            bool requiresAuthentication)
            : base(data, serializer, getEntities, requiresAuthentication)
        { }

        protected override ValueTask<(TResponse Response, HttpStatusCode ResponseCode)> BuildResponseAsync(HttpRequestMessage request, CancellationToken cancel)
        {
            var response = new TResponse
            {
                Items = GetEntities(Data, request).ToArray()
            };

            return ValueTask.FromResult((response, HttpStatusCode.OK));
        }
    }
}
