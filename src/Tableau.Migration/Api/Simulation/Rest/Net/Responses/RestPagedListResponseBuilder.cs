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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Net;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    internal sealed class RestPagedListResponseBuilder<TResponse, TResponseItem> : RestEntityListResponseBuilderBase<TResponse, TResponseItem>
        where TResponse : PagedTableauServerResponse<TResponseItem>, new()
    {
        private readonly IEntityResponsePager<TResponseItem> _pager;

        public RestPagedListResponseBuilder(
            TableauData data,
            IHttpContentSerializer serializer,
            Func<TableauData, HttpRequestMessage, ICollection<TResponseItem>> getEntities,
            bool requiresAuthentication)
            : base(data, serializer, getEntities, requiresAuthentication)
        {
            _pager = new RestEntityResponsePager<TResponseItem>();
        }

        protected override ValueTask<(TResponse Response, HttpStatusCode ResponseCode)> BuildResponseAsync(HttpRequestMessage request, CancellationToken cancel)
        {
            var filteredData = GetEntities(Data, request);
            var pageOptions = _pager.GetPageOptions(request);

            var response = new TResponse();

            response.Pagination.PageNumber = pageOptions.PageNumber;
            response.Pagination.PageSize = pageOptions.PageSize;
            response.Pagination.TotalAvailable = filteredData.Count;

            var page = _pager.GetPage(filteredData, pageOptions);
            response.Items = page.ToArray();

            return ValueTask.FromResult((response, HttpStatusCode.OK));
        }
    }
}
