using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Net;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    internal class RestDeleteResponseBuilder : EmptyRestResponseBuilder
    {
        private readonly Func<TableauData, HttpRequestMessage, HttpStatusCode> _delete;

        public RestDeleteResponseBuilder(
            TableauData data,
            Func<TableauData, HttpRequestMessage, HttpStatusCode> delete,
            IHttpContentSerializer serializer)
            : base(data, serializer, requiresAuthentication: true)
        {
            _delete = delete;
        }

        protected override Task<HttpResponseMessage> BuildResponseAsync(
            HttpRequestMessage request,
            CancellationToken cancel)
            => Task.FromResult(new HttpResponseMessage(_delete(Data, request)));
    }
}
