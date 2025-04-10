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
using System.Collections.Generic;
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.Engine.Pipelines;
using Tableau.Migration.JsonConverters.SerializableObjects;

namespace Tableau.Migration.ManifestExplorer.ViewModels
{
    internal sealed class DesignManifestViewModel : ManifestViewModel
    {
        public DesignManifestViewModel()
            : base(BuildDesignManifest(), new DesignMainViewModel())
        { }

        private static IMigrationManifest BuildDesignManifest()
        {
            var manifest = new SerializableMigrationManifest
            {
                Entries = new(),
                Errors = new(),
                PipelineProfile = PipelineProfile.ServerToCloud,
                ManifestVersion = MigrationManifest.LatestManifestVersion,
                MigrationId = Guid.NewGuid(),
                PlanId = Guid.NewGuid()
            };

            SerializableManifestEntry CreateEntry(MigrationPipelineContentType type, MigrationManifestEntryStatus status, params string[] path)
            {
                var sourceLocation = ContentLocation.ForContentType(type.ContentType, path);

                return new SerializableManifestEntry
                {
                    Source = new() { Id = Guid.NewGuid().ToString(), ContentUrl = string.Empty, Location = new(sourceLocation), Name = path[^1] },
                    MappedLocation = new(sourceLocation),
                    Status = status.ToString()
                };
            }

            foreach (var contentType in MigrationPipelineContentType.GetMigrationPipelineContentTypes(manifest.PipelineProfile.Value))
            {
                var entries = new List<SerializableManifestEntry>
                {
                    CreateEntry(contentType, MigrationManifestEntryStatus.Migrated, "Migrated"),
                    CreateEntry(contentType, MigrationManifestEntryStatus.Migrated, "Parent", "Migrated"),
                    CreateEntry(contentType, MigrationManifestEntryStatus.Skipped, "Skipped"),
                    CreateEntry(contentType, MigrationManifestEntryStatus.Skipped, "Parent", "Skipped"),
                };

                manifest.Entries.Add(contentType.ContentType.FullName!, entries);
            }

            return manifest.ToMigrationManifest();
        }
    }
}
