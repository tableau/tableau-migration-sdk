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
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Content;
using Tableau.Migration.Paging;

namespace Tableau.Migration.Api.Rest.Models
{
    /// <summary>
    /// Pager implementation that pages group user members response objects
    /// for a given group.
    /// </summary>
    internal sealed class GroupUsersResponsePager
        : IndexedPagerBase<IUser>, IPager<IUser>
    {
        private readonly IGroupsApiClient _apiClient;
        private readonly Guid _groupId;

        public GroupUsersResponsePager(
            IGroupsApiClient apiClient,
            Guid groupId,
            int pageSize)
            : base(pageSize)
        {
            _apiClient = apiClient;
            _groupId = groupId;
        }

        protected override async Task<IPagedResult<IUser>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancel)
            => await _apiClient.GetGroupUsersAsync(_groupId, pageNumber, pageSize, cancel).ConfigureAwait(false);
    }
}
