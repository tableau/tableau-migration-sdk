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

using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Net;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    internal class EmptyRestResponseBuilder : IRestApiResponseBuilder
    {
        private static readonly UnauthorizedRestErrorBuilder _unauthorizedErrorBuilder = new();

        private readonly IHttpContentSerializer _serializer;

        protected TableauData Data { get; }

        public bool RequiresAuthentication { get; }

        public IRestErrorBuilder? ErrorOverride { get; set; }

        public EmptyRestResponseBuilder(TableauData data, IHttpContentSerializer serializer, bool requiresAuthentication)
        {
            Data = data;
            _serializer = serializer;
            RequiresAuthentication = requiresAuthentication;
        }

        protected virtual Task<HttpResponseMessage> BuildResponseAsync(HttpRequestMessage request, CancellationToken cancel)
            => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));

        protected HttpResponseMessage BuildErrorResponse(HttpRequestMessage request, IRestErrorBuilder errorBuilder)
        {
            var contentType = request.Headers.Accept.FirstOrDefault() ?? MediaTypes.Xml;

            var tsResponse = new EmptyTableauServerResponse
            {
                Error = errorBuilder.BuildError(out var statusCode)
            };

            return new HttpResponseMessage(statusCode)
            {
                Content = _serializer.Serialize(tsResponse, contentType)
            };
        }

        public async Task<HttpResponseMessage> RespondAsync(HttpRequestMessage request, CancellationToken cancel)
        {
            if (ErrorOverride is not null)
            {
                return BuildErrorResponse(request, ErrorOverride);
            }
            else
            {
                if (this.IsUnauthorizedRequest(request, Data))
                {
                    return BuildErrorResponse(request, _unauthorizedErrorBuilder);
                }
            }

            return await BuildResponseAsync(request, cancel).ConfigureAwait(false);
        }
    }
}
