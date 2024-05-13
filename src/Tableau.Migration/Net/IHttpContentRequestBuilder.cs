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

namespace Tableau.Migration.Net
{
    /// <summary>
    /// Interface for a class that can build HTTP content requests.
    /// </summary>
    public interface IHttpContentRequestBuilder<TBuilderInterface> : IHttpRequestBuilder<TBuilderInterface>
    {
        /// <summary>
        /// Sets the content for the request.
        /// </summary>
        /// <param name="content">The request's content.</param>
        /// <returns>The current <typeparamref name="TBuilderInterface"/> instance.</returns>
        TBuilderInterface WithContent(HttpContent content);

        /// <summary>
        /// Sets the JSON content for the request.
        /// </summary>
        /// <param name="content">The request's content.</param>
        /// <returns>The current <typeparamref name="TBuilderInterface"/> instance.</returns>
        TBuilderInterface WithJsonContent<TContent>(TContent content)
            where TContent : class;

        /// <summary>
        /// Sets the XML content for the request.
        /// </summary>
        /// <param name="content">The request's content.</param>
        /// <returns>The current <typeparamref name="TBuilderInterface"/> instance.</returns>
        TBuilderInterface WithXmlContent<TContent>(TContent content)
            where TContent : class;

        /// <summary>
        /// Sets the content for the request.
        /// </summary>
        /// <typeparam name="TContent">The content's deserialized type.</typeparam>
        /// <param name="content">The request's content.</param>
        /// <param name="contentType">The request's content type.</param>
        /// <returns>The current <typeparamref name="TBuilderInterface"/> instance.</returns>
        TBuilderInterface WithContent<TContent>(TContent content, MediaTypeWithQualityHeaderValue contentType)
            where TContent : class;

        /// <summary>
        /// Sets the JSON content for the request.
        /// </summary>
        /// <param name="content">The request's content.</param>
        /// <returns>The current <typeparamref name="TBuilderInterface"/> instance.</returns>
        TBuilderInterface WithJsonContent(string content);

        /// <summary>
        /// Sets the XML content for the request.
        /// </summary>
        /// <param name="content">The request's content.</param>
        /// <returns>The current <typeparamref name="TBuilderInterface"/> instance.</returns>
        TBuilderInterface WithXmlContent(string content);

        /// <summary>
        /// Sets the content for the request.
        /// </summary>
        /// <param name="content">The request's content.</param>
        /// <param name="contentType">The request's content type.</param>
        /// <returns>The current <typeparamref name="TBuilderInterface"/> instance.</returns>
        TBuilderInterface WithContent(string content, MediaTypeWithQualityHeaderValue contentType);
    }
}
