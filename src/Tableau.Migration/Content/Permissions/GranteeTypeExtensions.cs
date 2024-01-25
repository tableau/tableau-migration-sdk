using System;

namespace Tableau.Migration.Content.Permissions
{
    internal static class GranteeTypeExtensions
    {
        public static string ToUrlSegment(this GranteeType granteeType)
        {
            return granteeType switch
            {
                GranteeType.User => "users",
                GranteeType.Group => "groups",
                _ => throw new NotSupportedException($"Could not determine URL segment for {granteeType}.")
            };
        }
    }
}
