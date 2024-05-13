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

using System.Collections.Immutable;
using System.Linq;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Options;
using Tableau.Migration.Resources;
using Microsoft.Extensions.Logging;

namespace Tableau.Migration.Engine.Hooks.Filters.Default
{
    /// <summary>
    /// <see cref="IContentFilter{IGroup}"/> implementation used to filter out the default "All Users" group. 
    /// </summary>
    public sealed class GroupAllUsersFilter : ContentFilterBase<IGroup>
    {
        private readonly IImmutableList<string> _allUsersTranslations;

        /// <summary>
        /// Creates a new <see cref="GroupAllUsersFilter"/> instance.
        /// </summary>
        /// <param name="optionsProvider">The filter options provider.</param>
        /// <param name="localizer">A string localizer.</param>
        /// <param name="logger">Default logger.</param>
        public GroupAllUsersFilter(
            IMigrationPlanOptionsProvider<GroupAllUsersFilterOptions> optionsProvider, 
            ISharedResourcesLocalizer localizer,
            ILogger<IContentFilter<IGroup>> logger) : base(localizer, logger)
        {
            var options = optionsProvider.Get();

            _allUsersTranslations = AllUsersTranslations.GetAll(options.AllUsersGroupNames);
        }

        /// <inheritdoc/>
        public override bool ShouldMigrate(ContentMigrationItem<IGroup> item)
        {
            // Special-casing for English since it'll be the most common.
            if (item.SourceItem.Name is AllUsersTranslations.English ||
                _allUsersTranslations.Contains(item.SourceItem.Name))
            {
                return false;
            }

            return true;
        }
    }
}
