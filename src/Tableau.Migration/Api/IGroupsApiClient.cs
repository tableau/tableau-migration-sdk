using System;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Content;
using Tableau.Migration.Paging;

namespace Tableau.Migration.Api
{
    /// <summary>
    /// Interface for API client group operations.
    /// </summary>
    public interface IGroupsApiClient :
        IContentApiClient, IPagedListApiClient<IGroup>, IPublishApiClient<IPublishableGroup, IGroup>, IPullApiClient<IGroup, IPublishableGroup>, IApiPageAccessor<IGroup>
    {
        /// <summary>
        /// Creates a local group
        /// </summary>
        /// <param name="name">The new group's name.</param>
        /// <param name="minimumSiteRole">The new group's minimum site role.</param>
        /// <param name="cancel">The cancellation token.</param>
        /// <returns>The newly created group.</returns>
        Task<IResult<IGroup>> CreateLocalGroupAsync(string name, string? minimumSiteRole, CancellationToken cancel);

        /// <summary>
        /// Creates a group from Active Directory and imports the group's users.
        /// </summary>
        /// <param name="name">The new group's name.</param>
        /// <param name="domainName">The Active Directory domain name.</param>
        /// <param name="minimumSiteRole">The new group's minimum site role.</param>
        /// <param name="grantLicenseMode">The mode for automatically applying licenses for group members.</param>
        /// <param name="cancel">The cancellation token.</param>
        /// <returns>The newly created group.</returns>
        Task<IResult<IGroup>> ImportGroupFromActiveDirectoryAsync(
            string name,
            string domainName,
            string minimumSiteRole,
            string? grantLicenseMode,
            CancellationToken cancel);

        /// <summary>
        /// Creates a group from Active Directory and imports the group's users as a background process
        /// </summary>
        /// <param name="name">The new group's name.</param>
        /// <param name="domainName">The Active Directory domain name.</param>
        /// <param name="minimumSiteRole">The new group's minimum site role.</param>
        /// <param name="grantLicenseMode">The mode for automatically applying licenses for group members.</param>
        /// <param name="cancel">The cancellation token.</param>
        /// <returns>The newly created group.</returns>
        Task<IResult<IImportJob>> ImportGroupFromActiveDirectoryBackgroundProcessAsync(
            string name,
            string domainName,
            string minimumSiteRole,
            string? grantLicenseMode,
            CancellationToken cancel);

        /// <summary>
        /// Gets all groups in the current site.
        /// </summary>
        /// <param name="pageNumber">The 1-indexed page number.</param>
        /// <param name="pageSize">The size of the page.</param>
        /// <param name="cancel">A cancellation token to obey.</param>
        /// <returns>A list of a page of groups in the current site.</returns>
        Task<IPagedResult<IGroup>> GetAllGroupsAsync(int pageNumber, int pageSize, CancellationToken cancel);

        /// <summary>
        /// Gets the users belonging to a group
        /// </summary>
        /// <param name="groupId">The group's ID</param>
        /// <param name="pageNumber">The 1-indexed page number.</param>
        /// <param name="pageSize">The size of the page.</param>
        /// <param name="cancel">The cancellation token.</param>
        /// <returns>A list of users for the given group ID</returns>
        Task<IPagedResult<IUser>> GetGroupUsersAsync(Guid groupId, int pageNumber, int pageSize, CancellationToken cancel);

        /// <summary>
        /// Adds a user to a group.
        /// </summary>
        /// <param name="groupId">The ID of the group the user should belong to.</param>
        /// <param name="userId">The user's ID.</param>
        /// <param name="cancel">The cancellation token.</param>
        /// <returns>The operation result.</returns>        
        Task<IResult<IAddUserToGroupResult>> AddUserToGroupAsync(Guid groupId, Guid userId, CancellationToken cancel);

        /// <summary>
        /// Removes a user from a group.
        /// </summary>
        /// <param name="groupId">The id of the group the user belongs to.</param>
        /// <param name="userId">The user-id.</param>
        /// <param name="cancel">The cancellation token.</param>
        /// <returns>The operation result.</returns>        
        Task<IResult> RemoveUserFromGroupAsync(Guid groupId, Guid userId, CancellationToken cancel);
    }
}
