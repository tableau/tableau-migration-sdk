using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Tableau.Migration.Tests
{
    internal class BaseUriComparer : IEqualityComparer<Uri>
    {
        public static BaseUriComparer Instance = new();

        public bool Equals(Uri? x, Uri? y)
        {
            if (x is null && y is null)
                return true;

            if (x is null || y is null)
                return false;

            return GetBaseUri(x).Equals(GetBaseUri(y));
        }

        public int GetHashCode([DisallowNull] Uri obj) => GetBaseUri(obj).GetHashCode();

        private static Uri GetBaseUri(Uri uri)
            => new(uri.GetComponents(UriComponents.SchemeAndServer, UriFormat.UriEscaped).ToLower());
    }
}
