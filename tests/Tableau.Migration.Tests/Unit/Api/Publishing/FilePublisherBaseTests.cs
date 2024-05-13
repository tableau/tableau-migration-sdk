//
//  Copyright (c) 2024, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the "License") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Api.Publishing;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models.Requests;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Net;
using Tableau.Migration.Net.Rest;
using Tableau.Migration.Resources;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Publishing
{
    public class FilePublisherBaseTests
    {
        #region - Test Types -

        public interface ITestPublishOptions : IPublishFileOptions
        { }

        [XmlType(XmlTypeName)]
        public class TestPublishRequest : TableauServerRequest
        {
            public ITestPublishOptions? Options { get; }

            public TestPublishRequest()
            { }

            public TestPublishRequest(ITestPublishOptions options)
            {
                Options = options;
            }
        }

        public interface ITestFilePublisher : IFilePublisher<ITestPublishOptions, TestContentType>
        { }

        internal class TestFilePublisher : FilePublisherBase<ITestPublishOptions, TestPublishRequest, TestContentType>, ITestFilePublisher
        {
            public record BuildCommitRequestCall(
                ITestPublishOptions Options,
                TestPublishRequest BuiltRequest);

            public record SendCommitRequestCall(
                ITestPublishOptions Options,
                string UploadSessionId,
                MultipartContent Content,
                CancellationToken Cancel);

            public List<BuildCommitRequestCall> BuildCommitRequestCalls = new();
            public List<SendCommitRequestCall> SendCommitRequestCalls = new();

            public TestFilePublisher(
                IRestRequestBuilderFactory restRequestBuilderFactory,
                IContentReferenceFinderFactory finderFactory,
                IServerSessionProvider sessionProvider,
                ILoggerFactory loggerFactory,
                ISharedResourcesLocalizer sharedResourcesLocalizer,
                IHttpStreamProcessor httpStreamProcessor,
                string contentTypeUrlPrefix)
                : base(restRequestBuilderFactory, finderFactory, sessionProvider, loggerFactory, sharedResourcesLocalizer, httpStreamProcessor, contentTypeUrlPrefix)
            { }

            protected override TestPublishRequest BuildCommitRequest(ITestPublishOptions options)
            {
                var request = new TestPublishRequest(options);
                BuildCommitRequestCalls.Add(new(options, request));
                return request;
            }

            protected override Task<IResult<TestContentType>> SendCommitRequestAsync(
                ITestPublishOptions options,
                string uploadSessionId,
                MultipartContent content,
                CancellationToken cancel)
            {
                SendCommitRequestCalls.Add(new(options, uploadSessionId, content, cancel));
                return Task.FromResult<IResult<TestContentType>>(Result<TestContentType>.Succeeded(new TestContentType()));
            }
        }

        #endregion

        public abstract class FilePublisherBaseTest : FilePublisherTestBase<ITestFilePublisher, ITestPublishOptions, TestContentType>
        {
            private const string UrlPrefix = nameof(TestContentType);

            internal readonly TestFilePublisher TestPublisher;

            protected override ITestFilePublisher Publisher => TestPublisher;

            public FilePublisherBaseTest()
                : base(UrlPrefix)
            {
                TestPublisher = new(
                    RestRequestBuilderFactory,
                    MockContentFinderFactory.Object,
                    MockSessionProvider.Object,
                    MockLoggerFactory.Object,
                    MockSharedResourcesLocalizer.Object,
                    HttpStreamProcessor,
                    UrlPrefix);
            }

            protected void AssertInitiateRequest()
            {
                MockHttpClient.AssertRequests(
                    r =>
                    {
                        r.AssertHttpMethod(HttpMethod.Post);
                        r.AssertSiteUri(SiteConnectionConfiguration, MockSessionProvider.Object, RestUrlPrefixes.FileUploads);
                    });
            }

            protected void AssertCommitRequest(
                ITestPublishOptions options,
                string uploadSessionId,
                CancellationToken cancel)
            {
                var buildCommitRequestCall = Assert.Single(TestPublisher.BuildCommitRequestCalls);
                var commitRequestCall = Assert.Single(TestPublisher.SendCommitRequestCalls);

                Assert.Same(options, commitRequestCall.Options);
                Assert.Same(uploadSessionId, commitRequestCall.UploadSessionId);
                Assert.Equal(cancel, commitRequestCall.Cancel);

                Assert.Collection(
                    commitRequestCall.Content,
                    c =>
                    {
                        var stringContent = Assert.IsType<StringContent>(c);
                        Assert.Equal(buildCommitRequestCall.BuiltRequest.ToXml(), stringContent.ReadAsStringAsync().Result);
                        stringContent.AssertSingleHeaderValue(RestHeaders.ContentDisposition, "name=\"request_payload\"");
                        Assert.Equal(MediaTypes.TextXml, stringContent.Headers.ContentType);
                    });
            }
        }

        public class PublishAsync : FilePublisherBaseTest
        {
            [Fact]
            public async Task Publishes()
            {
                var initiateResponse = SetupSuccessResponse<FileUploadResponse, FileUploadResponse.FileUploadType>();

                var publishOptions = Create<ITestPublishOptions>();

                await Publisher.PublishAsync(publishOptions, Cancel);

                AssertInitiateRequest();
                AssertCommitRequest(publishOptions, initiateResponse.Item.UploadSessionId!, Cancel);
            }

            [Fact]
            public async Task Returns_failure_when_initiate_fails()
            {
                var response = SetupErrorResponse<FileUploadResponse>();

                var publishResult = await Publisher.PublishAsync(Create<ITestPublishOptions>(), Cancel);

                Assert.False(publishResult.Success);

                var exception = Assert.Single(publishResult.Errors);
                var restException = Assert.IsType<RestException>(exception);
                restException.AssertErrorEquals(response.Error);

                AssertInitiateRequest();
                MockHttpClient.AssertSingleRequest();
            }
        }
    }
}
