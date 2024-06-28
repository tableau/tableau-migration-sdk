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
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Models;

namespace Tableau.Migration.Api
{
    /// <summary>
    /// Interface for API client job operations.
    /// </summary>
    public interface IJobsApiClient : IContentApiClient
    {
        /// <summary>
        /// Gets the status for a given job ID.
        /// </summary>
        /// <param name="jobId">The job's ID.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The status for the given job ID.</returns>
        Task<IResult<IJob>> GetJobStatusAsync(Guid jobId, CancellationToken cancel);

        /// <summary>
        /// Waits for the job with the given ID to complete.
        /// </summary>
        /// <param name="jobId">The job's ID.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The status for the given job ID.</returns>
        Task<IResult> WaitForJobAsync(Guid jobId, CancellationToken cancel);
    }
}
