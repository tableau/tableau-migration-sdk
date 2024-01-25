using System;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Api.Models
{
    /// <summary>
    /// Class representing a Tableau Job that timed out while waiting to finish
    /// </summary>
    public class TimeoutJobException : Exception
    {
        /// <summary>
        /// Gets the job that timed out. May be null if no job status was ever reported.
        /// </summary>
        public IJob? Job { get; }

        /// <summary>
        /// Creates a new <see cref="TimeoutJobException"/> object
        /// </summary>
        /// <param name="job">The last job status that timed out. May be null if no job status was ever reported.</param>
        /// <param name="sharedResourcesLocalizer">A string localizer.</param>
        public TimeoutJobException(IJob? job, ISharedResourcesLocalizer sharedResourcesLocalizer)
            : base(sharedResourcesLocalizer[SharedResourceKeys.TimeoutJobExceptionMessage])
        {
            Job = job;
        }
    }
}
