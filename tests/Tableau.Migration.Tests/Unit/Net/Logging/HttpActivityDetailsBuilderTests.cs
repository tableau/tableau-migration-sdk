//
//  Copyright (c) 2025, Salesforce, Inc.
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
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Moq;
using Tableau.Migration.Config;
using Tableau.Migration.Net.Logging;
using Tableau.Migration.Resources;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net.Logging
{
    public sealed class HttpActivityDetailsBuilderTests
    {
        public abstract class HttpActivityDetailsBuilderTest : AutoFixtureTestBase
        {
            protected NetworkOptions Config { get; }
            internal Mock<IHttpContentRedactor> MockRedactor { get; }
            protected Mock<ISharedResourcesLocalizer> MockLocalizer { get; }

            internal HttpActivityDetailsBuilder Builder { get; }

            public HttpActivityDetailsBuilderTest()
            {
                Config = Freeze<NetworkOptions>();

                MockRedactor = Freeze<Mock<IHttpContentRedactor>>();

                MockLocalizer = Freeze<Mock<ISharedResourcesLocalizer>>();
                MockLocalizer.Setup(x => x[SharedResourceKeys.HttpActivityEmptyText])
                    .Returns(new LocalizedString(SharedResourceKeys.HttpActivityEmptyText, string.Empty));

                Builder = Create<HttpActivityDetailsBuilder>();
            }
        }

        #region - AddHttpHeaderDetails -

        public class AddHttpHeaderDetails : HttpActivityDetailsBuilderTest
        {
            [Fact]
            public void EnableHeadersDetailsWithoutHeaders()
            {
                // Arrange
                var request = new HttpRequestMessage();
                var response = new HttpResponseMessage();
                Config.HeadersLoggingEnabled = true;

                // Act
                Builder.AddHttpHeaderDetails(request, response);

                // Assert
                MockLocalizer.Verify(x => x[SharedResourceKeys.HttpRequestHeadersSectionName], Times.Never);
                MockLocalizer.Verify(x => x[SharedResourceKeys.HttpResponseHeadersSectionName], Times.Never);
                Assert.Empty(Builder.Build());
            }

            [Fact]
            public void EnableHeadersDetailsWithoutContentHeaders()
            {
                // Arrange
                var request = new HttpRequestMessage
                {
                    Content = new StringContent(string.Empty)
                };
                request.Content.Headers.Clear();
                Config.HeadersLoggingEnabled = true;

                // Act
                Builder.AddHttpHeaderDetails(request);

                // Assert
                MockLocalizer.Verify(x => x[SharedResourceKeys.HttpRequestHeadersSectionName], Times.Never);
                MockLocalizer.Verify(x => x[SharedResourceKeys.HttpResponseHeadersSectionName], Times.Never);
                Assert.Empty(Builder.Build());
            }

            [Fact]
            public void EnableHeadersDetailsWithOnlyDisallowedHeaders()
            {
                // Arrange
                var request = new HttpRequestMessage();
                request.Headers.Authorization = new AuthenticationHeaderValue("bearer", "test");
                var response = new HttpResponseMessage();
                response.Headers.Date = DateTimeOffset.UtcNow;
                Config.HeadersLoggingEnabled = true;

                // Act
                Builder.AddHttpHeaderDetails(request, response);

                // Assert
                MockLocalizer.Verify(x => x[SharedResourceKeys.HttpRequestHeadersSectionName], Times.Never);
                MockLocalizer.Verify(x => x[SharedResourceKeys.HttpResponseHeadersSectionName], Times.Never);
                Assert.Empty(Builder.Build());
            }

            [Fact]
            public void EnableHeadersDetailsWithOnlyDisallowedContentHeaders()
            {
                // Arrange
                var request = new HttpRequestMessage
                {
                    Content = new StringContent(string.Empty)
                };
                request.Content.Headers.Clear();
                request.Content.Headers.Add("bearer", "test");
                
                Config.HeadersLoggingEnabled = true;

                // Act
                Builder.AddHttpHeaderDetails(request);

                // Assert
                MockLocalizer.Verify(x => x[SharedResourceKeys.HttpRequestHeadersSectionName], Times.Never);
                MockLocalizer.Verify(x => x[SharedResourceKeys.HttpResponseHeadersSectionName], Times.Never);
                Assert.Empty(Builder.Build());
            }

            [Fact]
            public void EnableHeadersDetailsWithAcceptHeader()
            {
                // Arrange
                var request = new HttpRequestMessage();
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/json"));
                var response = new HttpResponseMessage();
                response.Headers.CacheControl = new CacheControlHeaderValue
                {
                    NoCache = true
                };
                Config.HeadersLoggingEnabled = true;

                // Act
                Builder.AddHttpHeaderDetails(request, response);

                // Assert
                MockLocalizer.Verify(x => x[SharedResourceKeys.HttpRequestHeadersSectionName], Times.Once);
                MockLocalizer.Verify(x => x[SharedResourceKeys.HttpResponseHeadersSectionName], Times.Once);
                Assert.NotEmpty(Builder.Build());
            }

            [Fact]
            public void EnableHeadersDetailsWithContentTypeHeader()
            {
                // Arrange
                var request = new HttpRequestMessage
                {
                    Content = new StringContent(string.Empty)
                };
                request.Content.Headers.Clear();
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                
                Config.HeadersLoggingEnabled = true;

                // Act
                Builder.AddHttpHeaderDetails(request);

                // Assert
                MockLocalizer.Verify(x => x[SharedResourceKeys.HttpRequestHeadersSectionName], Times.Once);
                MockLocalizer.Verify(x => x[SharedResourceKeys.HttpResponseHeadersSectionName], Times.Never);
                Assert.NotEmpty(Builder.Build());
            }
        }

        #endregion

        #region - AddHttpContentDetailsAsync -

        public class AddHttpContentDetailsAsync : HttpActivityDetailsBuilderTest
        {
            [Fact]
            public async Task EnableContentDetailsWithoutContentAsync()
            {
                // Arrange
                var request = new HttpRequestMessage();
                
                Config.ContentLoggingEnabled = true;

                // Act
                await Builder.AddHttpContentDetailsAsync(request, null, Cancel);

                // Assert
                MockLocalizer.Verify(x => x[SharedResourceKeys.HttpRequestContentSectionName], Times.Never);
                MockLocalizer.Verify(x => x[SharedResourceKeys.HttpResponseContentSectionName], Times.Never);
                Assert.Empty(Builder.Build());
            }

            [Fact]
            public async Task EnableMultipartContentDetailsWithoutContentAsync()
            {
                // Arrange
                var request = new HttpRequestMessage
                {
                    Content = new MultipartFormDataContent()
                };
                
                Config.ContentLoggingEnabled = true;

                // Act
                await Builder.AddHttpContentDetailsAsync(request, null, Cancel);

                // Assert
                MockLocalizer.Verify(x => x[SharedResourceKeys.HttpRequestContentSectionName], Times.Once);
                MockLocalizer.Verify(x => x[SharedResourceKeys.HttpResponseContentSectionName], Times.Never);
                Assert.NotEmpty(Builder.Build());
            }

            [Fact]
            public async Task EnableContentDetailsWithEmptyPdfContentAsync()
            {
                // Arrange
                var request = new HttpRequestMessage
                {
                    Content = new StringContent(string.Empty)
                };
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                
                Config.ContentLoggingEnabled = true;

                // Act
                await Builder.AddHttpContentDetailsAsync(request, null, Cancel);

                // Assert
                MockLocalizer.Verify(x => x[SharedResourceKeys.HttpRequestContentSectionName], Times.Once);
                MockLocalizer.Verify(x => x[SharedResourceKeys.HttpResponseContentSectionName], Times.Never);
                Assert.NotEmpty(Builder.Build());
            }

            [Fact]
            public async Task EnableMultipartContentDetailsWithEmptyPdfContentAsync()
            {
                // Arrange
                var request = new HttpRequestMessage();
                var multipart = new MultipartFormDataContent();
                var content = new StringContent(string.Empty);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                multipart.Add(content);
                request.Content = multipart;
                
                Config.ContentLoggingEnabled = true;

                // Act
                await Builder.AddHttpContentDetailsAsync(request, null, Cancel);

                // Assert
                MockLocalizer.Verify(x => x[SharedResourceKeys.HttpRequestContentSectionName], Times.Once);
                MockLocalizer.Verify(x => x[SharedResourceKeys.HttpResponseContentSectionName], Times.Never);
                MockLocalizer.Verify(x => x[SharedResourceKeys.HttpActivityNotDisplayedText], Times.Once);
                Assert.NotEmpty(Builder.Build());
            }

            [Fact]
            public async Task EnableBinaryContentDetailsWithEmptyPdfContentAsync()
            {
                // Arrange
                var request = new HttpRequestMessage
                {
                    Content = new StringContent(string.Empty)
                };
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                
                Config.ContentLoggingEnabled = true;
                Config.BinaryContentLoggingEnabled = true;

                // Act
                await Builder.AddHttpContentDetailsAsync(request, null, Cancel);

                // Assert
                MockLocalizer.Verify(x => x[SharedResourceKeys.HttpRequestContentSectionName], Times.Once);
                MockLocalizer.Verify(x => x[SharedResourceKeys.HttpResponseContentSectionName], Times.Never);
                Assert.NotEmpty(Builder.Build());

                MockRedactor.Verify(x => x.IsSensitiveMultipartContent(It.IsAny<string>()), Times.Once);
                MockRedactor.Verify(x => x.ReplaceSensitiveData(It.IsAny<string>()), Times.Once);
            }

            [Fact]
            public async Task EnableMultipartBinaryContentDetailsWithEmptyPdfContentAsync()
            {
                // Arrange
                var request = new HttpRequestMessage();
                var multipart = new MultipartFormDataContent();
                var content = new StringContent(string.Empty);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                multipart.Add(content);
                request.Content = multipart;
                
                Config.ContentLoggingEnabled = true;
                Config.BinaryContentLoggingEnabled = true;

                // Act
                await Builder.AddHttpContentDetailsAsync(request, null, Cancel);

                // Assert
                MockLocalizer.Verify(x => x[SharedResourceKeys.HttpRequestContentSectionName], Times.Once);
                MockLocalizer.Verify(x => x[SharedResourceKeys.HttpResponseContentSectionName], Times.Never);
                Assert.NotEmpty(Builder.Build());

                MockRedactor.Verify(x => x.IsSensitiveMultipartContent(It.IsAny<string>()), Times.Once);
                MockRedactor.Verify(x => x.ReplaceSensitiveData(It.IsAny<string>()), Times.Once);
            }

            [Fact]
            public async Task EnableContentDetailsWithEmptyTextContentAsync()
            {
                // Arrange
                var request = new HttpRequestMessage
                {
                    Content = new StringContent(string.Empty)
                };
                
                Config.ContentLoggingEnabled = true;

                // Act
                await Builder.AddHttpContentDetailsAsync(request, null, Cancel);

                // Assert
                MockLocalizer.Verify(x => x[SharedResourceKeys.HttpRequestContentSectionName], Times.Once);
                MockLocalizer.Verify(x => x[SharedResourceKeys.HttpResponseContentSectionName], Times.Never);
                Assert.NotEmpty(Builder.Build());

                MockRedactor.Verify(x => x.IsSensitiveMultipartContent(It.IsAny<string>()), Times.Once);
                MockRedactor.Verify(x => x.ReplaceSensitiveData(It.IsAny<string>()), Times.Once);
            }

            [Fact]
            public async Task EnableMultipartContentDetailsWithEmptyTextContentAsync()
            {
                // Arrange
                var request = new HttpRequestMessage();
                var multipart = new MultipartFormDataContent();
                var content = new StringContent(string.Empty);
                multipart.Add(content);
                request.Content = multipart;
                
                Config.ContentLoggingEnabled = true;

                // Act
                await Builder.AddHttpContentDetailsAsync(request, null, Cancel);

                // Assert
                MockLocalizer.Verify(x => x[SharedResourceKeys.HttpRequestContentSectionName], Times.Once);
                MockLocalizer.Verify(x => x[SharedResourceKeys.HttpResponseContentSectionName], Times.Never);
                Assert.NotEmpty(Builder.Build());

                MockRedactor.Verify(x => x.IsSensitiveMultipartContent(It.IsAny<string>()), Times.Once);
                MockRedactor.Verify(x => x.ReplaceSensitiveData(It.IsAny<string>()), Times.Once);
            }

            [Fact]
            public async Task EnableMultipartContentDetailsTooLargeAsync()
            {
                // Arrange
                var request = new HttpRequestMessage();
                var multipart = new MultipartFormDataContent();
                var content = new StringContent(string.Empty);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                content.Headers.ContentLength = (long)int.MaxValue + 1;
                multipart.Add(content);
                request.Content = multipart;
                
                Config.ContentLoggingEnabled = true;
                Config.BinaryContentLoggingEnabled = true;

                // Act
                await Builder.AddHttpContentDetailsAsync(request, null, Cancel);

                // Assert
                MockLocalizer.Verify(x => x[SharedResourceKeys.HttpRequestContentSectionName], Times.Once);
                MockLocalizer.Verify(x => x[SharedResourceKeys.HttpResponseContentSectionName], Times.Never);
                MockLocalizer.Verify(x => x[SharedResourceKeys.HttpActivityNotDisplayedText], Times.Never);
                MockLocalizer.Verify(x => x[SharedResourceKeys.HttpContentTooLargeText], Times.Once);
                Assert.NotEmpty(Builder.Build());

                MockRedactor.Verify(x => x.IsSensitiveMultipartContent(It.IsAny<string>()), Times.Once);
            }

            [Fact]
            public async Task EnableMultipleContentDetailsAsync()
            {
                // Arrange
                var request = new HttpRequestMessage();
                var multipart = new MultipartFormDataContent();
                var content = new StringContent(string.Empty);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                multipart.Add(content);
                multipart.Add(new StringContent(string.Empty));
                request.Content = multipart;
                
                Config.ContentLoggingEnabled = true;

                // Act
                await Builder.AddHttpContentDetailsAsync(request, null, Cancel);

                // Assert
                MockLocalizer.Verify(x => x[SharedResourceKeys.HttpRequestContentSectionName], Times.Once);
                MockLocalizer.Verify(x => x[SharedResourceKeys.HttpResponseContentSectionName], Times.Never);
                MockLocalizer.Verify(x => x[SharedResourceKeys.HttpActivityNotDisplayedText], Times.Once);
                Assert.NotEmpty(Builder.Build());

                MockRedactor.Verify(x => x.IsSensitiveMultipartContent(It.IsAny<string>()), Times.Once);
                MockRedactor.Verify(x => x.ReplaceSensitiveData(It.IsAny<string>()), Times.Once);
            }

            [Fact]
            public async Task EnableMultipleContentDetailsWithSensitiveDataAsync()
            {
                // Arrange
                var request = new HttpRequestMessage();
                var multipart = new MultipartFormDataContent();
                var content = new StringContent(string.Empty);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                content.Headers.ContentDisposition = new ContentDispositionHeaderValue("test")
                {
                    Name = "password"
                };
                multipart.Add(content);
                multipart.Add(new StringContent(string.Empty));
                request.Content = multipart;

                Config.ContentLoggingEnabled = true;
                Config.BinaryContentLoggingEnabled = true;

                MockRedactor.Setup(x => x.IsSensitiveMultipartContent("password")).Returns(true);

                // Act
                await Builder.AddHttpContentDetailsAsync(request, null, Cancel);

                // Assert
                MockLocalizer.Verify(x => x[SharedResourceKeys.HttpRequestContentSectionName], Times.Once);
                MockLocalizer.Verify(x => x[SharedResourceKeys.HttpResponseContentSectionName], Times.Never);
                Assert.NotEmpty(Builder.Build());

                MockRedactor.Verify(x => x.IsSensitiveMultipartContent(It.IsAny<string>()), Times.Exactly(2));
                MockRedactor.Verify(x => x.ReplaceSensitiveData(It.IsAny<string>()), Times.Once);
            }
        }

        #endregion
    }
}
