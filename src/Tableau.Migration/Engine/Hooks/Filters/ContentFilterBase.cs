//
//  Copyright (c) 2026, Salesforce, Inc.
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
using Microsoft.Extensions.Logging;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Hooks.Filters
{
    /// <summary>
    /// Base implementation for an object that can filter content of a specific content type, to determine which content to migrate.
    /// </summary>
    /// <typeparam name="TContent"><inheritdoc/></typeparam>
    public abstract class ContentFilterBase<TContent> : RootContentFilterBase<TContent>
        where TContent : IContentReference
    {
        /// <summary>
        /// Creates a new <see cref="ContentFilterBase{TContent}"/> object.
        /// </summary>
        /// <param name="localizer"><inheritdoc /></param>
        /// <param name="logger"><inheritdoc /></param>
        public ContentFilterBase(ISharedResourcesLocalizer? localizer, ILogger<IContentFilter<TContent>>? logger)
            : base(localizer, logger)
        { }

        /// <inheritdoc />
        public override Task<ContentFilterContext<TContent>?> ExecuteAsync(ContentFilterContext<TContent> ctx, CancellationToken cancel)
        {
            if (!Disabled)
            {
                foreach (var item in ctx.Items)
                {
                    Filter(item);
                }
            }

            return Task.FromResult((ContentFilterContext<TContent>?)ctx);
        }

        /// <summary>
        /// Considers the content item for filtering.
        /// </summary>
        /// <param name="item">The item to potentially filter.</param>
        public virtual void Filter(ContentFilterContextItem<TContent> item)
        {
            /*
             * To reduce the impact of the breaking change between
             * pre-cascading filters and post-cascading filters,
             * we default filter base classes to emulate the  pre-cascading filter behavior.
             * Base class callers can opt-in to the new cascading filter behavior by
             * overridding Filter (this method) instead of ShouldMigrate.
             * 
             * Pre-cascading behavior considered all items filtered out as "skip without cascade,"
             * and the decision to filter out an item was final.
             * The following filters were not called with items that were already filtered out,
             * and could not reverse that decision.
             */
            if (item.Status is not FilterStatus.Migrate)
            {
                return;
            }

            var shouldMigrate = ShouldMigrate(item);
            if (!shouldMigrate)
            {
                item.Status = FilterStatus.Skip;
            }
        }

        /// <summary>
        /// Checks if the item should be migrated.
        /// </summary>
        /// <param name="item">The item to evaluate.</param>
        /// <returns>True if the item should be migrated.</returns>
        public virtual bool ShouldMigrate(ContentMigrationItem<TContent> item) => true;
    }
}

