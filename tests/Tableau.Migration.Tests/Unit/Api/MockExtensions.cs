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
using System.Linq.Expressions;
using System.Threading;
using Moq;
using Tableau.Migration.Net;
using Tableau.Migration.Net.Rest;

namespace Tableau.Migration.Tests.Unit.Api
{
    public static class MockExtensions
    {
        #region - IRestRequestBuilder Extensions -

        public static Mock<TRequestBuilder> SetupHttpRequest<TRequestBuilder>(
            this Mock<IRestRequestBuilder> mockBuilder,
            Expression<Func<IRestRequestBuilder, TRequestBuilder>> getHttpRequestBuilder,
            IHttpResponseMessage response,
            CancellationToken cancel)
            where TRequestBuilder : class, IHttpRequestBuilder
        {
            var mockHttpRequestBuilder = new Mock<TRequestBuilder>();
            mockBuilder.Setup(getHttpRequestBuilder).Returns(mockHttpRequestBuilder.Object);
            mockHttpRequestBuilder.Setup(b => b.SendAsync(cancel)).ReturnsAsync(response);
            return mockHttpRequestBuilder;
        }

        public static Mock<TRequestBuilder> SetupHttpRequest<TRequestBuilder, TResponse>(
            this Mock<IRestRequestBuilder> mockBuilder,
            Expression<Func<IRestRequestBuilder, TRequestBuilder>> getHttpRequestBuilder,
            IHttpResponseMessage<TResponse> response,
            CancellationToken cancel)
            where TRequestBuilder : class, IHttpRequestBuilder
            where TResponse : class
        {
            var mockHttpRequestBuilder = new Mock<TRequestBuilder>();
            mockBuilder.Setup(getHttpRequestBuilder).Returns(mockHttpRequestBuilder.Object);
            mockHttpRequestBuilder.Setup(b => b.SendAsync<TResponse>(cancel)).ReturnsAsync(response);
            return mockHttpRequestBuilder;
        }

        public static Mock<IHttpGetRequestBuilder> SetupGetRequest<TResponse>(
            this Mock<IRestRequestBuilder> mockBuilder,
            IHttpResponseMessage<TResponse> response,
            CancellationToken cancel)
            where TResponse : class
            => mockBuilder.SetupHttpRequest(b => b.ForGetRequest(), response, cancel);

        public static Mock<IHttpGetRequestBuilder> SetupGetRequest(
            this Mock<IRestRequestBuilder> mockBuilder,
            IHttpResponseMessage response,
            CancellationToken cancel)
            => mockBuilder.SetupHttpRequest(b => b.ForGetRequest(), response, cancel);

        public static Mock<IHttpPostRequestBuilder> SetupPostRequest<TResponse>(
            this Mock<IRestRequestBuilder> mockBuilder,
            IHttpResponseMessage<TResponse> response,
            CancellationToken cancel)
            where TResponse : class
            => mockBuilder.SetupHttpRequest(b => b.ForPostRequest(), response, cancel);

        public static Mock<IHttpPostRequestBuilder> SetupPostRequest(
            this Mock<IRestRequestBuilder> mockBuilder,
            IHttpResponseMessage response,
            CancellationToken cancel)
            => mockBuilder.SetupHttpRequest(b => b.ForPostRequest(), response, cancel);

        #endregion
    }
}
