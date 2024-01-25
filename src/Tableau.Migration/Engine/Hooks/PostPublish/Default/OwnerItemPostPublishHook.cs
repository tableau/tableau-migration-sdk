using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Content;

namespace Tableau.Migration.Engine.Hooks.PostPublish.Default
{
    /// <summary>
    /// Hook that updates a content item's owner after publish.
    /// </summary>
    /// <typeparam name="TPublish"><inheritdoc /></typeparam>
    /// <typeparam name="TResult"><inheritdoc /></typeparam>
    public class OwnerItemPostPublishHook<TPublish, TResult> : ContentItemPostPublishHookBase<TPublish, TResult>
        where TPublish : IRequiresOwnerUpdate
        where TResult : IWithOwner
    {
        private readonly IMigration _migration;

        /// <summary>
        /// Creates a new <see cref="OwnerItemPostPublishHook{TPublish, TDestination}"/> object.
        /// </summary>
        /// <param name="migration">The current migration</param>
        public OwnerItemPostPublishHook(IMigration migration)
        {
            _migration = migration;
        }

        /// <inheritdoc/>
        public override async Task<ContentItemPostPublishContext<TPublish, TResult>?> ExecuteAsync(ContentItemPostPublishContext<TPublish, TResult> ctx, CancellationToken cancel)
        {
            //Don't apply ownership if its a no-op
            //or if the source object has system ownership, which we can't assign on the destination.
            if (ctx.PublishedItem.Owner.Id == ctx.DestinationItem.Owner.Id
                || ctx.PublishedItem.Owner.Location == Constants.SystemUserLocation)
            {
                return ctx;
            }

            //The owner has already been mapped by our default OwnershipTransformer.
            var updateOwnerResult = await _migration.Destination.UpdateOwnerAsync<TPublish>(ctx.DestinationItem, ctx.PublishedItem.Owner, cancel)
                .ConfigureAwait(false);

            if (!updateOwnerResult.Success)
            {
                ctx.ManifestEntry.SetFailed(updateOwnerResult.Errors);
            }

            return ctx;
        }
    }
}
