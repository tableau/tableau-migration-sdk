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

namespace Tableau.Migration.Net
{
    /// <summary>
    /// Interface for <see cref="IHttpRequestBuilder"/> factories.
    /// </summary>
    public interface IHttpRequestBuilderFactory
    {
        /// <summary>
        /// Creates a new <see cref="IHttpDeleteRequestBuilder"/> instance.
        /// </summary>
        /// <param name="uri">The URI to use for the request.</param>
        /// <returns>A new <see cref="IHttpDeleteRequestBuilder"/> instance.</returns>
        IHttpDeleteRequestBuilder CreateDeleteRequest(Uri uri);

        /// <summary>
        /// Creates a new <see cref="IHttpGetRequestBuilder"/> instance.
        /// </summary>
        /// <param name="uri">The URI to use for the request.</param>
        /// <returns>A new <see cref="IHttpGetRequestBuilder"/> instance.</returns>
        IHttpGetRequestBuilder CreateGetRequest(Uri uri);

        /// <summary>
        /// Creates a new <see cref="IHttpPatchRequestBuilder"/> instance.
        /// </summary>
        /// <param name="uri">The URI to use for the request.</param>
        /// <returns>A new <see cref="IHttpPatchRequestBuilder"/> instance.</returns>
        IHttpPatchRequestBuilder CreatePatchRequest(Uri uri);

        /// <summary>
        /// Creates a new <see cref="IHttpPostRequestBuilder"/> instance.
        /// </summary>
        /// <param name="uri">The URI to use for the request.</param>
        /// <returns>A new <see cref="IHttpPostRequestBuilder"/> instance.</returns>
        IHttpPostRequestBuilder CreatePostRequest(Uri uri);

        /// <summary>
        /// Creates a new <see cref="IHttpPutRequestBuilder"/> instance.
        /// </summary>
        /// <param name="uri">The URI to use for the request.</param>
        /// <returns>A new <see cref="IHttpPutRequestBuilder"/> instance.</returns>
        IHttpPutRequestBuilder CreatePutRequest(Uri uri);
    }
}