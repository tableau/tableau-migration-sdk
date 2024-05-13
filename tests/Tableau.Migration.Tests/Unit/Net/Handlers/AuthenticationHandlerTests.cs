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
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Net.Handlers;
using Tableau.Migration.Net.Rest;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net.Handlers
{
    public class AuthenticationHandlerTests
    {
        public abstract class AuthenticationHandlerTest : AutoFixtureTestBase
        {
            protected readonly Mock<IAuthenticationTokenProvider> MockTokenProvider = new();

            internal readonly AuthenticationHandler Handler;

            public AuthenticationHandlerTest()
            {
                Handler = new(MockTokenProvider.Object);
            }

            protected static MockDelegatingHandler CreateMockDelegatingHandler(
                HttpStatusCode responseStatusCode,
                Action<HttpRequestMessage, int>? assertRequest = null)
            {
                var count = 0;

                return new MockDelegatingHandler(request =>
                {
                    count++;

                    assertRequest?.Invoke(request, count);

                    return new HttpResponseMessage(responseStatusCode);
                });
            }

            protected static void AssertAuthenticationHeader(HttpRequestMessage request, string? expectedToken)
            {
                if (expectedToken is null)
                {
                    request.AssertHeaderDoesNotExist(RestHeaders.AuthenticationToken);
                }
                else
                {
                    request.AssertSingleHeaderValue(RestHeaders.AuthenticationToken, expectedToken);
                }
            }
        }

        public class SendAsync : AuthenticationHandlerTest
        {
            [Fact]
            public async Task Skips_if_not_Rest_request()
            {
                var mockHandler = CreateMockDelegatingHandler(
                    HttpStatusCode.OK,
                    (request, _) =>
                    {
                        AssertAuthenticationHeader(request, null);
                    });

                Handler.InnerHandler = mockHandler;

                var invoker = new HttpMessageInvoker(Handler);

                await invoker.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://localhost/not-api/3.20/test"), Cancel);

                MockTokenProvider.VerifyNoOtherCalls();
            }

            [Fact]
            public async Task Sets_token()
            {
                var token = Create<string>();

                MockTokenProvider.Setup(p => p.GetAsync(Cancel)).ReturnsAsync(token);

                var mockHandler = CreateMockDelegatingHandler(
                    HttpStatusCode.OK,
                    (request, _) =>
                    {
                        AssertAuthenticationHeader(request, token);
                    });

                Handler.InnerHandler = mockHandler;

                var invoker = new HttpMessageInvoker(Handler);

                await invoker.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/3.20/test"), Cancel);

                MockTokenProvider.Verify(p => p.RequestRefreshAsync(It.IsAny<string?>(), It.IsAny<CancellationToken>()), Times.Never());
            }

            [Fact]
            public async Task Overwrites_token_with_provider_value()
            {
                var token = Create<string>();

                MockTokenProvider.Setup(p => p.GetAsync(Cancel)).ReturnsAsync(token);

                var mockHandler = CreateMockDelegatingHandler(
                    HttpStatusCode.OK,
                    (request, _) =>
                    {
                        AssertAuthenticationHeader(request, token);
                    });

                Handler.InnerHandler = mockHandler;

                var invoker = new HttpMessageInvoker(Handler);

                var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/3.20/test");
                request.Headers.TryAddWithoutValidation(RestHeaders.AuthenticationToken, Create<string>());

                await invoker.SendAsync(request, Cancel);
            }

            [Fact]
            public async Task Refreshes_token()
            {
                var token1 = Create<string>();
                var token2 = Create<string>();

                MockTokenProvider.Setup(p => p.GetAsync(Cancel)).ReturnsAsync(token1);

                MockTokenProvider
                    .Setup(p => p.RequestRefreshAsync(It.IsAny<string?>(), Cancel))
                    .Callback(() => MockTokenProvider.Setup(p => p.GetAsync(Cancel)).ReturnsAsync(token2));

                var mockHandler = CreateMockDelegatingHandler(
                    HttpStatusCode.Unauthorized,
                    (request, count) =>
                    {
                        AssertAuthenticationHeader(request, count == 1 ? token1 : token2);
                    });

                Handler.InnerHandler = mockHandler;

                var invoker = new HttpMessageInvoker(Handler);

                await invoker.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/3.20/test"), Cancel);

                MockTokenProvider.Verify(p => p.RequestRefreshAsync(It.IsAny<string?>(), It.IsAny<CancellationToken>()), Times.Once());
            }

            [Fact]
            public async Task Does_not_set_token_on_sign_in_attempts()
            {
                var mockHandler = CreateMockDelegatingHandler(HttpStatusCode.Unauthorized);

                Handler.InnerHandler = mockHandler;

                var invoker = new HttpMessageInvoker(Handler);

                await invoker.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/3.20/auth/signin"), Cancel);

                MockTokenProvider.VerifyNoOtherCalls();
            }
        }
    }
}
