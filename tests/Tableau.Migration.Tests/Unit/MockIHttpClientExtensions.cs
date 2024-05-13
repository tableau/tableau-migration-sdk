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
using System.Threading;
using Moq;
using Tableau.Migration.Net;

namespace Tableau.Migration.Tests.Unit
{
    public static class MockIHttpClientExtensions
    {
        public static void SetupResponse<TResponse>(
            this Mock<IHttpClient> mockHttpClient,
            IHttpResponseMessage<TResponse> response,
            Action<HttpRequestMessage>? onRequestSent = null)
            where TResponse : class
        {
            var returns = mockHttpClient
                .Setup(c => c.SendAsync<TResponse>(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            if (onRequestSent is not null)
                returns.Callback((HttpRequestMessage request, CancellationToken cancel) => onRequestSent.Invoke(request));
        }

        public static void SetupResponse(
            this Mock<IHttpClient> mockHttpClient,
            IHttpResponseMessage response,
            Action<HttpRequestMessage>? onRequestSent = null)
        {
            var returns = mockHttpClient
                .Setup(c => c.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            if (onRequestSent is not null)
                returns.Callback((HttpRequestMessage request, CancellationToken cancel) => onRequestSent.Invoke(request));

            returns = mockHttpClient
                .Setup(c => c.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<HttpCompletionOption>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            if (onRequestSent is not null)
                returns.Callback((HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancel) => onRequestSent.Invoke(request));

        }
    }
}
