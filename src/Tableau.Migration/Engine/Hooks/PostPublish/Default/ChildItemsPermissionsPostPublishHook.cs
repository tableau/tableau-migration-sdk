//
//  Copyright (c) 2025, Salesforce, Inc.
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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Hooks.Transformers;

namespace Tableau.Migration.Engine.Hooks.PostPublish.Default
{
    /// <summary>
    /// Permissions content migration completed hook 
    /// </summary>
    /// <typeparam name="TPublish"><inheritdoc /></typeparam>
    /// <typeparam name="TResult"><inheritdoc /></typeparam>
    public class ChildItemsPermissionsPostPublishHook<TPublish, TResult> : PermissionPostPublishHookBase<TPublish, TResult>
        where TPublish : IChildPermissionsContent
        where TResult : IChildPermissionsContent
    {
        /// <summary>
        /// Creates a new <see cref="PermissionsItemPostPublishHook{TPublish, TDestination}"/> object.
        /// </summary>
        /// <param name="migration"><inheritdoc /></param>
        /// <param name="transformerRunner"><inheritdoc /></param>
        public ChildItemsPermissionsPostPublishHook(IMigration migration, IContentTransformerRunner transformerRunner)
            : base(migration, transformerRunner)
        { }

        /// <inheritdoc/>
        public override async Task<ContentItemPostPublishContext<TPublish, TResult>?> ExecuteAsync(ContentItemPostPublishContext<TPublish, TResult> ctx, CancellationToken cancel)
        {
            if (!ctx.PublishedItem.ShouldMigrateChildPermissions ||
                await ParentProjectLockedAsync(ctx, cancel).ConfigureAwait(false))
            {
                return ctx;
            }

            var childItems = ImmutableDictionary.CreateBuilder<IContentReference, IContentReference>();

            foreach (var source in ctx.PublishedItem.ChildPermissionContentItems)
            {
                var destination = ctx.DestinationItem.ChildPermissionContentItems.FirstOrDefault(x => string.Equals(x.Name, source.Name, StringComparison.OrdinalIgnoreCase));
                if(destination is null)
                {
                    continue;
                }
                childItems.Add(source, destination);
            }

            //Aggregate result errors across all child items.
            var childResultBuilder = new ResultBuilder();
            foreach (var (source, destination) in childItems.ToImmutable())
            {
                var childType = ctx.PublishedItem.ChildType;

                var sourcePermissionsResult = await Migration.Source.GetPermissionsAsync(childType, source, cancel).ConfigureAwait(false);
                if (!sourcePermissionsResult.Success)
                {
                    childResultBuilder.Add(sourcePermissionsResult);
                    continue;
                }

                var transformedPermissions = await TransformPermissionsAsync(sourcePermissionsResult.Value, cancel).ConfigureAwait(false);

                var updatePermissionsResult = await Migration.Destination.UpdatePermissionsAsync(
                        childType,
                        destination,
                        transformedPermissions,
                        cancel)
                    .ConfigureAwait(false);

                childResultBuilder.Add(updatePermissionsResult);
            }

            var overallResult = childResultBuilder.Build();
            if (!overallResult.Success)
            {
                ctx.ManifestEntry.SetFailed(overallResult.Errors);
            }

            return ctx;
        }
    }
}
