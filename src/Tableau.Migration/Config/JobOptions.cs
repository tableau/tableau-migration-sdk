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
