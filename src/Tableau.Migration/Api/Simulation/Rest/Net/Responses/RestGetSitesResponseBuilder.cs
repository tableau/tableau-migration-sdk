﻿//
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

using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Net;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    internal class RestGetSitesResponseBuilder : EmptyRestResponseBuilder
    {
        public RestGetSitesResponseBuilder(
            TableauData data,
            IHttpContentSerializer serializer)
            : base(data, serializer, false)
        {
        }

        protected override Task<HttpResponseMessage> BuildResponseAsync(HttpRequestMessage request, CancellationToken cancel)
        {
            if (Data.IsTableauServer)
            {
                return Task.FromResult(new HttpResponseMessage());
            }

            return Task.FromResult(
                BuildErrorResponse(
                    request,
                    new StaticRestErrorBuilder(
                        HttpStatusCode.Forbidden,
                        69,
                        string.Empty,
                        string.Empty)));
        }
    }
}