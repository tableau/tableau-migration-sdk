﻿//
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

using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Content;

namespace Tableau.Migration.Engine.Hooks.PostPublish.Default
{
    /// <summary>
    /// Hook that updates a custom view's default users after publish.
    /// </summary>
    public class CustomViewDefaultUsersPostPublishHook
        : ContentItemPostPublishHookBase<IPublishableCustomView, ICustomView>
    {
        private readonly IMigration _migration;

        /// <summary>
        /// Creates a new <see cref="CustomViewDefaultUsersPostPublishHook"/> object.
        /// </summary>
        /// <param name="migration">The current migration.</param>
        public CustomViewDefaultUsersPostPublishHook(IMigration migration)
        {
            _migration = migration;
        }

        /// <inheritdoc/>
        public override async Task<ContentItemPostPublishContext<IPublishableCustomView, ICustomView>?> ExecuteAsync(
            ContentItemPostPublishContext<IPublishableCustomView, ICustomView> ctx,
            CancellationToken cancel)
        {
            var setResult = await _migration.Destination
                .SetCustomViewDefaultUsersAsync(
                ctx.DestinationItem.Id,
                ctx.PublishedItem.DefaultUsers,
                cancel)
                .ConfigureAwait(false);

            if (!setResult.Success)
            {
                ctx.ManifestEntry.SetFailed(setResult.Errors);
            }

            return ctx;
        }
    }
}
