using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Permissions;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Engine.Pipelines;

namespace Tableau.Migration.Engine.Hooks.Transformers.Default
{
    /// <summary>
    /// Transformer that maps the users or groups from a given permission.
    /// </summary>
    public class PermissionsTransformer : IPermissionsTransformer
    {
        private readonly IMappedContentReferenceFinder<IUser> _userContentFinder;
        private readonly IMappedContentReferenceFinder<IGroup> _groupContentFinder;

        /// <summary>
        /// Creates a new <see cref="PermissionsTransformer"/> object.
        /// </summary>
        /// <param name="migrationPipeline">Destination content finder object.</param>
        public PermissionsTransformer(IMigrationPipeline migrationPipeline)
        {
            _userContentFinder = migrationPipeline.CreateDestinationFinder<IUser>();
            _groupContentFinder = migrationPipeline.CreateDestinationFinder<IGroup>();
        }

        private static bool ShouldMigrateCapability(ICapability c)
        {
            //W-14374726 Some versions of Tableau Server (pre-2020.1) supported ProjectLeader Deny capabilities,
            //but that feature was removed. These capabilities remain in the database (even after upgrade)
            //but will throw errors when migrated through APIs.
            //Thus we filter out these capabilities to avoid errors the user has no control over.
            if (PermissionsCapabilityNames.IsAMatch(PermissionsCapabilityNames.ProjectLeader, c.Name))
            {
                if (PermissionsCapabilityModes.IsAMatch(PermissionsCapabilityModes.Deny, c.Mode))
                {
                    return false;
                }
            }

            //Inherited leaders are calculated automatically and don't need to be set manually.
            if (PermissionsCapabilityNames.IsAMatch(PermissionsCapabilityNames.InheritedProjectLeader, c.Name))
            {
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public async Task<IImmutableList<IGranteeCapability>?> ExecuteAsync(
            IImmutableList<IGranteeCapability> granteeCapabilities,
            CancellationToken cancel)
        {
            var transformedGrantees = new List<IGranteeCapability>();

            var groupsById = new HashSet<IGranteeCapability>(granteeCapabilities).GroupBy(c => c.GranteeId);

            foreach (var group in groupsById)
            {               
                var granteeType = group.First().GranteeType;

                IMappedContentReferenceFinder contentFinder = granteeType is GranteeType.User
                    ? _userContentFinder : _groupContentFinder;

                var destinationGrantee = await contentFinder
                    .FindDestinationReferenceAsync(group.Key, cancel)
                    .ConfigureAwait(false);

                if (destinationGrantee is null)
                {
                    continue;
                }

                var destinationCapabilities = group
                    .SelectMany(g => g.Capabilities)
                    .Where(ShouldMigrateCapability)
                    .ResolveCapabilityModeConflicts();

                //Capability resolution automatically happens here since this
                //GranteeCapability constructor applies that logic.
                var transformedGrantee = new GranteeCapability(
                    granteeType,
                    destinationGrantee.Id,
                    destinationCapabilities);

                transformedGrantees.Add(transformedGrantee);
            }

            return transformedGrantees.ToImmutableArray();
        }
    }
}
