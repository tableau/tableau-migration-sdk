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
using Tableau.Migration.Resources;

namespace Tableau.Migration.Api.Models
{
    /// <summary>
    /// Class representing a failed Tableau job.
    /// </summary>
    public class FailedJobException : Exception, IEquatable<FailedJobException>
    {
        /// <summary>
        /// Gets the failed job.
        /// </summary>
        public IJob FailedJob { get; }

        /// <summary>
        /// Creates a new <see cref="FailedJobException"/> object
        /// </summary>
        /// <remarks>This should only be used for deserialization.</remarks>
        /// <param name="job">The failed job.</param>
        /// <param name="exceptionMessage">Message for base Exception.</param>
        internal FailedJobException(IJob job, string exceptionMessage)
            : base(exceptionMessage)
        {
            FailedJob = job;
        }

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

        #region - IEquatable -

        /// <inheritdoc/>
        public bool Equals(FailedJobException? other)
        {
            if (other == null) return false;

            // Use IJob's IEquatable implementation for comparison
            return FailedJob?.Equals(other.FailedJob) ?? other.FailedJob == null;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (obj is FailedJobException other)
            {
                return Equals(other);
            }

            return false;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return FailedJob?.GetHashCode() ?? 0;
        }

        #endregion
    }
}
