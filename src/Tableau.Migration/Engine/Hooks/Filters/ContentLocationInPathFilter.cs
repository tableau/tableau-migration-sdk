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
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Hooks.Filters
{
    /// <summary>
    /// Base filter for content specific filters. 
    /// Filters based on the <see cref="ContentLocation"/> field.
    /// </summary>
    public class ContentLocationInPathFilter<TContent> : ContentFilterBase<TContent>
        where TContent : IContentReference
    {
        /// <summary>
        /// Path for the filter.
        /// </summary>
        public readonly string Path;

        /// <summary>
        /// Default constructor for <see cref="ContentLocationInPathFilter{TContent}"/>.
        /// </summary>
        /// <param name="path">Path to filter out.</param>
        /// <param name="localizer">A string localizer.</param>
        /// <param name="logger">The default logger.</param>
        public ContentLocationInPathFilter(string path, ISharedResourcesLocalizer localizer, ILogger<IContentFilter<TContent>> logger) : base(localizer, logger) => Path = path;

        /// <inheritdoc/>
        public override bool ShouldMigrate(ContentMigrationItem<TContent> item)
            => Path.Contains(item.SourceItem.Location.Path);
    }
}
