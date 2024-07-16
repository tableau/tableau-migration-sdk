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
using System.Collections.Immutable;
using System.Linq;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.Resources;

namespace Tableau.Migration.JsonConverters.SerializableObjects
{
    /// <summary>
    /// Represents a serializable version of a migration manifest, which can be used for JSON serialization and deserialization.
    /// </summary>
    public class SerializableMigrationManifest
    {
        /// <summary>
        /// Gets or sets the unique identifier for the migration plan.
        /// </summary>
        public Guid? PlanId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the migration.
        /// </summary>
        public Guid? MigrationId { get; set; }

        /// <summary>
        /// Gets or sets the list of errors encountered during the migration process.
        /// </summary>
        public List<SerializableException>? Errors { get; set; } = new List<SerializableException>();

        /// <summary>
        /// Gets or sets the collection of entries that are part of the migration manifest.
        /// </summary>
        public SerializableEntryCollection? Entries { get; set; } = new();

        /// <summary>
        /// Gets or sets the version of the migration manifest.
        /// </summary>
        public uint? ManifestVersion { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableMigrationManifest"/> class.
        /// </summary>
        public SerializableMigrationManifest() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableMigrationManifest"/> class with details from an <see cref="IMigrationManifest"/>.
        /// </summary>
        /// <param name="manifest">The migration manifest to serialize.</param>
        public SerializableMigrationManifest(IMigrationManifest manifest)
        {
            PlanId = manifest.PlanId;
            MigrationId = manifest.MigrationId;
            ManifestVersion = manifest.ManifestVersion;

            Errors = manifest.Errors.Select(e => new SerializableException(e)).ToList();

            foreach (var partitionType in manifest.Entries.GetPartitionTypes())
            {
                Guard.AgainstNull(partitionType, nameof(partitionType));
                Guard.AgainstNullOrEmpty(partitionType.FullName, nameof(partitionType.FullName));

                Entries.Add(partitionType!.FullName, manifest.Entries.ForContentType(partitionType).Select(entry => new SerializableManifestEntry(entry)).ToList());
            }
        }

        /// <summary>
        /// Converts the serializable migration manifest back into an <see cref="IMigrationManifest"/> instance.
        /// </summary>
        /// <param name="localizer">The shared resources localizer.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <returns>An instance of <see cref="IMigrationManifest"/>.</returns>
        public IMigrationManifest ToMigrationManifest(ISharedResourcesLocalizer localizer, ILoggerFactory loggerFactory)
        {
            VerifyDeserialization();

            // Create the manifest to return
            var manifest = new MigrationManifest(localizer, loggerFactory, PlanId!.Value, MigrationId!.Value);

            // Get the Tableau.Migration assembly to get the type from later
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            var tableauMigrationAssembly = loadedAssemblies.Where(a => a.ManifestModule.Name == "Tableau.Migration.dll").First();

            // Copy the entries to the manifest
            foreach (var partitionTypeStr in Entries!.Keys)
            {
                var partitionType = tableauMigrationAssembly.GetType(partitionTypeStr);
                Guard.AgainstNull(partitionType, nameof(partitionType));

                var partition = manifest.Entries.GetOrCreatePartition(partitionType);

                var partitionEntries = Entries.GetValueOrDefault(partitionTypeStr);
                Guard.AgainstNull(partitionEntries, nameof(partitionEntries));

                partition.CreateEntries(partitionEntries.ToImmutableArray());
            }

            manifest.AddErrors(Errors!.Where(e => e.Error is not null).Select(e => e.Error)!);

            return manifest;
        }

        internal void VerifyDeserialization()
        {
            Guard.AgainstNull(PlanId, nameof(PlanId));
            Guard.AgainstNull(MigrationId, nameof(MigrationId));
            Guard.AgainstNull(Errors, nameof(Errors));
            Guard.AgainstNull(Entries, nameof(Entries));
            Guard.AgainstNull(ManifestVersion, nameof(ManifestVersion));
        }
    }
}
