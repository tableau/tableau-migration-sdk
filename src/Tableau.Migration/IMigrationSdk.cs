using System;

namespace Tableau.Migration
{
    /// <summary>
    /// Abstraction responsible to return the current SDK version and user agent string, based on the Executing Assembly Version.
    /// </summary>
    internal interface IMigrationSdk
    {
        /// <summary>
        /// The current SDK Version
        /// </summary>
        /// <returns>The current SDK version.</returns>
        Version Version { get; }
        /// <summary>
        /// Identifier string for the SDK user-agent.
        /// </summary>
        string UserAgent { get; }
    }
}
