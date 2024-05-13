//
//  Copyright (c) 2024, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the "License") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

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
