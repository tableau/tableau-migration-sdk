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

using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Api
{
    /// <summary>
    /// Interface for an API client that can pull information to publish with.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    /// <typeparam name="TPublish">The publish type.</typeparam>
    public interface IPullApiClient<TContent, TPublish>
        where TPublish : class
    {
        /// <summary>
        /// Pulls enough information to publish the content item.
        /// </summary>
        /// <param name="contentItem">The content item to pull.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The result of the pull operation with the item to publish.</returns>
        Task<IResult<TPublish>> PullAsync(TContent contentItem, CancellationToken cancel);
    }
}
