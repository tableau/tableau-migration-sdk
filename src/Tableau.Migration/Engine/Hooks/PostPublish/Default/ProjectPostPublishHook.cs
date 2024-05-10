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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Permissions;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Hooks.Transformers.Default;

namespace Tableau.Migration.Engine.Hooks.PostPublish.Default
{
    /// <summary>
    /// Project content migration completed hook.
    /// </summary>
    public class ProjectPostPublishHook : PermissionPostPublishHookBase<IProject, IProject>
    {
        private readonly IPermissionsTransformer _permissionsTransformer;

        private readonly ISourceApiEndpoint? _sourceApiEndpoint;
        private readonly IDestinationApiEndpoint? _destinationApiEndpoint;

        /// <summary>
        /// Gets whether the hook is enabled.
        /// </summary>
        [MemberNotNullWhen(true, nameof(_sourceApiEndpoint), nameof(_destinationApiEndpoint))]
        internal bool IsEnabled { get; } // Using a property instead of a field here so we can use the MemberNotNullWhen attribute.

        /// <summary>
        /// Creates a new <see cref="ProjectPostPublishHook"/> object.
        /// </summary>
        /// <param name="migration">The current migration.</param>
        /// <param name="permissionsTransformer">The transformer used for permissions.</param>
        public ProjectPostPublishHook(IMigration migration,
            IPermissionsTransformer permissionsTransformer)
            : base(migration)
        {
            _permissionsTransformer = permissionsTransformer;

            IsEnabled = Migration.TryGetSourceApiEndpoint(out _sourceApiEndpoint) && Migration.TryGetDestinationApiEndpoint(out _destinationApiEndpoint);
        }

        /// <inheritdoc/>
        public override async Task<ContentItemPostPublishContext<IProject, IProject>?> ExecuteAsync(ContentItemPostPublishContext<IProject, IProject> ctx, CancellationToken cancel)
        {
            Migration.Pipeline.GetDestinationLockedProjectCache().UpdateLockedProjectCache(ctx.DestinationItem);

            if (!IsEnabled || await ParentProjectLockedAsync(ctx, cancel).ConfigureAwait(false))
            {
                return ctx;
            }

            var sourceDefaultPermissionsResult = await _sourceApiEndpoint.SiteApi.Projects.GetAllDefaultPermissionsAsync(ctx.PublishedItem.Id, cancel).ConfigureAwait(false);
            if (!sourceDefaultPermissionsResult.Success)
            {
                ctx.ManifestEntry.SetFailed(sourceDefaultPermissionsResult.Errors);
                return ctx;
            }

            var transformedPermissions = await TransformPermissionsAsync(sourceDefaultPermissionsResult.Value, cancel).ConfigureAwait(false);

            var updateDefaultPermissionsResult = await _destinationApiEndpoint.SiteApi.Projects.UpdateAllDefaultPermissionsAsync(
                    ctx.DestinationItem.Id,
                    transformedPermissions,
                    cancel)
                .ConfigureAwait(false);

            if (!updateDefaultPermissionsResult.Success)
            {
                ctx.ManifestEntry.SetFailed(updateDefaultPermissionsResult.Errors);
            }

            return ctx;
        }

        private async Task<IImmutableDictionary<string, IPermissions>> TransformPermissionsAsync(
            IReadOnlyDictionary<string, IPermissions> sourcePermissions,
            CancellationToken cancel)
        {
            var transformedPermissions = ImmutableDictionary.CreateBuilder<string, IPermissions>(StringComparer.OrdinalIgnoreCase);

            foreach (var sourcePermission in sourcePermissions)
            {
                var transformedGrantees = await _permissionsTransformer.ExecuteAsync(sourcePermission.Value.GranteeCapabilities.ToImmutableArray(), cancel).ConfigureAwait(false);

                transformedPermissions.Add(sourcePermission.Key, new Permissions(null, transformedGrantees));
            }

            return transformedPermissions.ToImmutable();
        }
    }
}
