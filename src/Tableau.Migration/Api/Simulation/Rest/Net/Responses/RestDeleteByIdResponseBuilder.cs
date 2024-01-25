using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Net;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    internal class RestDeleteByIdResponseBuilder<TResponse, TResponseItem> : RestEntityIdResponseBuilderBase<TResponse, TResponseItem>
        where TResponse : TableauServerResponse, ITableauServerResponse<TResponseItem>, new()
        where TResponseItem : IRestIdentifiable
    {
        public RestDeleteByIdResponseBuilder(
            TableauData data,
            IHttpContentSerializer serializer,
            Func<TableauData, HttpRequestMessage, ICollection<TResponseItem>> deleteData,
            bool requiresAuthentication)
            : base(data, serializer, deleteData, requiresAuthentication)
        { }

        protected override TResponseItem DoServerWork(HttpRequestMessage request, TResponseItem foundEntity, ICollection<TResponseItem> allEntities)
        {
            allEntities.Remove(foundEntity);
            return foundEntity;
        }

        protected override (TResponse Response, HttpStatusCode ResponseCode) BuildEntityResponse(TResponseItem entity)
        {
            return (new TResponse(), HttpStatusCode.NoContent);
        }
    }
}
