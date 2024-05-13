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
    /// Interface for classes that can build HTTP requests.
    /// </summary>
    public interface IRequestBuilder
    {
        /// <summary>
        /// Creates a new <see cref="IHttpDeleteRequestBuilder"/> instance.
        /// </summary>
        /// <returns>A new <see cref="IHttpDeleteRequestBuilder"/> instance.</returns>
        IHttpDeleteRequestBuilder ForDeleteRequest();

        /// <summary>
        /// Creates a new <see cref="IHttpGetRequestBuilder"/> instance.
        /// </summary>
        /// <returns>A new <see cref="IHttpGetRequestBuilder"/> instance.</returns>
        IHttpGetRequestBuilder ForGetRequest();

        /// <summary>
        /// Creates a new <see cref="IHttpPatchRequestBuilder"/> instance.
        /// </summary>
        /// <returns>A new <see cref="IHttpPatchRequestBuilder"/> instance.</returns>
        IHttpPatchRequestBuilder ForPatchRequest();

        /// <summary>
        /// Creates a new <see cref="IHttpPostRequestBuilder"/> instance.
        /// </summary>
        /// <returns>A new <see cref="IHttpPostRequestBuilder"/> instance.</returns>
        IHttpPostRequestBuilder ForPostRequest();

        /// <summary>
        /// Creates a new <see cref="IHttpPutRequestBuilder"/> instance.
        /// </summary>
        /// <returns>A new <see cref="IHttpPutRequestBuilder"/> instance.</returns>
        IHttpPutRequestBuilder ForPutRequest();
    }

    /// <summary>
    /// Interface for classes that can build URIs
    /// </summary>
    public interface IRequestBuilder<TBuilder> : IRequestBuilder
        where TBuilder : IRequestBuilder<TBuilder>
    {
        /// <summary>
        /// Configures the query for the URI.
        /// </summary>
        /// <param name="query">The callback used to build the URI's query string.</param>
        /// <returns>The current <typeparamref name="TBuilder"/> instance.</returns>
        TBuilder WithQuery(Action<IQueryStringBuilder> query);

        /// <summary>
        /// Configures the query for the URI.
        /// </summary>
        /// <param name="key">The query string key.</param>
        /// <param name="value">The query string value.</param>
        /// <returns>The current <typeparamref name="TBuilder"/> instance.</returns>
        TBuilder WithQuery(string key, string value);
    }
}