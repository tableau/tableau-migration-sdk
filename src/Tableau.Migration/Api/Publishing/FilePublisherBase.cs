﻿using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models.Requests;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Net;
using Tableau.Migration.Net.Rest;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Api.Publishing
{
    internal abstract class FilePublisherBase<TPublishOptions, TPublishRequest, TPublishResult> : IFilePublisher<TPublishOptions, TPublishResult>
        where TPublishOptions : IPublishFileOptions
        where TPublishRequest : TableauServerRequest
        where TPublishResult : class, IContentReference
    {
        private readonly IServerSessionProvider _sessionProvider;
        private readonly IHttpStreamProcessor _httpStreamProcessor;

        protected readonly IRestRequestBuilderFactory RestRequestBuilderFactory;
        protected readonly IContentReferenceFinderFactory ContentFinderFactory;
        protected readonly ISharedResourcesLocalizer SharedResourcesLocalizer;

        protected readonly string ContentTypeUrlPrefix;

        public FilePublisherBase(
            IRestRequestBuilderFactory restRequestBuilderFactory,
            IContentReferenceFinderFactory finderFactory,
            IServerSessionProvider sessionProvider,
            ISharedResourcesLocalizer sharedResourcesLocalizer,
            IHttpStreamProcessor httpStreamProcessor,
            string contentTypeUrlPrefix)
        {
            RestRequestBuilderFactory = restRequestBuilderFactory;
            ContentFinderFactory = finderFactory;
            SharedResourcesLocalizer = sharedResourcesLocalizer;
            ContentTypeUrlPrefix = contentTypeUrlPrefix;

            _sessionProvider = sessionProvider;
            _httpStreamProcessor = httpStreamProcessor;
        }

        protected abstract TPublishRequest BuildCommitRequest(TPublishOptions options);

        /// <summary>
        /// Publishes a stream to a given site and commits the upload. 
        /// </summary>
        /// <param name="options">The publish options context object.</param>
        /// <param name="cancel">The cancellation token</param>
        /// <returns>The specified type object with the completion data.</returns>
        public async Task<IResult<TPublishResult>> PublishAsync(
            TPublishOptions options,
            CancellationToken cancel)
        {
            static IResult<TPublishResult> GetFailedResult(IResult failedResult)
                => Result<TPublishResult>.Failed(failedResult.Errors);

            var initiateResult = await InitiateFileUpload(cancel)
                .ToResultAsync(r => r, SharedResourcesLocalizer)
                .ConfigureAwait(false);

            if (!initiateResult.Success)
            {
                return GetFailedResult(initiateResult);
            }

            var fileName = string.IsNullOrWhiteSpace(options.FileName) ? Guid.NewGuid().ToString("N") : options.FileName;
            var uploadSessionId = initiateResult.Value.Item?.UploadSessionId ?? string.Empty;
            var boundary = Guid.NewGuid().ToString("N");
            var uploadSessionUri = $"{RestUrlPrefixes.FileUploads}/{uploadSessionId}";

            var chunkResults = await _httpStreamProcessor
                .ProcessAsync<FileUploadResponse>(
                    options.File,
                    (chunk, bytesRead) => BuildChunkRequest(uploadSessionUri, boundary, fileName, chunk, bytesRead),
                    cancel)
                .ConfigureAwait(false);

            var lastResult = chunkResults.Last()
                .ToResult(r => r, SharedResourcesLocalizer);

            if (!lastResult.Success)
            {
                return GetFailedResult(lastResult);
            }

            return await CommitPublishedContentAsync(options, uploadSessionId, boundary, cancel).ConfigureAwait(false);
        }

        private async Task<IResult<TPublishResult>> CommitPublishedContentAsync(
            TPublishOptions options,
            string uploadSessionId,
            string boundary,
            CancellationToken cancel)
        {
            var content = BuildCommitRequestContent(boundary, options);

            return await SendCommitRequestAsync(options, uploadSessionId, content, cancel).ConfigureAwait(false);
        }

        protected abstract Task<IResult<TPublishResult>> SendCommitRequestAsync(
            TPublishOptions options,
            string uploadSessionId,
            MultipartContent content,
            CancellationToken cancel);

        private async Task<IHttpResponseMessage<FileUploadResponse>> InitiateFileUpload(CancellationToken cancel)
        {
            return await RestRequestBuilderFactory
                .CreateUri(RestUrlPrefixes.FileUploads)
                .WithApiVersion(_sessionProvider.Version!.Value.RestApiVersion)
                .WithSiteId(_sessionProvider.SiteId!.Value)
                .ForPostRequest()
                .SendAsync<FileUploadResponse>(cancel)
                .ConfigureAwait(false);
        }

        internal static MultipartContent BuildChunkContent(
            string boundary,
            string filename,
            byte[] chunk,
            int bytesRead)
        {
            var partialContent = new MultipartContent("mixed", boundary);

            var headerContent = new StringContent(string.Empty);
            headerContent.Headers.TryAddWithoutValidation(RestHeaders.ContentDisposition, "name=\"request_payload\"");
            headerContent.Headers.ContentType = MediaTypes.TextXml;

            var fileData = new ByteArrayContent(chunk, 0, bytesRead);
            fileData.Headers.TryAddWithoutValidation(RestHeaders.ContentDisposition, $"name=\"tableau_file\"; filename=\"{filename}\"");
            fileData.Headers.ContentType = MediaTypes.OctetStream;

            partialContent.Add(headerContent);
            partialContent.Add(fileData);

            return partialContent;
        }

        private HttpRequestMessage BuildChunkRequest(
            string uploadSessionUri,
            string boundary,
            string filename,
            byte[] chunk,
            int bytesRead)
        {
            var partialContent = BuildChunkContent(boundary, filename, chunk, bytesRead);

            return RestRequestBuilderFactory
                .CreateUri(uploadSessionUri)
                .WithApiVersion(_sessionProvider.Version!.Value.RestApiVersion)
                .WithSiteId(_sessionProvider.SiteId!.Value)
                .ForPutRequest()
                .WithContent(partialContent)
                .Request;
        }

        private MultipartContent BuildCommitRequestContent(
            string boundary,
            TPublishOptions options)
        {
            var xmlRequest = BuildCommitRequest(options);
            var payloadContent = new StringContent(xmlRequest.ToXml());

            payloadContent.Headers.TryAddWithoutValidation(RestHeaders.ContentDisposition, "name=\"request_payload\"");
            payloadContent.Headers.ContentType = MediaTypes.TextXml;
            return new MultipartContent("mixed", boundary)
            {
                payloadContent
            };
        }
    }
}
