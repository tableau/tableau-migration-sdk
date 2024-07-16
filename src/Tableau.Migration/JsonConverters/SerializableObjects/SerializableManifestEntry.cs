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
using Tableau.Migration.Engine.Manifest;

namespace Tableau.Migration.JsonConverters.SerializableObjects
{
    /// <summary>
    /// Represents a JSON serializable entry in a migration manifest. This class implements <see cref="IMigrationManifestEntry"/>
    /// to allow for easy conversion between the manifest entry and its JSON representation.
    /// </summary>
    public class SerializableManifestEntry : IMigrationManifestEntry
    {
        /// <summary>
        /// Gets or sets the source content reference.
        /// </summary>
        public SerializableContentReference? Source { get; set; }

        /// <summary>
        /// Gets or sets the mapped location for the content.
        /// </summary>
        public SerializableContentLocation? MappedLocation { get; set; }

        /// <summary>
        /// Gets or sets the destination content reference.
        /// </summary>
        public SerializableContentReference? Destination { get; set; }

        /// <summary>
        /// Gets or sets the status of the migration for this entry.
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the content has been migrated.
        /// </summary>
        public bool HasMigrated { get; set; }

        /// <summary>
        /// Gets or sets the list of errors encountered during the migration of this entry.
        /// </summary>
        public List<SerializableException>? Errors { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableManifestEntry"/> class.
        /// </summary>
        public SerializableManifestEntry() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableManifestEntry"/> class with details from an <see cref="IMigrationManifestEntry"/>.
        /// </summary>
        /// <param name="entry">The migration manifest entry to serialize.</param>
        internal SerializableManifestEntry(IMigrationManifestEntry entry)
        {
            Source = new SerializableContentReference(entry.Source);
            MappedLocation = new SerializableContentLocation(entry.MappedLocation);
            Destination = entry.Destination == null ? null : new SerializableContentReference(entry.Destination);
            Status = (int)entry.Status;
            HasMigrated = entry.HasMigrated;

            Errors = entry.Errors.Select(e => new SerializableException(e)).ToList();
        }

        IContentReference IMigrationManifestEntry.Source => Source!.AsContentReferenceStub();

        ContentLocation IMigrationManifestEntry.MappedLocation => MappedLocation!.AsContentLocation();

        IContentReference? IMigrationManifestEntry.Destination => Destination?.AsContentReferenceStub();

        MigrationManifestEntryStatus IMigrationManifestEntry.Status => (MigrationManifestEntryStatus)Status;

        bool IMigrationManifestEntry.HasMigrated => HasMigrated;

        IReadOnlyList<Exception> IMigrationManifestEntry.Errors
        {
            get
            {
                if (Errors == null)
                {
                    return Array.Empty<Exception>();
                }
                else
                {
                    return Errors.Select(e => e.Error).ToImmutableArray();
                }
            }
        }

        /// <summary>
        /// Sets the list of errors encountered during the migration of this entry.
        /// </summary>
        /// <param name="errors">The list of errors to set.</param>
        public void SetErrors(List<Exception> errors)
        {
            Errors = errors.Select(e => new SerializableException(e)).ToList();
        }

        /// <summary>
        /// Throw exception if any values are still null
        /// </summary>
        public void VerifyDeseralization()
        {
            // Destination can be null, so we shouldn't do a nullability check on it

            Guard.AgainstNull(Source, nameof(Source));
            Guard.AgainstNull(MappedLocation, nameof(MappedLocation));

            Source.VerifyDeserialization();
            MappedLocation.VerifyDeseralization();
        }

        /// <summary>
        /// Returns current object as a <see cref="IMigrationManifestEntry"/>
        /// </summary>
        /// <returns></returns>
        public IMigrationManifestEntry AsMigrationManifestEntry(IMigrationManifestEntryBuilder partition)
        {
            VerifyDeseralization();
            var ret = new MigrationManifestEntry(partition, this);

            return ret;
        }

        /// <inheritdoc/>
        public bool Equals(IMigrationManifestEntry? other)
            => MigrationManifestEntry.Equals(this, other);
    }
}