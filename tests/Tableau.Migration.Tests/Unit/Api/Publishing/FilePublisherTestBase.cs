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
