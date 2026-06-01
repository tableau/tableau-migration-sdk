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

using Tableau.Migration.Engine.Manifest;

namespace Tableau.Migration.Engine.Hooks.Filters
{
    /// <summary>
    /// Interface for a hook result that allows a content item to be filtered out from migration.
    /// </summary>
    public interface IContentItemFilterResult
    {
        /// <summary>
        /// Gets the manifest entry for the content item.
        /// </summary>
        IMigrationManifestEntryEditor ManifestEntry { get; }

        /// <summary>
        /// Gets the current filtering status for the pulled content item.
        /// </summary>
        FilterStatus Status { get; }
    }
}
