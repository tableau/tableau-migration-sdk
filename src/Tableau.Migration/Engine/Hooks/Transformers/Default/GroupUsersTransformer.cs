using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Pipelines;

namespace Tableau.Migration.Engine.Hooks.Transformers.Default
{
    /// <summary>
    /// Transformer that maps the users from a given group.
    /// </summary>
    public class GroupUsersTransformer : IContentTransformer<IPublishableGroup>
    {
        private readonly IMigrationPipeline _migrationPipeline;
        private readonly ILogger<GroupUsersTransformer> _logger;

        /// <summary>
        /// Creates a new <see cref="GroupUsersTransformer"/> object.
        /// </summary>
        /// <param name="migrationPipeline">Destination content finder object.</param>
        /// <param name="logger">The logger used to log messages.</param>
        public GroupUsersTransformer(IMigrationPipeline migrationPipeline, ILogger<GroupUsersTransformer> logger)
        {
            _migrationPipeline = migrationPipeline;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<IPublishableGroup?> ExecuteAsync(
            IPublishableGroup ctx,
            CancellationToken cancel)
        {
            var contentFinder = _migrationPipeline.CreateDestinationFinder<IUser>();

            foreach (var user in ctx.Users)
            {
                var contentDestination = await contentFinder
                    .FindDestinationReferenceAsync(user.User.Location, cancel)
                    .ConfigureAwait(false);

                if (contentDestination is not null)
                {
                    user.User = contentDestination;
                }
                else
                {
                    _logger.LogWarning("Group {GroupName} cannot map {UserLocation}", ctx.Name, user.User.Location);
                }
            }
            return ctx;
        }
    }
}
