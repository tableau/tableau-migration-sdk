using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
        /// <param name="previousMigrationEntry">An entry from a previous migration manifest to copy values from.</param>
        public MigrationManifestEntry(IMigrationManifestEntryBuilder entryBuilder,
            IMigrationManifestEntry previousMigrationEntry)
        {
            _entryBuilder = entryBuilder;
            Source = previousMigrationEntry.Source;
            MappedLocation = previousMigrationEntry.MappedLocation;
            
            //Status is reset since this is a new run.
            //The HasMigrated flag is copied since it tracks whether the 
            //item has migrated in any run.
            HasMigrated = previousMigrationEntry.HasMigrated;

            //Errors are reset since this is a new migration.
            
            //Destination is reset in case our IDs are out-of-date
            //(e.g. a user manually deleted between runs)
        }

        /// <summary>
        /// Creates a new <see cref="MigrationManifestEntry"/> object.
        /// </summary>
        /// <param name="entryBuilder">The entry builder to notify with changes.</param>
        /// <param name="previousMigrationEntry">An entry from a previous migration manifest to copy values from.</param>
        /// <param name="sourceReference">The content item's updated source information, as a stub.</param>
        public MigrationManifestEntry(IMigrationManifestEntryBuilder entryBuilder,
            IMigrationManifestEntry previousMigrationEntry, ContentReferenceStub sourceReference)
            : this(entryBuilder, previousMigrationEntry)
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
        public virtual MigrationManifestEntryStatus Status { get; private set; }

        /// <inheritdoc />
        public virtual bool HasMigrated { get; private set; }

        /// <inheritdoc />
        public virtual IReadOnlyList<Exception> Errors => _errors ?? ImmutableArray<Exception>.Empty;

        /// <summary>
        /// Indicates if the current IMigrationManifestEntry is equal to another IMigrationManifestEntry.
        /// *Note: This ignores if the errors are different*
        /// </summary>
        /// <returns>True if the current object is equal to the other parameter. Otherwise false.</returns>
        public static bool Equals(IMigrationManifestEntry entry, IMigrationManifestEntry? other)
        {
            if (other is null)
                return false;

            if ((entry.Source.Equals(other.Source) && entry.MappedLocation.Equals(other.MappedLocation) && entry.Status.Equals(other.Status)) == false)
                return false;

            // Nullability of Destination must match
            if (!Object.ReferenceEquals(entry.Destination, other.Destination))
            {
                if (entry.Destination is null || other.Destination is null)
                {
                    return false;
                }
                else if (!entry.Destination.Equals(other.Destination))
                {
                    return false;
                }
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
        public virtual IMigrationManifestEntryEditor SetSkipped()
        {
            Status = MigrationManifestEntryStatus.Skipped;
            return this;
        }

        /// <inheritdoc />
        public virtual IMigrationManifestEntryEditor SetFailed(IEnumerable<Exception> errors)
        {
            _errors = errors.ToImmutableArray();
            Status = MigrationManifestEntryStatus.Error;

            _entryBuilder.MigrationFailed(this);

            return this;
        }

        /// <inheritdoc />
        public virtual IMigrationManifestEntryEditor SetFailed(params Exception[] errors)
            => SetFailed((IEnumerable<Exception>)errors);

        /// <inheritdoc />
        public virtual IMigrationManifestEntryEditor SetCanceled()
        {
            Status = MigrationManifestEntryStatus.Canceled;
            return this;
        }

        /// <inheritdoc />
        public virtual IMigrationManifestEntryEditor SetMigrated()
        {
            Status = MigrationManifestEntryStatus.Migrated;
            HasMigrated = true;
            return this;
        }

        #endregion
    }
}
