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
using System.Collections.Generic;

namespace Tableau.Migration.Engine.Manifest
{
    /// <summary>
    /// Interface for an object that contains the entries of the migration manifest.
    /// </summary>
    public interface IMigrationManifestEntryCollection : IEnumerable<IMigrationManifestEntry>, IEquatable<IMigrationManifestEntryCollection>
    {
        /// <summary>
        /// Gets the manifest entries for a given content type.
        /// </summary>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <returns>The set of manifest entries for the content type.</returns>
        IMigrationManifestContentTypePartition ForContentType<TContent>();

        /// <summary>
        /// Gets the manifest entries for a given content type.
        /// </summary>
        /// <param name="contentType">The content type.</param>
        /// <returns>The set of manifest entries for the content type.</returns>
        IMigrationManifestContentTypePartition ForContentType(Type contentType);

        /// <summary>
        /// Deep copies all the entries in this collection to another collection.
        /// </summary>
        /// <param name="copyTo">The collection to deep copy entries to.</param>
        void CopyTo(IMigrationManifestEntryCollectionEditor copyTo);

        /// <summary>
        /// Returns a list of all the partition type this collection has.
        /// </summary>
        /// <returns>List of partion types</returns>
        IEnumerable<Type> GetPartitionTypes();
    }
}
