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
using System.Linq;

namespace Tableau.Migration.Engine.Hooks.Filters
{
    /// <summary>
    /// Context for <see cref="IContentFilter{TContent}"/> operations,
    /// determining which items should be migrated and
    /// whether to cascade filtering to dependent content types.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    public class ContentFilterContext<TContent>
        where TContent : IContentReference
    {
        /// <summary>
        /// Gets the items to potentially filter.
        /// </summary>
        public IImmutableList<ContentFilterContextItem<TContent>> Items { get; }

        /// <summary>
        /// Creates a new <see cref="ContentFilterContext{TContent}"/> object.
        /// </summary>
        /// <param name="items">The items to consider for filtering.</param>
        public ContentFilterContext(params IEnumerable<ContentMigrationItem<TContent>> items)
        {
            Items = items
                .Select(i => new ContentFilterContextItem<TContent>(i.SourceItem, i.ManifestEntry))
                .ToImmutableArray();
        }
    }
}
