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

namespace Tableau.Migration.Config
{
    /// <summary>
    /// Options related to jobs.
    /// </summary>
    public class JobOptions
    {
        /// <summary>
        /// Defaults for job options.
        /// </summary>
        public static class Defaults
        {
            /// <summary>
            /// The rate to poll for asynchronous job completion.
            /// </summary>
            public static readonly TimeSpan JOB_POLL_RATE = TimeSpan.FromSeconds(3);

            /// <summary>
            /// The default waiting for job timeout. 30 minutes as Default.
            /// </summary>
            public readonly static TimeSpan JOB_TIMEOUT = TimeSpan.FromMinutes(30);
        }

        /// <summary>
        /// Get or sets the rate to poll for asynchronous job completion.
        /// </summary>
        public TimeSpan JobPollRate
        {
            get => _jobPollRate ?? Defaults.JOB_POLL_RATE;
            set => _jobPollRate = value;
        }
        private TimeSpan? _jobPollRate;

        /// <summary>
        /// Indicates the waiting for a job to finish timeout. The default value is 30 minutes.
        /// </summary>
        public TimeSpan JobTimeout
        {
            get => _jobTimeout ?? Defaults.JOB_TIMEOUT;
            set => _jobTimeout = value;
        }
        private TimeSpan? _jobTimeout;
    }
}
