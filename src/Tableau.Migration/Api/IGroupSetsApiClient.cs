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
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Content;

namespace Tableau.Migration.Api
{
    /// <summary>
    /// Interface for API client group set operations.
    /// </summary>
    public interface IGroupSetsApiClient 
        : IContentApiClient, IPagedListApiClient<IGroupSet>, IApiPageAccessor<IGroupSet>, 
        IReadApiClient<IGroupSet>, IPullApiClient<IGroupSet, IPublishableGroupSet>,
        IPublishApiClient<IPublishableGroupSet>
    {
        /// <summary>
        /// Adds a group to a group set.
        /// </summary>
        /// <param name="groupSetId">The group set ID.</param>
        /// <param name="groupId">The group ID.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The result.</returns>
        Task<IResult> AddGroupToGroupSetAsync(Guid groupSetId, Guid groupId, CancellationToken cancel);

        /// <summary>
        /// Removes a group from a group set.
        /// </summary>
        /// <param name="groupSetId">The group set ID.</param>
        /// <param name="groupId">The group ID.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The result.</returns>
        Task<IResult> RemoveGroupFromGroupSetAsync(Guid groupSetId, Guid groupId, CancellationToken cancel);
    }
}
