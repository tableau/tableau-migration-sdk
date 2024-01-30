// Copyright (c) 2023, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the ""License"") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an ""AS IS"" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using System;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Api.Models
{
    /// <summary>
    /// Class representing a failed Tableau job.
    /// </summary>
    public class FailedJobException : Exception
    {
        /// <summary>
        /// Gets the failed job.
        /// </summary>
        public IJob FailedJob { get; }

        /// <summary>
        /// Creates a new <see cref="FailedJobException"/> object
        /// </summary>
        /// <param name="job">The failed job.</param>
        /// <param name="sharedResourcesLocalizer">A string localizer.</param>
        public FailedJobException(IJob job, ISharedResourcesLocalizer sharedResourcesLocalizer)
            : base(sharedResourcesLocalizer[SharedResourceKeys.FailedJobExceptionContent])
        {
            FailedJob = job;
        }
    }
}
