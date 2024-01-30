using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Permissions;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Hooks.PostPublish;
using Tableau.Migration.Engine.Hooks.PostPublish.Default;

namespace Csharp.ExampleApplication.Hooks.PostPublish
{
    #region class
    public class UpdatePermissionsHook<TPublish, TResult> : PermissionPostPublishHookBase<TPublish, TResult>
        where TResult : IPermissionsContent, IWithTags
    {
        private static readonly StringComparer StringComparer = StringComparer.OrdinalIgnoreCase;

        private readonly ILogger<UpdatePermissionsHook<TPublish, TResult>> _logger;

        public UpdatePermissionsHook(IMigration migration, ILogger<UpdatePermissionsHook<TPublish, TResult>> logger)
            : base(migration)
        {
            _logger = logger;
        }

        /// <summary>
        /// Gets whether the content item's permissions should be updated.
        /// The logic here can be customized for other scenarios.
        /// </summary>
        /// <param name="item">The content item to evaluate</param>
        /// <returns>True to update the item's permissions, false otherwise.</returns>
        private static bool ShouldUpdatePermissions(TResult item)
        {
            // Only update permissions for content with a "Production" tag.
            var hasProductionTag = item.Tags.Any(t => StringComparer.Equals("Production", t.Label));

            return hasProductionTag;
        }

        /// <summary>
        /// Updates the capability (permission).
        /// The logic here can be customized for other scenarios.
        /// </summary>
        /// <param name="contentItem">The content item.</param>
        /// <param name="capabilities">The capability collection to update.</param>
        private bool UpdateCapabilities(TResult contentItem, HashSet<ICapability> capabilities)
        {
            var capabilityToUpdate = PermissionsCapabilityNames.Write;

            var removedCount = capabilities.RemoveWhere(c => 
                StringComparer.Equals(capabilityToUpdate, c.Name));

            if (removedCount == 0)
                return false;

            // Add the write/deny permission
            var updatedCapability = new Capability(capabilityToUpdate, PermissionsCapabilityModes.Deny);

            capabilities.Add(updatedCapability);

            _logger.LogInformation(
                "Set {ContentType} {ContentItem}'s {PermissionName} permission to {PermissionMode}.",
                typeof(TResult).Name,
                contentItem.Location,
                capabilityToUpdate,
                updatedCapability.Mode);

            return true;
        }

        public override async Task<ContentItemPostPublishContext<TPublish, TResult>?> ExecuteAsync(ContentItemPostPublishContext<TPublish, TResult> ctx, CancellationToken cancel)
        {
            // If parent project permissions are locked we can't update them.
            if (await ParentProjectLockedAsync(ctx, cancel))
            {
                return ctx;
            }

            // Since we're updating content after publish, our changes will be made to the destination item.
            var contentItem = ctx.DestinationItem;

            if (!ShouldUpdatePermissions(contentItem))
            {
                return ctx;
            }

            // Get the content item's current permissions.
            var permissionsResult = await Migration.Destination.GetPermissionsAsync<TResult>(contentItem, cancel);

            if (!permissionsResult.Success)
            {
                ctx.ManifestEntry.SetFailed(permissionsResult.Errors);
                return ctx;
            }

            var permissions = permissionsResult.Value;

            var hasUpdates = false; 

            // Loop through the permission items to find/update the capabilities.
            foreach (var granteeCapability in permissions.GranteeCapabilities)
            {
                if (UpdateCapabilities(ctx.DestinationItem, granteeCapability.Capabilities))
                    hasUpdates = true;
            }

            // If we haven't made any changes we can skip this part.
            if (hasUpdates)
            {
                // Update the content's permissions with our updated ones.
                var updatePermissionsResult = await Migration.Destination.UpdatePermissionsAsync<TResult>(
                    contentItem,
                    permissions,
                    cancel);

                if (!updatePermissionsResult.Success)
                {
                    ctx.ManifestEntry.SetFailed(updatePermissionsResult.Errors);
                }
            }

            return ctx;
        }
    }
    #endregion
}
