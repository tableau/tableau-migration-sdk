using System;

namespace Tableau.Migration.Net.Simulation
{
    /// <summary>
    /// Static class containing extension methods for <see cref="Uri"/> objects.
    /// </summary>
    public static class UriExtensions
    {
        /// <summary>
        /// Returns the absolute path of the URI, trimmed of any final path separator.
        /// </summary>
        /// <param name="uri">The URI to get the trimmed path for.</param>
        /// <returns>The trimmed path.</returns>
        public static string TrimmedPath(this Uri uri) => uri.AbsolutePath.TrimEnd('/');
    }
}
