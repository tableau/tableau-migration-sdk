using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Simulation.Rest.Net.Requests;
using Tableau.Migration.Net;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    internal class RestDownloadFileByIdResponseBuilder : EmptyRestResponseBuilder
    {
        private readonly Func<TableauData, IReadOnlyDictionary<Guid, byte[]>> _getFilesById;
        private readonly int _idNotFoundSubCode;

        public RestDownloadFileByIdResponseBuilder(
            TableauData data,
            IHttpContentSerializer serializer,
            Func<TableauData, IReadOnlyDictionary<Guid, byte[]>> getFilesById,
            int idNotFoundSubCode,
            bool requiresAuthentication)
            : base(data, serializer, requiresAuthentication)
        {
            _getFilesById = getFilesById;
            _idNotFoundSubCode = idNotFoundSubCode;
        }

        protected override Task<HttpResponseMessage> BuildResponseAsync(HttpRequestMessage request, CancellationToken cancel)
        {
            var filesById = _getFilesById(Data);

            var entityId = request.GetRequestIdFromUri(hasSuffix: true);
            if (!filesById.TryGetValue(entityId, out var file))
            {
                var errorBuilder = new StaticRestErrorBuilder(HttpStatusCode.NotFound, _idNotFoundSubCode, string.Empty, string.Empty);
                return Task.FromResult(BuildErrorResponse(request, errorBuilder));
            }

            return Task.FromResult(new HttpResponseMessage
            {
                Content = new ByteArrayContent(file)
            });
        }
    }
}