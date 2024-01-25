using System;
using Tableau.Migration.Content.Permissions;

namespace Tableau.Migration.Api.Rest
{
    /// <summary>
    /// An interface for building REST permission-related URIs.
    /// </summary>
    public interface IPermissionsUriBuilder
    {
        /// <summary>
        /// Gets the prefix of the URI (i.e. "projects" in /api/{api-version}/sites/{site-id}/projects/{project-id}/permissions).
        /// </summary>
        string Prefix { get; }

        /// <summary>
        /// Gets the suffix of the URI (i.e. "permissions" in /api/{api-version}/sites/{site-id}/projects/{project-id}/permissions).
        /// </summary>
        string Suffix { get; }

        /// <summary>
        /// Builds the URI for a permission operation. Use <see cref="BuildDeleteUri(Guid, ICapability, GranteeType, Guid)"/> for delete operations.
        /// </summary>
        /// <param name="contentItemId">The ID of the content item.</param>
        /// <returns>The URI string for the permissions URI.</returns>
        string BuildUri(Guid contentItemId);

        /// <summary>
        /// Builds the URI for a permission delete operation. Use <see cref="BuildUri(Guid)"/> for non-delete operations.
        /// </summary>
        /// <param name="contentItemId">The ID of the content item.</param>
        /// <param name="capability">The capability to delete.</param>
        /// <param name="granteeType">The type of grantee for the capability to delete.</param>
        /// <param name="granteeId">The ID of the grantee for the capability to delete.</param>
        /// <returns>The URI string for the permissions URI.</returns>
        string BuildDeleteUri(
            Guid contentItemId,
            ICapability capability,
            GranteeType granteeType,
            Guid granteeId);
    }
}