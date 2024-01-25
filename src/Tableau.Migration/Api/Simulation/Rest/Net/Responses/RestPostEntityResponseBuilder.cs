using System;
using System.Collections.Generic;
using System.Net.Http;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Net;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    internal class RestPostEntityResponseBuilder<TResponse, TResponseItem> : RestEntityIdResponseBuilderBase<TResponse, TResponseItem>
        where TResponse : TableauServerResponse, ITableauServerResponse<TResponseItem>, new()
        where TResponseItem : IRestIdentifiable
    {
        public RestPostEntityResponseBuilder(
            TableauData data,
            IHttpContentSerializer serializer,
            Func<TableauData, HttpRequestMessage, ICollection<TResponseItem>> postData,
            bool requiresAuthentication)
            : base(data, serializer, postData, requiresAuthentication)
        { }

        protected override TResponseItem DoServerWork(HttpRequestMessage request, TResponseItem foundEntity, ICollection<TResponseItem> allEntities)
        {
            //TODO: get entity from request, add to entity list.
            return foundEntity;
        }
    }
}
