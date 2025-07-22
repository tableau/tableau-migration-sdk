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
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Permissions;
using Tableau.Migration.Engine.Hooks.Transformers;

namespace Tableau.Migration.Engine.Hooks.PostPublish.Default
{
    /// <summary>
    /// Abstract base class for post publish hooks that perform permissions updates.
    /// </summary>
    /// <typeparam name="TPublish"><inheritdoc /></typeparam>
    /// <typeparam name="TResult"><inheritdoc /></typeparam>
    public abstract class PermissionPostPublishHookBase<TPublish, TResult> : ContentItemPostPublishHookBase<TPublish, TResult>
    {
        private readonly IContentTransformerRunner _transformerRunner;

        /// <summary>
        /// Gets the current migration.
        /// </summary>
        protected IMigration Migration { get; }

        /// <summary>
        /// Creates a new <see cref="PermissionPostPublishHookBase{TPublish, TResult}"/> object.
        /// </summary>
        /// <param name="migration">The current migration.</param>
        /// <param name="transformerRunner">A transformer runner to use for permissions.</param>
        public PermissionPostPublishHookBase(IMigration migration, IContentTransformerRunner transformerRunner)
        {
            _transformerRunner = transformerRunner;

            Migration = migration;
        }

        /// <summary>
        /// Runs transformers on a set of permission grantees, retaining parent ID.
        /// </summary>
        /// <param name="permissions">The permissions to transform</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The transformed permissions.</returns>
        protected async Task<IPermissions> TransformPermissionsAsync(IPermissions permissions, CancellationToken cancel)
        {
            var resultSet = await _transformerRunner.ExecuteAsync((IPermissionSet)permissions, cancel).ConfigureAwait(false);

            if (resultSet is IPermissions result)
            {
                return result;
            }

            return new Permissions(permissions.ParentId, resultSet.GranteeCapabilities);
        }

        /// <summary>
        /// Finds the parent project ID of the content item.
        /// </summary>
        /// <param name="result">The content item's destination publis result.</param>
        /// <returns>The parent project ID, or null if the item does not have a parent project.</returns>
        protected virtual Guid? GetDestinationProjectId(TResult result)
        {
            if (result is IContainerContent containerContent)
            {
                return containerContent.Container.Id;
            }
            else if (result is IMappableContainerContent mapableContainerContent)
            {
                return mapableContainerContent.Container?.Id;
            }

            return null;
        }

        /// <summary>
        /// Finds whether or not the parent/containing project is locked.
        /// </summary>
        /// <param name="ctx">The post publish context.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>Whether or not the parent project is locked.</returns>
        protected async Task<bool> ParentProjectLockedAsync(ContentItemPostPublishContext<TPublish, TResult> ctx, CancellationToken cancel)
        {
            var destinationProjectId = GetDestinationProjectId(ctx.DestinationItem);
            if (destinationProjectId is null)
            {
                return false;
            }

            //For purposes of permissions, a project can still set permissions
            //if the parent is "locked without nesting."
            //No other content types can set permissions when the parent is locked, even without nesting.
            var includeWithoutNested = ctx.DestinationItem is not IProject;

            var lockedProjectCache = Migration.Pipeline.GetDestinationLockedProjectCache();
            var parentProjectLocked = await lockedProjectCache.IsProjectLockedAsync(destinationProjectId.Value, cancel, includeWithoutNested)
                .ConfigureAwait(false);

            return parentProjectLocked;
        }
    }
}
