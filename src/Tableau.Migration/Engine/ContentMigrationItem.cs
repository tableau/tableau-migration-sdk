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

using Tableau.Migration.Engine.Manifest;

namespace Tableau.Migration.Engine
{
    /// <summary>
    /// Record containing in-progress migration state for a content item.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    /// <param name="SourceItem">The content item's source data.</param>
    /// <param name="ManifestEntry">The manifest entry that describes the content item's overall migration status.</param>
    public record ContentMigrationItem<TContent>(TContent SourceItem, IMigrationManifestEntryEditor ManifestEntry)
    { }
}
