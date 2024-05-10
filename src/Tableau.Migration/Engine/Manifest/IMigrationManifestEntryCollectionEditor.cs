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

using System;

namespace Tableau.Migration.Engine.Manifest
{
    /// <summary>
    /// Interface for an editable migration manifest.
    /// </summary>
    public interface IMigrationManifestEntryCollectionEditor : IMigrationManifestEntryCollection
    {
        /// <summary>
        /// Creates a partition of manifest entries for a given content type.
        /// </summary>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <returns>The created partition to add manifest entries to.</returns>
        IMigrationManifestContentTypePartitionEditor GetOrCreatePartition<TContent>();

        /// <summary>
        /// Creates a partition of manifest entries for a given content type.
        /// </summary>
        /// <param name="contentType">The content type.</param>
        /// <returns>The created partition to add manifest entries to.</returns>
        IMigrationManifestContentTypePartitionEditor GetOrCreatePartition(Type contentType);
    }
}
