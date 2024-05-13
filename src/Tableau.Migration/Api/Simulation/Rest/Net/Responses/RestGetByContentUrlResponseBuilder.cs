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
using System.Linq;
using System.Net.Http;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Simulation.Rest.Net.Requests;
using Tableau.Migration.Net;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    internal class RestGetByContentUrlResponseBuilder<TResponse, TResponseItem> : RestSearchEntityResponseBuilderBase<TResponse, TResponseItem>
        where TResponse : TableauServerResponse, ITableauServerResponse<TResponseItem>, new()
        where TResponseItem : IApiContentUrl
    {
        public RestGetByContentUrlResponseBuilder(
            TableauData data,
            IHttpContentSerializer serializer,
            Func<TableauData, HttpRequestMessage, ICollection<TResponseItem>> getData,
            bool requiresAuthentication)
            : base(data, serializer, getData, requiresAuthentication)
        { }

        protected override TResponseItem? FindEntity(ICollection<TResponseItem> entities, HttpRequestMessage request)
        {
            var contentUrl = request.GetLastSegment();
            return entities.FirstOrDefault(e => String.Equals(e.ContentUrl, contentUrl, StringComparison.Ordinal));
        }
    }
}
