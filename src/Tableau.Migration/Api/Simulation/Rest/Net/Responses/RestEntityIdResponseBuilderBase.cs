using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Simulation.Rest.Net.Requests;
using Tableau.Migration.Net;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    /// <summary>
    /// Abstract base class for a REST API style response builder that operates on a single entity that is matched by the request ID.
    /// </summary>
    internal abstract class RestEntityIdResponseBuilderBase<TResponse, TResponseItem> : RestSearchEntityResponseBuilderBase<TResponse, TResponseItem>
        where TResponse : TableauServerResponse, ITableauServerResponse<TResponseItem>, new()
        where TResponseItem : IRestIdentifiable
    {
        public RestEntityIdResponseBuilderBase(
            TableauData data,
            IHttpContentSerializer serializer,
            Func<TableauData, HttpRequestMessage, ICollection<TResponseItem>> getData,
            bool requiresAuthentication)
            : base(data, serializer, getData, requiresAuthentication)
        { }

        protected override TResponseItem? FindEntity(ICollection<TResponseItem> entities, HttpRequestMessage request)
        {
            var entityId = request.GetRequestIdFromUri();
            return entities.FirstOrDefault(e => e.Id == entityId);
        }
    }
}
