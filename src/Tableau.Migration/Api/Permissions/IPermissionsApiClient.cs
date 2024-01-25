using System;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Content.Permissions;

namespace Tableau.Migration.Api.Permissions
{
    /// <summary>
    /// Interface for API client content item permissions operations.
    /// </summary>
    public interface IPermissionsApiClient
    {
        /// <summary>
        /// Gets the permissions for the content item with the specified ID.
        /// </summary>
        /// <param name="contentItemId">The ID of the content item.</param>
        /// <param name="cancel">The cancellation token.</param>
        /// <returns>The permissions result with <see cref="IPermissions"/>.</returns>
        Task<IResult<IPermissions>> GetPermissionsAsync(
            Guid contentItemId,
            CancellationToken cancel);

        /// <summary>
        /// Creates the permissions for the content item with the specified ID.
        /// </summary>
        /// <param name="contentItemId">The id of the content item.</param>
        /// <param name="permissions">The permissions of the content item.</param>
        /// <param name="cancel">The cancellation token.</param>
        /// <returns>The permissions result with <see cref="IPermissions"/>.</returns>
        Task<IResult<IPermissions>> CreatePermissionsAsync(
            Guid contentItemId,
            IPermissions permissions,
            CancellationToken cancel);

        /// <summary>
        /// Remove a <see cref="ICapability"/> for a user/group on a content item.
        /// </summary>
        /// <param name="contentItemId">Id of the content item.</param>
        /// <param name="granteeId">The id of the permissions grantee. This is either a user or group.</param>
        /// <param name="granteeType">The type of the permissions grantee.</param>
        /// <param name="capability">The object containing the capability name and mode.</param>
        /// <param name="cancel">A cancellation token to obey.</param>
        /// <returns></returns>
        Task<IResult> DeleteCapabilityAsync(
            Guid contentItemId,
            Guid granteeId,
            GranteeType granteeType,
            ICapability capability,
            CancellationToken cancel);

        /// <summary>
        /// Remove all <paramref name="permissions"/> for a content item.
        /// </summary>
        /// <param name="contentItemId">Id of the content item.</param>        
        /// <param name="permissions"></param>
        /// <param name="cancel"></param>
        /// <returns></returns>
        Task<IResult> DeleteAllPermissionsAsync(
            Guid contentItemId,
            IPermissions permissions,
            CancellationToken cancel);

        /// <summary>
        /// Updates the permissions for the content item with the specified ID.
        /// </summary>
        /// <param name="contentItemId">Id of the content item.</param>
        /// <param name="permissions">The permissions of the content item.</param>
        /// <param name="cancel">The cancellation token.</param>
        /// <returns>The permissions result with <see cref="IPermissions"/>.</returns>
        Task<IResult<IPermissions>> UpdatePermissionsAsync(
            Guid contentItemId,
            IPermissions permissions,
            CancellationToken cancel);
    }
}