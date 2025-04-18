﻿//
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
using System.Collections.Immutable;
using System.Linq;
using Tableau.Migration.Content;

namespace Tableau.Migration.Engine.Manifest
{
    /// <summary>
    /// Default <see cref="IMigrationManifestEntry"/> and <see cref="IMigrationManifestEntryEditor"/> implementation.
    /// </summary>
    public class MigrationManifestEntry : IMigrationManifestEntryEditor
    {
        private ImmutableArray<Exception>? _errors; //Only allocate a list of errors when we need to, since there may be a large number of entries.

        private IMigrationManifestEntryBuilder _entryBuilder;

        /// <summary>
        /// Creates a new <see cref="MigrationManifestEntry"/> object.
        /// </summary>
        /// <param name="entryBuilder">The entry builder to notify with changes.</param>
        /// <param name="sourceReference">The content item's source information, as a stub.</param>
        public MigrationManifestEntry(IMigrationManifestEntryBuilder entryBuilder,
            ContentReferenceStub sourceReference)
        {
            _entryBuilder = entryBuilder;
            Source = sourceReference;
            MappedLocation = sourceReference.Location;
        }

        /// <summary>
        /// Creates a new <see cref="MigrationManifestEntry"/> object.
        /// </summary>
        /// <param name="entryBuilder">The entry builder to notify with changes.</param>
        /// <param name="copy">An entry from to copy values from.</param>
        public MigrationManifestEntry(IMigrationManifestEntryBuilder entryBuilder,
            IMigrationManifestEntry copy)
        {
            _entryBuilder = entryBuilder;
            Source = copy.Source;
            MappedLocation = copy.MappedLocation;
            _status = copy.Status;
            _skippedReason = copy.SkippedReason;
            Destination = copy.Destination;
            HasMigrated = copy.HasMigrated;
            _errors = copy.Errors.ToImmutableArray();
        }

        /// <summary>
        /// Creates a new <see cref="MigrationManifestEntry"/> object.
        /// </summary>
        /// <param name="entryBuilder">The entry builder to notify with changes.</param>
        /// <param name="copy">An entry to copy values from.</param>
        /// <param name="sourceReference">The content item's updated source information, as a stub.</param>
        public MigrationManifestEntry(IMigrationManifestEntryBuilder entryBuilder,
            IMigrationManifestEntry copy, ContentReferenceStub sourceReference)
            : this(entryBuilder, copy)
        {
            Source = sourceReference;
        }

        #region - IMigrationManifestEntry Implementation -

        /// <inheritdoc />
        public virtual IContentReference Source { get; }

        /// <inheritdoc />
        public virtual ContentLocation MappedLocation { get; private set; }

        /// <inheritdoc />
        public virtual IContentReference? Destination
        {
            get => _destination;
            set
            {
                var oldDestinationInfo = _destination;
                _destination = value;

                _entryBuilder.DestinationInfoUpdated(this, oldDestinationInfo);
            }
        }
        private IContentReference? _destination;

        /// <inheritdoc />
        public virtual MigrationManifestEntryStatus Status
        {
            get => _status;
            set
            {
                var oldStatus = _status;
                _status = value;

                if (oldStatus != _status)
                {
                    _entryBuilder.StatusUpdated(this, oldStatus);
                }
            }
        }
        private MigrationManifestEntryStatus _status;

        /// <inheritdoc />
        public virtual string SkippedReason
        {
            get => _skippedReason;
            set
            {
                var oldStatus = _status;
                _skippedReason = value;

                if (oldStatus != _status)
                {
                    _entryBuilder.StatusUpdated(this, oldStatus);
                }
            }
        }
        private string _skippedReason = string.Empty;

        /// <inheritdoc />
        public virtual bool HasMigrated { get; private set; }

        /// <inheritdoc />
        public virtual IReadOnlyList<Exception> Errors => _errors ?? ImmutableArray<Exception>.Empty;

