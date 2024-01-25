using System;
using System.Collections.Generic;
using System.Net.Http;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Net;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    internal class RestPutByIdResponseBuilder<TResponse, TResponseItem> : RestEntityIdResponseBuilderBase<TResponse, TResponseItem>
        where TResponse : TableauServerResponse, ITableauServerResponse<TResponseItem>, new()
        where TResponseItem : IRestIdentifiable
    {
        public RestPutByIdResponseBuilder(
            TableauData data,
            IHttpContentSerializer serializer,
            Func<TableauData, HttpRequestMessage, ICollection<TResponseItem>> putData,
            bool requiresAuthentication)
            : base(data, serializer, putData, requiresAuthentication)
        { }

        protected override TResponseItem DoServerWork(HttpRequestMessage request, TResponseItem foundEntity, ICollection<TResponseItem> allEntities)
        {
            allEntities.Remove(foundEntity);
            //TODO: get entity from request, update entity.
            allEntities.Add(foundEntity);
            return foundEntity;
        }
    }
}
