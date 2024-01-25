using System;
using System.Net.Http;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Api.Publishing;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models.Responses;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Publishing
{
    public abstract class FilePublisherTestBase<TPublisher, TPublishOptions, TPublishResult> : ApiTestBase
        where TPublisher : IFilePublisher<TPublishOptions, TPublishResult>
        where TPublishOptions : IPublishFileOptions
        where TPublishResult : class, IContentReference
    {
        protected readonly string ContentTypeUrlPrefix;

        protected abstract TPublisher Publisher { get; }

        public FilePublisherTestBase(string contentTypeUrlPrefix)
        {
            ContentTypeUrlPrefix = contentTypeUrlPrefix;

            MockSessionProvider.SetupGet(p => p.SiteContentUrl).Returns(SiteConnectionConfiguration.SiteContentUrl);
            MockSessionProvider.SetupGet(p => p.SiteId).Returns(Create<Guid>());
            MockSessionProvider.SetupGet(p => p.UserId).Returns(Create<Guid>());
        }

        protected void AssertRequests(
            FileUploadResponse.FileUploadType initiateResponse,
            Action<HttpRequestMessage> assertCommitRequest)
        {
            HttpRequestMessage? streamRequest = null;

            MockHttpClient.AssertRequests(
                r =>
                {
                    r.AssertHttpMethod(HttpMethod.Post);
                    r.AssertSiteUri(SiteConnectionConfiguration, MockSessionProvider.Object, RestUrlPrefixes.FileUploads);
                },
                r =>
                {
                    streamRequest = r;

                    r.AssertHttpMethod(HttpMethod.Put);
                    r.AssertSiteUri(SiteConnectionConfiguration, MockSessionProvider.Object, $"{RestUrlPrefixes.FileUploads}/{initiateResponse.UploadSessionId}");
                },
                r =>
                {
                    r.AssertHttpMethod(HttpMethod.Post);
                    r.AssertSiteUri(SiteConnectionConfiguration, MockSessionProvider.Object, ContentTypeUrlPrefix);
                    r.AssertQuery("uploadSessionId", initiateResponse.UploadSessionId!);

                    assertCommitRequest(r);
                });

            Assert.Same(streamRequest, HttpStreamProcessor.AssertSingleRequest());
        }
    }
}