        /// <summary>
        /// Indicates if the current IMigrationManifestEntry is equal to another IMigrationManifestEntry.
        /// </summary>
        /// <returns>True if the current object is equal to the other parameter. Otherwise false.</returns>
        public static bool Equals(IMigrationManifestEntry entry, IMigrationManifestEntry? other)
        {
            if (other is null)
                return false;

            if (!entry.Source.Equals(other.Source) ||
                !entry.MappedLocation.Equals(other.MappedLocation) ||
                !entry.Status.Equals(other.Status) ||
                !entry.SkippedReason.Equals(other.SkippedReason) ||
                !entry.Errors.SequenceEqual(other.Errors, new ExceptionComparer()))
                return false;

            // Nullability of Destination must match
            if (entry.Destination != other.Destination)
            {
                if (entry.Destination is null || other.Destination is null)
                    return false;

                return entry.Destination.Equals(other.Destination);
            }

            return true;
        }

        /// <summary>
        /// Indicates if the current IMigrationManifestEntry is equal to another IMigrationManifestEntry.
        /// *Note: This ignores if the errors are different*
        /// </summary>
        /// <returns>True if the current object is equal to the other parameter. Otherwise false.</returns>
        public bool Equals(IMigrationManifestEntry? other) => Equals(this, other);

        /// <inheritdoc />
        public override bool Equals(object? other)
        {
            if (other is null || !(other is IMigrationManifestEntry entry))
                return false;

            return Equals(entry);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(Source, MappedLocation, Status, Destination);
        }

        /// <inheritdoc/>
        public static bool operator ==(MigrationManifestEntry? a, MigrationManifestEntry? b)
            => a?.Equals(b) ?? false;

        /// <inheritdoc/>
        public static bool operator !=(MigrationManifestEntry? a, MigrationManifestEntry? b)
            => !(a == b);

        #endregion

        #region - IMigrationManifestEntryEditor Implementation -

        /// <inheritdoc />
        public virtual IMigrationManifestEntryEditor ResetStatus()
        {
            // Mapped location, destination info, and long-term migrated flag are not reset between migrations.

            _errors = ImmutableArray<Exception>.Empty;
            Status = MigrationManifestEntryStatus.Pending;
            SkippedReason = string.Empty;

            return this;
        }

        /// <inheritdoc />
        public virtual IMigrationManifestEntryEditor MapToDestination(ContentLocation destinationLocation)
        {
            MappedLocation = destinationLocation;
            if (Destination is not null && Destination.Location != MappedLocation)
            {
                Destination = null;
            }

            return this;
        }

        /// <inheritdoc />
        public virtual IMigrationManifestEntryEditor DestinationFound(IContentReference destinationInfo)
        {
            MappedLocation = destinationInfo.Location;
            Destination = destinationInfo;
            return this;
        }

        /// <inheritdoc />
        public virtual IMigrationManifestEntryEditor SetSkipped(string? skippedReason)
        {
            Status = MigrationManifestEntryStatus.Skipped;
            if (!string.IsNullOrWhiteSpace(skippedReason))
            {
                SkippedReason = skippedReason;
            }

            return this;
        }

        /// <inheritdoc />
        public virtual IMigrationManifestEntryEditor SetFailed(params IEnumerable<Exception> errors)
        {
            _errors = errors.ToImmutableArray();
            Status = MigrationManifestEntryStatus.Error;
            SkippedReason = string.Empty;

            _entryBuilder.MigrationFailed(this);

            return this;
        }

        /// <inheritdoc />
        public virtual IMigrationManifestEntryEditor SetCanceled()
        {
            Status = MigrationManifestEntryStatus.Canceled;
            SkippedReason = string.Empty;

            return this;
        }

        /// <inheritdoc />
        public virtual IMigrationManifestEntryEditor SetMigrated()
        {
            Status = MigrationManifestEntryStatus.Migrated;
            SkippedReason = string.Empty;
            HasMigrated = true;

            return this;
        }

        #endregion
    }
}
