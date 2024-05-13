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
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest.Models;

namespace Tableau.Migration.Net
{
    /// <summary>
    /// Interface for an object that can serialize HTTP content to and from common Tableau API formats.
    /// </summary>
    public interface IHttpContentSerializer
    {
        /// <summary>
        /// Deserializes <see cref="HttpContent"/>.
        /// </summary>
        /// <typeparam name="TContent">The content's deserialized type.</typeparam>
        /// <param name="content">The content to deserialize.</param>
        /// <param name="cancel">The cancellation token.</param>
        /// <returns>The deserialized content.</returns>
        Task<TContent?> DeserializeAsync<TContent>(HttpContent content, CancellationToken cancel);

        /// <summary>
        /// Serializes content for a <see cref="HttpRequestMessage"/>.
        /// </summary>
        /// <typeparam name="TContent">The request's content type.</typeparam>
        /// <param name="content">The <typeparamref name="TContent"/> instance to serialize for the request content.</param>
        /// <param name="contentType">The serialized content type.</param>
        StringContent? Serialize<TContent>(TContent content, MediaTypeWithQualityHeaderValue contentType)
            where TContent : class;

        /// <summary>
        /// Deserializes a Tableau Server <see cref="Error"/> if it exists in the content.
        /// </summary>
        /// <param name="content">The HTTP content to deserialize.</param>
        /// <param name="cancel">The cancellation token.</param>
        /// <returns>The deserialized error if found in the content, null otherwise.</returns>
        Task<Error?> TryDeserializeErrorAsync(HttpContent content, CancellationToken cancel);
    }
}