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

using System.Collections.Generic;
using System.Linq;
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
    public abstract class ContentFilterBase<TContent> : IContentFilter<TContent>
        where TContent : IContentReference
    {
        private readonly string _typeName;

        /// <summary>
        /// Default constructor for ContentFilterBase
        /// </summary>
        /// <param name="localizer">A string localizer.</param>
        /// <param name="logger">Default logger.</param>
        public ContentFilterBase(
            ISharedResourcesLocalizer? localizer,
            ILogger<IContentFilter<TContent>>? logger)
        {
            Localizer = localizer;
            Logger = logger;
            _typeName = GetType().Name;
        }

        /// <summary>
        /// The string localizer.
        /// </summary>
        protected ISharedResourcesLocalizer? Localizer { get; }

        /// <summary>
        /// Default logger.
        /// </summary>
        protected ILogger<IContentFilter<TContent>>? Logger { get; }

        /// <inheritdoc />
        public Task<IEnumerable<ContentMigrationItem<TContent>>?> ExecuteAsync(
            IEnumerable<ContentMigrationItem<TContent>> unfilteredItems,
            CancellationToken cancel)
        {
            var result = unfilteredItems;

            //Avoid re-allocation on a no-op/disabled filter.
            if (!Disabled)
            {
                result = unfilteredItems.Where(ShouldMigrate);

                // Log the filtered items if requested
                if (Logger is not null && Localizer is not null) 
                {
                    // Don't do the work if the logger is not enabled for this level
                    var filteredItems = unfilteredItems.Except(result).ToList();
                    if (filteredItems.Count() > 0)
                    {
                        foreach (var filteredItem in filteredItems)
                        {
                            Logger.LogDebug(
                                Localizer[SharedResourceKeys.ContentFilterBaseDebugMessage], 
                                _typeName, 
                                filteredItem.SourceItem.ToStringForLog());
                        }
                    }
                }
            }

            return Task.FromResult((IEnumerable<ContentMigrationItem<TContent>>?)result);
        }

        /// <summary>
        /// Gets or sets whether the filter is disabled.
        /// </summary>
        protected virtual bool Disabled { get; set; }

        /// <summary>
        /// Checks if the item should be migrated.
        /// </summary>
        /// <param name="item">The item to evaluate.</param>
        /// <returns>True if the item should be migrated.</returns>
        public abstract bool ShouldMigrate(ContentMigrationItem<TContent> item);
    }
}

