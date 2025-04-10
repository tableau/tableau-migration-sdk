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

using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Content;
using Tableau.Migration.Paging;

namespace Tableau.Migration.Api
{
    /// <summary>
    /// Interface for API client server subscriptions operations.
    /// </summary>
    public interface IServerSubscriptionsApiClient : IPagedListApiClient<IServerSubscription>, IApiPageAccessor<IServerSubscription>
    {
        /// <summary>
        /// Gets all subscriptions on the server site.
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The list of subscriptions on the site.</returns>
        Task<IPagedResult<IServerSubscription>> GetAllSubscriptionsAsync(int pageNumber, int pageSize, CancellationToken cancel);
    }
}