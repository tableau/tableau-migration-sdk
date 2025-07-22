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

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Hooks.Filters
{
    /// <summary>
    /// Base implementation for an object that can filter content of a specific content type, to determine which content to migrate.
    /// </summary>
    /// <typeparam name="TContent"><inheritdoc/></typeparam>
    public abstract class AsyncContentFilterBase<TContent> : RootContentFilterBase<TContent>
        where TContent : IContentReference
    {
        /// <summary>
        /// Creates a new <see cref="AsyncContentFilterBase{TContent}"/> object.
        /// </summary>
        /// <param name="localizer"><inheritdoc /></param>
        /// <param name="logger"><inheritdoc /></param>
        public AsyncContentFilterBase(ISharedResourcesLocalizer? localizer, ILogger<IContentFilter<TContent>>? logger)
            : base(localizer, logger)
        { }

        /// <inheritdoc />
        public override async Task<IEnumerable<ContentMigrationItem<TContent>>?> ExecuteAsync(IEnumerable<ContentMigrationItem<TContent>> ctx, CancellationToken cancel)
        {
            // Avoid re-allocation on a no-op/disabled filter.
            if (Disabled)
            {
                return ctx;
            }

            var builder = ImmutableArray.CreateBuilder<ContentMigrationItem<TContent>>();
            foreach (var item in ctx)
            {
                if(await ShouldMigrateAsync(item, cancel).ConfigureAwait(false))
                {
                    builder.Add(item);
                }
            }

            return builder.ToImmutable();
        }

        /// <summary>
        /// Checks if the item should be migrated.
        /// </summary>
        /// <param name="item">The item to evaluate.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>True if the item should be migrated.</returns>
        public abstract Task<bool> ShouldMigrateAsync(ContentMigrationItem<TContent> item, CancellationToken cancel);
    }
}
