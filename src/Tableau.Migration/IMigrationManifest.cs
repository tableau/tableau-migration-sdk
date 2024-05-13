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
using Tableau.Migration.Engine.Manifest;

namespace Tableau.Migration
{
    /// <summary>
    /// Interface for an object that describes the various Tableau data items found to migrate and their migration results.
    /// </summary>
    public interface IMigrationManifest : IEquatable<IMigrationManifest>
    {
        /// <summary>
        /// Gets the unique identifier of the <see cref="IMigrationPlan"/> that was executed to produce this manifest.
        /// </summary>
        Guid PlanId { get; }

        /// <summary>
        /// Gets the unique identifier of the migration run that produced this manifest.
        /// </summary>
        Guid MigrationId { get; }

        /// <summary>
        /// Gets top-level errors that are not related to any Tableau content item but occurred during the migration.
        /// </summary>
        IReadOnlyList<Exception> Errors { get; }

        /// <summary>
        /// Gets the collection of manifest entries.
        /// </summary>
        IMigrationManifestEntryCollection Entries { get; }

        /// <summary>
        /// Gets the version of this manifest. Used for serialization.
        /// </summary>
        uint ManifestVersion { get; }
    }
}
