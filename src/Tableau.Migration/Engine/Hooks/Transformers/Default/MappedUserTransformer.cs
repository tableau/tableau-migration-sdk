using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Engine.Pipelines;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Hooks.Transformers.Default
{
    /// <summary>
    /// Transformer that maps the user for a content item owner.
    /// </summary>
    public class MappedUserTransformer : IMappedUserTransformer
    {
        private readonly IMappedContentReferenceFinder<IUser> _userFinder;
        private readonly ILogger _logger;
        private readonly ISharedResourcesLocalizer _localizer;

        /// <summary>
        /// Creates a new <see cref="MappedUserTransformer"/> object.
        /// </summary>
        /// <param name="pipeline">The migration pipeline.</param>
        /// <param name="logger">The logger used to log messages.</param>
        /// <param name="localizer">The string localizer.</param>
        public MappedUserTransformer(
            IMigrationPipeline pipeline,
            ILogger<MappedUserTransformer> logger,
            ISharedResourcesLocalizer localizer)
        {
            _userFinder = pipeline.CreateDestinationFinder<IUser>();
            _logger = logger;
            _localizer = localizer;
        }

        /// <inheritdoc />
        public async Task<IContentReference?> ExecuteAsync(IContentReference ctx, CancellationToken cancel)
        {
            //Unable to map system user, as its info is hidden from APIs except for owner references.
            if(ctx.Location == Constants.SystemUserLocation)
            {
                return null;
            }

            var mapped = await _userFinder.FindDestinationReferenceAsync(ctx.Location, cancel)
                .ConfigureAwait(false);

            if (mapped is null)
            {
                _logger.LogWarning(_localizer[SharedResourceKeys.SourceUserNotFoundLogMessage], ctx.Name, ctx.Id);
            }

            return mapped;
        }
    }
}
