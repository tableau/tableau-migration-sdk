using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Net;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    internal class RestSingleEntityResponseBuilder<TResponse, TResponseItem> : RestApiResponseBuilderBase<TResponse>
        where TResponse : TableauServerResponse, ITableauServerResponse<TResponseItem>, new()
    {
        private readonly Func<TableauData, HttpRequestMessage, TResponseItem?> _getEntity;

        public RestSingleEntityResponseBuilder(
            TableauData data,
            IHttpContentSerializer serializer,
            Func<TableauData, HttpRequestMessage, TResponseItem?> getEntity,
            bool requiresAuthentication)
            : base(data, serializer, requiresAuthentication)
        {
            _getEntity = getEntity;
        }

        protected virtual void ChangeEntity(TResponseItem entity)
        { }

        protected virtual (TResponse Response, HttpStatusCode ResponseCode) BuildEntityResponse(TResponseItem entity)
        {
            var response = new TResponse()
            {
                Item = entity
            };

            return (response, HttpStatusCode.OK);
        }

        protected override ValueTask<(TResponse Response, HttpStatusCode ResponseCode)> BuildResponseAsync(HttpRequestMessage request, CancellationToken cancel)
        {
            var entity = _getEntity(Data, request);

            if (entity is null)
            {
                return BuildEmptyErrorResponseAsync(HttpStatusCode.NotFound, 0, "Not Found Summary", "Not Found Detail");
            }
            else
            {
                ChangeEntity(entity);
                return ValueTask.FromResult(BuildEntityResponse(entity));
            }
        }
    }
}
