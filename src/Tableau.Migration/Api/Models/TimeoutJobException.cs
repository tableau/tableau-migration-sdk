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
    /// Class representing a Tableau Job that timed out while waiting to finish
    /// </summary>
    public class TimeoutJobException : Exception, IEquatable<TimeoutJobException>
    {
        /// <summary>
        /// Gets the job that timed out. May be null if no job status was ever reported.
        /// </summary>
        public IJob? Job { get; }

        /// <summary>
        /// Creates a new <see cref="TimeoutJobException"/> object
        /// </summary>
        /// <remarks>This should only be used for deserialization.</remarks>
        /// <param name="job">The last job status that timed out. May be null if no job status was ever reported.</param>
        /// <param name="exceptionMessage">Message of the base exception</param>
        internal TimeoutJobException(IJob? job, string exceptionMessage)
            : base(exceptionMessage)
        {
            Job = job;
        }

        /// <summary>
        /// Creates a new <see cref="TimeoutJobException"/> object
        /// </summary>
        /// <param name="job">The last job status that timed out. May be null if no job status was ever reported.</param>
        /// <param name="sharedResourcesLocalizer">A string localizer.</param>
        public TimeoutJobException(IJob? job, ISharedResourcesLocalizer sharedResourcesLocalizer)
            : this(job, sharedResourcesLocalizer[SharedResourceKeys.TimeoutJobExceptionMessage])
        { }

        #region - IEquatable -

        ///<inheritdoc/>
        public override bool Equals(object? obj)
        {
            return Equals(obj as TimeoutJobException);
        }

        ///<inheritdoc/>
        public bool Equals(TimeoutJobException? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            // Now directly using IJob's Equals method for comparison.
            return Job?.Equals(other.Job) ?? other.Job == null;
        }

        ///<inheritdoc/>
        public override int GetHashCode()
        {
            return Job?.GetHashCode() ?? 0;
        }

        #endregion
    }
}
