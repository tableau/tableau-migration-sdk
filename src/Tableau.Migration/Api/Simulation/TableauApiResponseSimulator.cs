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
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Net;
using Tableau.Migration.Net.Simulation;
using Tableau.Migration.Net.Simulation.Requests;
using Tableau.Migration.Net.Simulation.Responses;

namespace Tableau.Migration.Api.Simulation
{
    /// <summary>
    /// Default <see cref="IResponseSimulator"/> implementation.
    /// </summary>
    public class TableauApiResponseSimulator : IResponseSimulator
    {
        private static readonly EmptyResponseBuilder NotFoundResponse = new(HttpStatusCode.NotFound);

        private readonly Dictionary<IRequestMatcher, MethodSimulator> _methodSimulators = new();

        /// <summary>
        /// Gets the base URL to match requests on.
        /// </summary>
        public Uri BaseUrl { get; }

        /// <summary>
        /// Gets the simulated data.
        /// </summary>
        public TableauData Data { get; }

        /// <summary>
        /// Gets the serializer to use for responses.
        /// </summary>
        public IHttpContentSerializer Serializer { get; }

        /// <summary>
        /// Creates a new <see cref="TableauApiResponseSimulator"/> object.
        /// </summary>
        /// <param name="baseUrl">The base URL to respond to.</param>
        /// <param name="data">The simulated data.</param>
        /// <param name="serializer">A serializer to use.</param>
        public TableauApiResponseSimulator(Uri baseUrl, TableauData data, IHttpContentSerializer serializer)
        {
            BaseUrl = baseUrl;
            Data = data;
            Serializer = serializer;
        }

        /// <summary>
        /// Configures a simulated response.
        /// </summary>
        /// <param name="httpMethod">The HTTP method to match.</param>
        /// <param name="requestUrl">The request URL to match.</param>
        /// <param name="response">The response builder to use.</param>
        /// <returns>This response simulator, for fluent API usage.</returns>
        public TableauApiResponseSimulator SetupResponse(HttpMethod httpMethod, Uri requestUrl, IResponseBuilder response)
            => SetupMethod(new(new PathRequestMatcher(httpMethod, requestUrl), response));

        /// <summary>
        /// Configures the simulated response for an API method.
        /// </summary>
        /// <param name="method">The simulated API method.</param>
        /// <returns>This response simulator, for fluent API usage.</returns>
        public TableauApiResponseSimulator SetupMethod(MethodSimulator method)
        {
            _methodSimulators[method.RequestMatcher] = method;
            return this;
        }

        /// <inheritdoc />
        public async Task<HttpResponseMessage> RespondAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            foreach (var methodSimulator in _methodSimulators.Values)
            {
                if (methodSimulator.RequestMatcher.Matches(request))
                {
                    if (methodSimulator.ResponseOverride is not null)
                    {
                        return await methodSimulator.ResponseOverride.RespondAsync(request, cancellationToken).ConfigureAwait(false);
                    }

                    return await methodSimulator.ResponseBuilder.RespondAsync(request, cancellationToken).ConfigureAwait(false);
                }
            }

            return await NotFoundResponse.RespondAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
