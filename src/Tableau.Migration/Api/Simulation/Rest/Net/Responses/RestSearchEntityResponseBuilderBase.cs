using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Net;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    /// <summary>
    /// Abstract base class for REST API style response builders that operate on a single entity that is found from the list of entities.
    /// </summary>
    internal abstract class RestSearchEntityResponseBuilderBase<TResponse, TResponseItem> : RestEntityListResponseBuilderBase<TResponse, TResponseItem>
        where TResponse : TableauServerResponse, ITableauServerResponse<TResponseItem>, new()
    {
        protected RestSearchEntityResponseBuilderBase(
            TableauData data,
            IHttpContentSerializer serializer,
            Func<TableauData, HttpRequestMessage, ICollection<TResponseItem>> getEntities,
            bool requiresAuthentication)
            : base(data, serializer, getEntities, requiresAuthentication)
        { }

        protected abstract TResponseItem? FindEntity(ICollection<TResponseItem> entities, HttpRequestMessage request);

        protected virtual TResponseItem DoServerWork(HttpRequestMessage request, TResponseItem foundEntity, ICollection<TResponseItem> allEntities)
            => foundEntity;

        protected virtual HttpStatusCode SuccessStatusCode { get; } = HttpStatusCode.OK;

        protected virtual (TResponse Response, HttpStatusCode ResponseCode) BuildEntityResponse(TResponseItem entity)
        {
            var response = new TResponse()
            {
                Item = entity
            };

            return (response, SuccessStatusCode);
        }

        protected override ValueTask<(TResponse Response, HttpStatusCode ResponseCode)> BuildResponseAsync(HttpRequestMessage request, CancellationToken cancel)
        {
            var filteredEntities = GetEntities(Data, request);

            var foundEntity = FindEntity(filteredEntities, request);

            if (foundEntity is null)
            {
                return BuildEmptyErrorResponseAsync(HttpStatusCode.NotFound, 0, "Not Found Summary", "Not Found Detail");
            }

            var responseItem = DoServerWork(request, foundEntity, filteredEntities);
            return ValueTask.FromResult(BuildEntityResponse(responseItem));
        }
    }
}
