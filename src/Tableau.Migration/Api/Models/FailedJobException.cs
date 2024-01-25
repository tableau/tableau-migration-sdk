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
