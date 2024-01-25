using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Tableau.Migration.Net.Rest
{
    internal static class UriExtensions
    {
        public static bool IsRest([NotNullWhen(true)] this Uri? uri)
        {
            if (uri is null)
                return false;

            var absoluteUri = uri.EnsureAbsoluteUri();

            var segments = absoluteUri.GetNonSlashSegments();

            // At least /api/<version>
            if (segments.Length < 2)
                return false;

            return segments[0].Equals("api", StringComparison.OrdinalIgnoreCase) is true;
        }

        public static bool IsRestSignIn([NotNullWhen(true)] this Uri? uri)
        {
            if (!uri.IsRest())
                return false;

            return uri.EnsureAbsoluteUri().GetNonSlashSegments().Skip(2).Take(2).SequenceEqual(new[] { "auth", "signin" }, StringComparer.OrdinalIgnoreCase) is true;
        }

        public static bool IsRestSignOut([NotNullWhen(true)] this Uri? uri)
        {
            if (!uri.IsRest())
                return false;

            return uri.EnsureAbsoluteUri().GetNonSlashSegments().Skip(2).Take(2).SequenceEqual(new[] { "auth", "signout" }, StringComparer.OrdinalIgnoreCase) is true;
        }

        internal static Uri EnsureAbsoluteUri(this Uri uri)
            => uri.IsAbsoluteUri ? uri : new Uri(new Uri("https://localhost"), uri.ToString());

        internal static string[] GetNonSlashSegments(this Uri uri)
            => uri.Segments.Where(s => s != "/").Select(s => s.TrimPaths()).ToArray();
    }
}
