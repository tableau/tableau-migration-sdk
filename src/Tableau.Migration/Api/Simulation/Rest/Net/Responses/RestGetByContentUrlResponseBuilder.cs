using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Simulation.Rest.Net.Requests;
using Tableau.Migration.Net;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    internal class RestGetByContentUrlResponseBuilder<TResponse, TResponseItem> : RestSearchEntityResponseBuilderBase<TResponse, TResponseItem>
        where TResponse : TableauServerResponse, ITableauServerResponse<TResponseItem>, new()
        where TResponseItem : IApiContentUrl
    {
        public RestGetByContentUrlResponseBuilder(
            TableauData data,
            IHttpContentSerializer serializer,
            Func<TableauData, HttpRequestMessage, ICollection<TResponseItem>> getData,
            bool requiresAuthentication)
            : base(data, serializer, getData, requiresAuthentication)
        { }

        protected override TResponseItem? FindEntity(ICollection<TResponseItem> entities, HttpRequestMessage request)
        {
            var contentUrl = request.GetLastSegment();
            return entities.FirstOrDefault(e => String.Equals(e.ContentUrl, contentUrl, StringComparison.Ordinal));
        }
    }
}
