//
//  Copyright (c) 2025, Salesforce, Inc.
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
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest.Models.Requests;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Simulation.Rest.Net.Requests;
using Tableau.Migration.Net;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    internal class RestRequestResponseBuilder<TResponse> : RestResponseBuilderBase<TResponse>
        where TResponse : TableauServerResponse, new()
    {
        private readonly Func<TableauData, HttpRequestMessage, (TResponse Response, HttpStatusCode ResponseCode)> _buildResponse;

        public RestRequestResponseBuilder(TableauData data, IHttpContentSerializer serializer, 
            Func<TableauData, HttpRequestMessage, (TResponse Response, HttpStatusCode ResponseCode)> buildResponse,
            bool requiresAuthentication) 
            : base(data, serializer, requiresAuthentication)
        {
            _buildResponse = buildResponse;
        }

        public RestRequestResponseBuilder(TableauData data, IHttpContentSerializer serializer,
            Func<TableauData, HttpRequestMessage, TResponse> buildResponse,
            bool requiresAuthentication)
            : this(data, serializer, (d, r) => (buildResponse(d, r), HttpStatusCode.OK), requiresAuthentication)
        { }

        protected override ValueTask<(TResponse Response, HttpStatusCode ResponseCode)> BuildResponseAsync(HttpRequestMessage request, CancellationToken cancel)
        {
            return ValueTask.FromResult(_buildResponse(Data, request));
        }
    }

    internal class RestRequestResponseBuilder<TRequest, TResponse> : RestRequestResponseBuilder<TResponse>
        where TRequest : TableauServerRequest
        where TResponse : TableauServerResponse, new()
    {
        public RestRequestResponseBuilder(TableauData data, IHttpContentSerializer serializer,
            Func<TableauData, TRequest, (TResponse Response, HttpStatusCode ResponseCode)> buildResponse,
            bool requiresAuthentication)
            : base(data, serializer, ResponseBuilder(buildResponse), requiresAuthentication)
        { }

        public RestRequestResponseBuilder(TableauData data, IHttpContentSerializer serializer,
            Func<TableauData, TRequest, TResponse> buildResponse,
            bool requiresAuthentication)
            : base(data, serializer, ResponseBuilder((d, r) => (buildResponse(d, r), HttpStatusCode.OK)), requiresAuthentication)
        { }

        private static Func<TableauData, HttpRequestMessage, (TResponse Response, HttpStatusCode ResponseCode)> ResponseBuilder(Func<TableauData, TRequest, (TResponse Response, HttpStatusCode ResponseCode)> buildResponse)
        {
            return (TableauData data, HttpRequestMessage request) =>
            {
                var typedRequest = request.GetTableauServerRequest<TRequest>();
                if(typedRequest is null)
                {
                    return BuildEmptyErrorResponse(HttpStatusCode.BadRequest, 0, $"Request must be of the type {nameof(TRequest)} and not null", string.Empty);
                }

                var result = buildResponse(data, typedRequest);
                return result;
            };
        }
    }
}
