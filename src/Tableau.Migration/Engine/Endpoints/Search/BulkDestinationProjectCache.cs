using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Config;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Manifest;

namespace Tableau.Migration.Engine.Endpoints.Search
{
    /// <summary>
    /// <see cref="BulkDestinationCache{IProject}"/> implementation that tracks locked projects.
    /// </summary>
    public class BulkDestinationProjectCache : BulkDestinationCache<IProject>, ILockedProjectCache
    {
        private readonly ConcurrentDictionary<Guid, string> _projectContentPermissionModeCache;

        /// <summary>
        /// Creates a new <see cref="BulkDestinationProjectCache"/> object.
        /// </summary>
        /// <param name="manifest">The migration manifest.</param>
        /// <param name="endpoint">The destination endpoint.</param>
        /// <param name="configReader">The configuration reader.</param>
        public BulkDestinationProjectCache(IMigrationManifestEditor manifest, IDestinationEndpoint endpoint, IConfigReader configReader) 
            : base(manifest, endpoint, configReader)
        {
            _projectContentPermissionModeCache = new();
        }

        /// <inheritdoc />
        protected override void ItemLoaded(IProject item)
        {
            base.ItemLoaded(item);
            UpdateLockedProjectCache(item);            
        }

        /// <inheritdoc />
        public async Task<bool> IsProjectLockedAsync(Guid id, CancellationToken cancel, bool includeWithoutNested = true)
        {
            await LoadStoreAsync(cancel).ConfigureAwait(false);

            if(!_projectContentPermissionModeCache.TryGetValue(id, out var mode))
            {
                return false;
            }
            
            if(ContentPermissions.IsAMatch(ContentPermissions.LockedToProject, mode))
            {
                return true;
            }
            else if(includeWithoutNested && ContentPermissions.IsAMatch(ContentPermissions.LockedToProjectWithoutNested, mode))
            {
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public void UpdateLockedProjectCache(IProject project)
        {
            _projectContentPermissionModeCache.AddOrUpdate(project.Id, project.ContentPermissions, (k, v) => project.ContentPermissions);
        }
    }
}
