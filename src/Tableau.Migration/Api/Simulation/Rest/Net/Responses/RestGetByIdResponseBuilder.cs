using System;
using System.Collections.Generic;
using System.Net.Http;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Net;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    internal class RestGetByIdResponseBuilder<TResponse, TResponseItem> : RestEntityIdResponseBuilderBase<TResponse, TResponseItem>
        where TResponse : TableauServerResponse, ITableauServerResponse<TResponseItem>, new()
        where TResponseItem : IRestIdentifiable
    {
        public RestGetByIdResponseBuilder(
            TableauData data,
            IHttpContentSerializer serializer,
            Func<TableauData, HttpRequestMessage, ICollection<TResponseItem>> getData,
            bool requiresAuthentication)
            : base(data, serializer, getData, requiresAuthentication)
        { }
    }
}
