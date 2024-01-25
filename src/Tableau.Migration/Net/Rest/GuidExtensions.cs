using System;

namespace Tableau.Migration.Net.Rest
{
    internal static class GuidExtensions
    {
        public static string ToUrlSegment(this Guid guid)
        {
            return guid.ToString("D");
        }
    }
}
