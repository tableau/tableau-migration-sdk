using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Permissions;
using Tableau.Migration.Engine.Hooks.Transformers.Default;

namespace Tableau.Migration.Engine.Hooks.PostPublish.Default
{
    /// <summary>
    /// Permissions content migration completed hook. 
    /// </summary>
    /// <typeparam name="TPublish"><inheritdoc /></typeparam>
    /// <typeparam name="TResult"><inheritdoc /></typeparam>
    public class PermissionsItemPostPublishHook<TPublish, TResult> : PermissionPostPublishHookBase<TPublish, TResult>
        where TPublish : IPermissionsContent
        where TResult : IContentReference
    {
        private readonly IPermissionsTransformer _permissionsTransformer;

        /// <summary>
        /// Creates a new <see cref="PermissionsItemPostPublishHook{TPublish, TDestination}"/> object.
        /// </summary>
        /// <param name="migration">The current migration.</param>
        /// <param name="permissionsTransformer">The transformer used for permissions.</param>
        public PermissionsItemPostPublishHook(IMigration migration,
            IPermissionsTransformer permissionsTransformer)
            : base(migration)
        {
            _permissionsTransformer = permissionsTransformer;
        }

        /// <inheritdoc/>
        public override async Task<ContentItemPostPublishContext<TPublish, TResult>?> ExecuteAsync(ContentItemPostPublishContext<TPublish, TResult> ctx, CancellationToken cancel)
        {
            if (await ParentProjectLockedAsync(ctx, cancel).ConfigureAwait(false))
            {
                return ctx;
            }

            var sourcePermissionsResult = await Migration.Source.GetPermissionsAsync<TPublish>(ctx.PublishedItem, cancel).ConfigureAwait(false);
            if (!sourcePermissionsResult.Success)
            {
                ctx.ManifestEntry.SetFailed(sourcePermissionsResult.Errors);
                return ctx;
            }

            var transformedPermissions = await TransformPermissionsAsync(ctx, sourcePermissionsResult.Value, cancel).ConfigureAwait(false);

            var updatePermissionsResult = await Migration.Destination.UpdatePermissionsAsync<TPublish>(
                    ctx.DestinationItem,
                    transformedPermissions,
                    cancel)
                .ConfigureAwait(false);

            if (!updatePermissionsResult.Success)
            {
                ctx.ManifestEntry.SetFailed(updatePermissionsResult.Errors);
            }

            return ctx;
        }

        private async Task<IPermissions> TransformPermissionsAsync(
            ContentItemPostPublishContext<TPublish, TResult> ctx,
            IPermissions sourcePermissions,
            CancellationToken cancel)
        {
            var transformedGrantees = await _permissionsTransformer.ExecuteAsync(sourcePermissions.GranteeCapabilities.ToImmutableArray(), cancel).ConfigureAwait(false);

            Guid? parentId = null;

            if (sourcePermissions.ParentId is not null && sourcePermissions.ParentId == ctx.PublishedItem.Id)
                parentId = ctx.DestinationItem.Id;

            return new Permissions(parentId, transformedGrantees);
        }
    }
}
