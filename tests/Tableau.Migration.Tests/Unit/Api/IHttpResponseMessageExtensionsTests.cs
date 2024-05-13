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
using System.Collections.Immutable;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Net;
using Tableau.Migration.Net.Rest;
using Tableau.Migration.Resources;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api
{
    public class IHttpResponseMessageExtensionsTests
    {
        #region - Test Classes -

        public class TestModel
        {
            public TestTableauServerResponse Response { get; init; }

            public TestModel(TestTableauServerResponse r)
            {
                Response = r;
            }
        }

        public abstract class IHttpResponseMessageExtensionsTest : AutoFixtureTestBase
        {
            protected readonly Mock<IHttpContentSerializer> MockSerializer = new();
            protected readonly Mock<IHttpResponseMessage> MockResponse = new();
            protected readonly Mock<HttpContent> MockContent = new() { CallBase = true };
            protected readonly HttpContentHeaders ContentHeaders;

            public IHttpResponseMessageExtensionsTest()
            {
                // We can't mock HttpContentHeaders because it's sealed,
                // but HttpContent.Headers is lazily initialized, so we're using the instance
                // from the mocked content.
                ContentHeaders = MockContent.Object.Headers;

                MockResponse.SetupGet(x => x.Content).Returns(MockContent.Object);
            }
        }

        public abstract class IHttpResponseMessageExtensionsTest<TResponse> : AutoFixtureTestBase
            where TResponse : class
        {
            protected readonly Mock<IHttpContentSerializer> MockSerializer = new();
            protected readonly Mock<IHttpResponseMessage<TResponse>> MockResponse = new();
        }

        #endregion

        #region - ToResultAsync (Serializer) -

        public class ToResultAsyncSerializer : IHttpResponseMessageExtensionsTest
        {
            [Fact]
            public async Task Returns_failure_on_exception()
            {
                var expectedException = new Exception();

                MockResponse.Setup(r => r.EnsureSuccessStatusCode()).Throws(expectedException);

                var result = await MockResponse.Object.ToResultAsync(MockSerializer.Object, Mock.Of<ISharedResourcesLocalizer>(), Cancel);

                Assert.False(result.Success);
                var actualException = Assert.Single(result.Errors);

                Assert.Same(expectedException, actualException);
            }

            [Fact]
            public async Task Returns_failure_on_server_error()
            {
                var mockLocalizer = new Mock<ISharedResourcesLocalizer>();
                mockLocalizer
                    .Setup(x => x[It.IsAny<string>()])
                    .Returns(new LocalizedString("Error", "Error"));
                var error = new TestTableauServerResponse
                {
                    Error = Create<Error>()
                };

                var content = HttpContentSerializer.Instance.Serialize(error, MediaTypes.Xml)!;
                MockResponse.Setup(r => r.Content).Returns(content);

                MockSerializer.Setup(s => s.TryDeserializeErrorAsync(content, Cancel))
                    .ReturnsAsync(error.Error);

                var result = await MockResponse.Object.ToResultAsync(MockSerializer.Object, mockLocalizer.Object, Cancel);

                Assert.False(result.Success);
                var actualException = Assert.Single(result.Errors);

                Assert.IsType<RestException>(actualException);
            }

            [Fact]
            public async Task Returns_success()
            {
                var result = await MockResponse.Object.ToResultAsync(MockSerializer.Object, Mock.Of<ISharedResourcesLocalizer>(), Cancel);

                Assert.True(result.Success);
            }
        }

        #endregion

        #region - ToResultAsync (Async Model Factory) -

        public class ToResultAsyncModelAsync : IHttpResponseMessageExtensionsTest<TestTableauServerResponse>
        {
            [Fact]
            public async Task Returns_failure_on_exception()
            {
                var expectedException = new Exception();

                MockResponse.Setup(r => r.EnsureSuccessStatusCode()).Throws(expectedException);

                var result = await Task.FromResult(MockResponse.Object)
                    .ToResultAsync(
                        async (r, c) => await Task.FromResult(new TestModel(r)),
                        Mock.Of<ISharedResourcesLocalizer>()
                    , Cancel);

                Assert.False(result.Success);
                var actualException = Assert.Single(result.Errors);

                Assert.Same(expectedException, actualException);
            }

            [Fact]
            public async Task Returns_failure_on_server_error()
            {
                var mockLocalizer = new Mock<ISharedResourcesLocalizer>();
                mockLocalizer
                    .Setup(x => x[It.IsAny<string>()])
                    .Returns(new LocalizedString("Error", "Error"));
                var error = new TestTableauServerResponse
                {
                    Error = Create<Error>()
                };

                MockResponse.Setup(x => x.DeserializedContent).Returns(error);

                var result = await Task.FromResult(MockResponse.Object)
                    .ToResultAsync(
                        async (r, c) => await Task.FromResult(new TestModel(r)),
                        mockLocalizer.Object,
                        Cancel);

                Assert.False(result.Success);
                var actualException = Assert.Single(result.Errors);

                Assert.IsType<RestException>(actualException);
            }

            [Fact]
            public async Task Returns_success()
            {
                var resp = new TestTableauServerResponse();
                MockResponse.Setup(x => x.DeserializedContent).Returns(resp);

                var result = await Task.FromResult(MockResponse.Object)
                    .ToResultAsync(
                        async (r, c) => await Task.FromResult(new TestModel(r)),
                        Mock.Of<ISharedResourcesLocalizer>()
                    , Cancel);

                Assert.True(result.Success);
                Assert.Same(resp, result.Value.Response);
            }
        }

        #endregion

        #region - ToPagedResultAsync (Sync Model Factory) -

        public class ToPagedResultSync : IHttpResponseMessageExtensionsTest<TestPagedTableauServerResponse>
        {
            [Fact]
            public void Returns_failure_on_exception()
            {
                var expectedException = new Exception();

                MockResponse.Setup(r => r.EnsureSuccessStatusCode()).Throws(expectedException);

                var result = MockResponse.Object.ToPagedResult(r => r.Items.ToImmutableArray(), Mock.Of<ISharedResourcesLocalizer>());

                Assert.False(result.Success);
                var actualException = Assert.Single(result.Errors);

                Assert.Same(expectedException, actualException);
            }

            [Fact]
            public void Returns_failure_on_server_error()
            {
                var mockLocalizer = new Mock<ISharedResourcesLocalizer>();
                mockLocalizer
                    .Setup(x => x[It.IsAny<string>()])
                    .Returns(new LocalizedString("Error", "Error"));
                var error = new TestPagedTableauServerResponse
                {
                    Error = Create<Error>()
                };

                MockResponse.Setup(r => r.DeserializedContent).Returns(error);

                var result = MockResponse.Object.ToPagedResult(r => r.Items.ToImmutableArray(), mockLocalizer.Object);

                Assert.False(result.Success);
                var actualException = Assert.Single(result.Errors);

                Assert.IsType<RestException>(actualException);
            }

            [Fact]
            public void Returns_success()
            {
                var error = new TestPagedTableauServerResponse();

                MockResponse.Setup(r => r.DeserializedContent).Returns(error);

                var result = MockResponse.Object.ToPagedResult(r => r.Items.ToImmutableArray(), Mock.Of<ISharedResourcesLocalizer>());

                Assert.True(result.Success);
            }
        }

        #endregion

        #region - ToPagedResultAsync (Async Model Factory) -

        public class ToPagedResultAsync : IHttpResponseMessageExtensionsTest<TestPagedTableauServerResponse>
        {
            [Fact]
            public async Task ReturnsFailureOnExceptionAsync()
            {
                var expectedException = new Exception();

                MockResponse.Setup(r => r.EnsureSuccessStatusCode()).Throws(expectedException);

                var result = await MockResponse.Object.ToPagedResultAsync(
                    (r, c) => Task.FromResult<IImmutableList<object>>(r.Items.ToImmutableArray()),
                    Mock.Of<ISharedResourcesLocalizer>(), Cancel);

                Assert.False(result.Success);
                var actualException = Assert.Single(result.Errors);

                Assert.Same(expectedException, actualException);
            }

            [Fact]
            public async Task ReturnsFailureOnServerErrorAsync()
            {
                var mockLocalizer = new Mock<ISharedResourcesLocalizer>();
                mockLocalizer
                    .Setup(x => x[It.IsAny<string>()])
                    .Returns(new LocalizedString("Error", "Error"));
                var error = new TestPagedTableauServerResponse
                {
                    Error = Create<Error>()
                };

                MockResponse.Setup(r => r.DeserializedContent).Returns(error);

                var result = await MockResponse.Object.ToPagedResultAsync(
                    (r, c) => Task.FromResult<IImmutableList<object>>(r.Items.ToImmutableArray()),
                    mockLocalizer.Object, Cancel);

                Assert.False(result.Success);
                var actualException = Assert.Single(result.Errors);

                Assert.IsType<RestException>(actualException);
            }

            [Fact]
            public async Task ReturnsSuccessAsync()
            {
                var response = new TestPagedTableauServerResponse();

                MockResponse.Setup(r => r.DeserializedContent).Returns(response);

                var result = await MockResponse.Object.ToPagedResultAsync(
                    (r, c) => Task.FromResult<IImmutableList<object>>(r.Items.ToImmutableArray()),
                    Mock.Of<ISharedResourcesLocalizer>(), Cancel);

                Assert.True(result.Success);
            }

            [Fact]
            public async Task EvaluatesTaskAsync()
            {
                var response = new TestPagedTableauServerResponse();

                MockResponse.Setup(r => r.DeserializedContent).Returns(response);

                var task = Task.FromResult<IHttpResponseMessage<TestPagedTableauServerResponse>>(MockResponse.Object);

                var result = await task.ToPagedResultAsync(
                    (r, c) => Task.FromResult<IImmutableList<object>>(r.Items.ToImmutableArray()),
                    Mock.Of<ISharedResourcesLocalizer>(), Cancel);

                Assert.True(result.Success);
            }
        }

        #endregion

        #region - GetContentDispositionFilename -

        public class GetContentDispositionFilename : IHttpResponseMessageExtensionsTest
        {
            [Fact]
            public void NoHeader()
            {
                var result = MockResponse.Object.GetContentDispositionFilename();

                Assert.Null(result);
            }

            [Fact]
            public void EmptyHeader()
            {
                ContentHeaders.TryAddWithoutValidation(RestHeaders.ContentDisposition, new[] { (string?)null });

                var result = MockResponse.Object.GetContentDispositionFilename();

                Assert.Null(result);
            }

            [Fact]
            public void NoMatchingPart()
            {
                ContentHeaders.TryAddWithoutValidation(RestHeaders.ContentDisposition, "bad; key=value");

                var result = MockResponse.Object.GetContentDispositionFilename();

                Assert.Null(result);
            }

            [Fact]
            public void SupportsMultipartHeaderValue()
            {
                ContentHeaders.TryAddWithoutValidation(RestHeaders.ContentDisposition, "bad; key=value; filename=test");

                var result = MockResponse.Object.GetContentDispositionFilename();

                Assert.Equal("test", result);
            }

            [Fact]
            public void SupportsQuotedFilename()
            {
                ContentHeaders.TryAddWithoutValidation(RestHeaders.ContentDisposition, "name=\"tableau_datasource\"; filename=\"a test\"");

                var result = MockResponse.Object.GetContentDispositionFilename();

                Assert.Equal("a test", result);
            }

            [Fact]
            public void CaseInsensitive()
            {
                ContentHeaders.TryAddWithoutValidation(RestHeaders.ContentDisposition, "FileName=test");

                var result = MockResponse.Object.GetContentDispositionFilename();

                Assert.Equal("test", result);
            }
        }

        #endregion

        #region - DownloadResultAsync -

        public class DownloadResultAsync : IHttpResponseMessageExtensionsTest
        {
            [Fact]
            public async Task ExceptionAsync()
            {
                var expectedException = new Exception();

                MockResponse.Setup(r => r.EnsureSuccessStatusCode()).Throws(expectedException);

                var result = await Task.FromResult(MockResponse.Object)
                    .DownloadResultAsync(Cancel);

                Assert.False(result.Success);
                var actualException = Assert.Single(result.Errors);

                Assert.Same(expectedException, actualException);
            }

            [Fact]
            public async Task CreatesFileDownloadAsync()
            {
                var content = new ByteArrayContent(Constants.DefaultEncoding.GetBytes("test content"));
                MockResponse.Setup(x => x.Content).Returns(content);

                content.Headers.TryAddWithoutValidation(RestHeaders.ContentDisposition, "FileName=test");

                var result = await Task.FromResult(MockResponse.Object)
                    .DownloadResultAsync(Cancel);

                result.AssertSuccess();

                Assert.Equal("test", result.Value!.Filename);

                var resultContent = await new StreamReader(result.Value.Content).ReadToEndAsync();
                Assert.Equal("test content", resultContent);
            }
        }

        #endregion
    }
}
