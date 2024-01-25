using System;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Content.Permissions;
using Tableau.Migration.Net.Rest;

namespace Tableau.Migration.Api
{
    internal static class IRestRequestBuilderFactoryExtensions
    {
        public static IRestRequestBuilder CreatePermissionsUri(
            this IRestRequestBuilderFactory requestBuilderFactory,
            IPermissionsUriBuilder permissionsUriBuilder,
            Guid contentItemId)
        {
            return requestBuilderFactory.CreateUri(permissionsUriBuilder.BuildUri(contentItemId));
        }

        public static IRestRequestBuilder CreatePermissionsDeleteUri(
            this IRestRequestBuilderFactory requestBuilderFactory,
            IPermissionsUriBuilder permissionsUriBuilder,
            Guid contentItemId,
            ICapability capability,
            GranteeType granteeType,
            Guid granteeId)
        {
            return requestBuilderFactory.CreateUri(permissionsUriBuilder.BuildDeleteUri(contentItemId, capability, granteeType, granteeId));
        }
    }
}
