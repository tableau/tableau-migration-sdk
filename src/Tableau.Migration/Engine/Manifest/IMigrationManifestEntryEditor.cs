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
    /// Interface for a <see cref="IMigrationManifestEntry"/> that can be edited.
    /// </summary>
    public interface IMigrationManifestEntryEditor : IMigrationManifestEntry
    {
        /// <summary>
        /// Sets the intended mapped destination location to the manifest entry.
        /// Clears the <see cref="IMigrationManifestEntry.Destination"/> information if the mapped location is different.
        /// </summary>
        /// <param name="destinationLocation">The intended destination location to migrate to.</param>
        /// <returns>The current entry editor, for fluent API usage.</returns>
        IMigrationManifestEntryEditor MapToDestination(ContentLocation destinationLocation);

        /// <summary>
        /// Sets the <see cref="IMigrationManifestEntry.Destination"/> and 
        /// <see cref="IMigrationManifestEntry.MappedLocation"/> information
        /// based on the given destination item reference.
        /// </summary>
        /// <param name="destinationInfo">The destination reference information.</param>
        /// <returns>The current entry editor, for fluent API usage.</returns>
        IMigrationManifestEntryEditor DestinationFound(IContentReference destinationInfo);

        /// <summary>
        /// Sets the entry to skipped status.
        /// </summary>
        /// <returns>The current entry editor, for fluent API usage.</returns>
        IMigrationManifestEntryEditor SetSkipped();

        /// <summary>
        /// Sets the entry to failed status and adding errors to the entry.
        /// </summary>
        /// <param name="errors">The errors to add to the entry.</param>
        /// <returns>The current entry editor, for fluent API usage.</returns>
        IMigrationManifestEntryEditor SetFailed(IEnumerable<Exception> errors);

        /// <summary>
        /// Sets the entry to failed status and adding errors to the entry.
        /// </summary>
        /// <param name="errors">The errors to add to the entry.</param>
        /// <returns>The current entry editor, for fluent API usage.</returns>
        IMigrationManifestEntryEditor SetFailed(params Exception[] errors);

        /// <summary>
        /// Sets the entry to canceled status.
        /// </summary>
        /// <returns>The current entry editor, for fluent API usage.</returns>
        IMigrationManifestEntryEditor SetCanceled();

        /// <summary>
        /// Sets the entry to migrated status.
        /// </summary>
        /// <returns>The current entry editor, for fluent API usage.</returns>
        IMigrationManifestEntryEditor SetMigrated();
    }
}
