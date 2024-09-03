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
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Models.Cloud;
using Tableau.Migration.Content.Schedules.Cloud;

namespace Tableau.Migration.Api
{
    /// <summary>
    /// Interface for API client Cloud tasks operations.
    /// </summary>
    public interface ICloudTasksApiClient : 
        IContentApiClient,
        IPublishApiClient<ICloudExtractRefreshTask>
    {
        /// <summary>
        /// Gets a list of extract refresh tasks.
        /// </summary>
        /// <param name="cancel">The cancellation token to obey.</param>
        Task<IResult<IImmutableList<ICloudExtractRefreshTask>>> GetAllExtractRefreshTasksAsync(CancellationToken cancel);

        /// <summary>
        /// Create an extract refresh task.
        /// </summary>
        /// <param name="options">The new extract refresh task's details.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The published extract refresh task.</returns>
        Task<IResult<ICloudExtractRefreshTask>> CreateExtractRefreshTaskAsync(
            ICreateExtractRefreshTaskOptions options,
            CancellationToken cancel);

        /// <summary>
        /// Deletes an extract refresh task.
        /// </summary>
        /// <param name="extractRefreshTaskId">The extract refresh task's ID.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns></returns>
        Task<IResult> DeleteExtractRefreshTaskAsync(
            Guid extractRefreshTaskId,
            CancellationToken cancel);
    }
}
