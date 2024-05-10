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

using System.Net.Http;
using Moq;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Net;
using Xunit;

namespace Tableau.Migration.Tests
{
    public static class IHttpResponseMessageExtensions
    {
        public static bool IsSuccessStatusCode(this IHttpResponseMessage response)
            => response.StatusCode.IsSuccessStatusCode();

        public static bool IsSuccessStatusCode<TResponse>(this Mock<TResponse> mockResponse)
            where TResponse : class, IHttpResponseMessage
            => IsSuccessStatusCode(mockResponse.Object);

        public static TResponse EnsureSuccessStatusCode<TResponse>(this TResponse response)
            where TResponse : IHttpResponseMessage
        {
            using var tempResponse = new HttpResponseMessage(response.StatusCode);
            tempResponse.EnsureSuccessStatusCode();
            return response;
        }

        public static TResponse EnsureSuccessStatusCode<TResponse>(this Mock<TResponse> mockResponse)
            where TResponse : class, IHttpResponseMessage
            => EnsureSuccessStatusCode(mockResponse.Object);

        public static TContent EnsureDeserializedContent<TContent>(this IHttpResponseMessage<TContent> response)
            where TContent : TableauServerResponse
        {
            Assert.NotNull(response.DeserializedContent);
            return response.DeserializedContent;
        }

        public static TItem EnsureItem<TContent, TItem>(this IHttpResponseMessage<TContent> response)
            where TContent : class, ITableauServerResponse<TItem>
            where TItem : class
        {
            Assert.NotNull(response.DeserializedContent?.Item);
            return response.DeserializedContent.Item;
        }
    }
}
