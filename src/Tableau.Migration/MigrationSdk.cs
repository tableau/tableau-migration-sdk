using System;
using System.Reflection;

namespace Tableau.Migration
{
    /// <summary>
    /// Implementation responsible to return the current SDK version and user agent string, based on the Executing Assembly Version.
    /// </summary>
    internal sealed class MigrationSdk : IMigrationSdk
    {
        private readonly IUserAgentSuffixProvider _userAgentSuffixProvider;

        /// <summary>
        /// Sets the <see cref="MigrationSdk"/> properties on initialization so they are immutable.
        /// </summary>
        public MigrationSdk(IUserAgentSuffixProvider userAgentSuffixProvider)
        {
            Version = Assembly.GetExecutingAssembly().GetName().Version ?? new Version();
            _userAgentSuffixProvider = userAgentSuffixProvider;

            UserAgent = $"{Constants.USER_AGENT_PREFIX}{_userAgentSuffixProvider.UserAgentSuffix}/{Version}";
        }
        /// <summary>
        /// The current SDK Version
        /// </summary>
        /// <returns>The current SDK version.</returns>
        public Version Version { get; init; }
        /// <summary>
        /// Identifier string for the SDK user-agent.
        /// </summary>

        public string UserAgent { get; init; }
    }
}
