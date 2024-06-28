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
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Net.Simulation.Responses
{
    /// <summary>
    /// Interface for an object that can build a HTTP response.
    /// </summary>
    public interface IResponseBuilder
    {
        /// <summary>
        /// Gets whether or not an unauthenticated response should be returned if the request is unauthenticated.
        /// </summary>
        bool RequiresAuthentication { get; }

        /// <summary>
        /// Builds a HTTP response for the given request.
        /// </summary>
        /// <param name="request">The HTTP request to respond to.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The HTTP response.</returns>
        Task<HttpResponseMessage> RespondAsync(HttpRequestMessage request, CancellationToken cancel);
    }
}
