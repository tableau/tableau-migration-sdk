using System;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Content;

namespace Tableau.Migration.Engine.Endpoints.Search
{
    /// <summary>
    /// Interface for an object that contains information on projects that are locked.
    /// </summary>
    public interface ILockedProjectCache
    {
        /// <summary>
        /// Finds whether a project is locked.
        /// </summary>
        /// <param name="id">The ID of the project.</param>
        /// <param name="cancel">A cancellation token to obey.</param>
        /// <param name="includeWithoutNested">
        /// Whether or not to consider <see cref="ContentPermissions.LockedToProjectWithoutNested"/> as locked.
        /// Except for narrow special cases this is true.
        /// </param>
        /// <returns>True if the project is locked; false if the project is not locked or not found.</returns>
        Task<bool> IsProjectLockedAsync(Guid id, CancellationToken cancel, bool includeWithoutNested = true);

        /// <summary>
        /// Updates the locked project cache with the given project information.
        /// </summary>
        /// <param name="project">The project to update the cache for.</param>
        void UpdateLockedProjectCache(IProject project);
    }
}
