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
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Net;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    internal class RestSingleEntityResponseBuilder<TResponse, TResponseItem> : RestApiResponseBuilderBase<TResponse>
        where TResponse : TableauServerResponse, ITableauServerResponse<TResponseItem>, new()
    {
        private readonly Func<TableauData, HttpRequestMessage, TResponseItem?> _getEntity;

        public RestSingleEntityResponseBuilder(
            TableauData data,
            IHttpContentSerializer serializer,
            Func<TableauData, HttpRequestMessage, TResponseItem?> getEntity,
            bool requiresAuthentication)
            : base(data, serializer, requiresAuthentication)
        {
            _getEntity = getEntity;
        }

        protected virtual void ChangeEntity(TResponseItem entity)
        { }

        protected virtual (TResponse Response, HttpStatusCode ResponseCode) BuildEntityResponse(TResponseItem entity)
        {
            var response = new TResponse()
            {
                Item = entity
            };

            return (response, HttpStatusCode.OK);
        }

        protected override ValueTask<(TResponse Response, HttpStatusCode ResponseCode)> BuildResponseAsync(HttpRequestMessage request, CancellationToken cancel)
        {
            var entity = _getEntity(Data, request);

            if (entity is null)
            {
                return BuildEmptyErrorResponseAsync(HttpStatusCode.NotFound, 0, "Not Found Summary", "Not Found Detail");
            }
            else
            {
                ChangeEntity(entity);
                return ValueTask.FromResult(BuildEntityResponse(entity));
            }
        }
    }
}
