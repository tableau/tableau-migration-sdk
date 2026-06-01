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
using Tableau.Migration.Engine.Manifest;

namespace Tableau.Migration.Engine.Hooks.Filters
{
    /// <summary>
    /// Context for <see cref="IContentFilter{TContent}"/> operations,
    /// determining whether a content item should be migrated and
    /// whether to cascade filtering to dependent content types.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    /// <param name="SourceItem"><inheritdoc /></param>
    /// <param name="ManifestEntry"><inheritdoc /></param>
    public record ContentFilterContextItem<TContent>(TContent SourceItem, IMigrationManifestEntryEditor ManifestEntry)
        : ContentMigrationItem<TContent>(SourceItem, ManifestEntry), IContentItemFilterResult
        where TContent : IContentReference
    {
        /// <summary>
        /// Gets or sets the current filtering status for the content item.
        /// </summary>
        public FilterStatus Status { get; set; } = FilterStatus.Migrate;

        /// <summary>
        /// Gets the record's equality contract type.
        /// </summary>
        /// <remarks>Overridden for <see cref="ContentMigrationItem{TContent}"/> equality rules.</remarks>
        protected override Type EqualityContract => typeof(ContentMigrationItem<TContent>);
    }
}
