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

using Microsoft.Extensions.Logging;
using Tableau.Migration.Engine.Options;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Hooks.Filters.Default
{
    /// <summary>
    /// Migration filter that skips previously migrated content.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    public class PreviouslyMigratedFilter<TContent> 
        : ContentFilterBase<TContent> where TContent : IContentReference
    {
        /// <summary>
        /// Creates a new <see cref="PreviouslyMigratedFilter{TContent}"/> object.
        /// </summary>
        /// <param name="input">The migration input.</param>
        /// <param name="optionsProvider">The options provider.</param>
        /// <param name="localizer">A string localizer.</param>
        /// <param name="logger">The logger.</param>
        public PreviouslyMigratedFilter(
            IMigrationInput input, 
            IMigrationPlanOptionsProvider<PreviouslyMigratedFilterOptions> optionsProvider, 
            ISharedResourcesLocalizer localizer,
            ILogger<IContentFilter<TContent>> logger) : base(localizer, logger)
        {
            Disabled = input.PreviousManifest is null || optionsProvider.Get().Disabled;
        }

        /// <inheritdoc />
        public override bool ShouldMigrate(ContentMigrationItem<TContent> item)
        {
            //The item's manifest entry will be set to migrated if a previous manifest
            //was used and the item successfully migrated.
            return !(item.ManifestEntry.HasMigrated);
        }
    }
}
