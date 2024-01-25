using System;
using System.Collections.Generic;
using System.Net.Http;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Net;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    /// <summary>
    /// Abstract base calss for REST API style response builders that operate on a list of entities in some form.
    /// </summary>
    internal abstract class RestEntityListResponseBuilderBase<TResponse, TItem> : RestApiResponseBuilderBase<TResponse>
        where TResponse : TableauServerResponse, new()
    {
        private readonly Func<TableauData, HttpRequestMessage, ICollection<TItem>> _getEntities;

        public RestEntityListResponseBuilderBase(
            TableauData data,
            IHttpContentSerializer serializer,
            Func<TableauData, HttpRequestMessage, ICollection<TItem>> getEntities,
            bool requiresAuthentication)
            : base(data, serializer, requiresAuthentication)
        {
            _getEntities = getEntities;
        }

        protected ICollection<TItem> GetEntities(TableauData data, HttpRequestMessage request)
            => _getEntities(data, request);
    }
}
