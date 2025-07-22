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
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Hooks.Mappings;

namespace Tableau.Migration.Engine.Manifest
{
    /// <summary>
    /// Object representing a partition of the manifest entry collection for a specific content type.
    /// </summary>
    public class MigrationManifestContentTypePartition : IMigrationManifestContentTypePartitionEditor, IMigrationManifestEntryBuilder
    {
        private readonly Dictionary<ContentLocation, IMigrationManifestEntryEditor> _entriesBySourceLocation = new();
        private readonly Dictionary<Guid, IMigrationManifestEntryEditor> _entriesBySourceId = new();
        private readonly Dictionary<string, IMigrationManifestEntryEditor> _entriesBySourceContentUrl = new();

        //Entry destination info can be updated in the multithreaded batch,
        //so thread safe collections are necessary.
        private readonly ConcurrentDictionary<ContentLocation, IMigrationManifestEntryEditor> _entriesByMappedLocation = new();
        private readonly ConcurrentDictionary<Guid, IMigrationManifestEntryEditor> _entriesByDestinationId = new();

        private readonly ConcurrentDictionary<MigrationManifestEntryStatus, int> _statusTotals = new();

        /// <summary>
        /// Creates a new <see cref="MigrationManifestContentTypePartition"/> object.
        /// </summary>
        /// <param name="type">The content type the partition holds manifest entries for.</param>
        public MigrationManifestContentTypePartition(Type type)
        {
            ContentType = type;

            foreach (var status in Enum.GetValues<MigrationManifestEntryStatus>())
            {
                _statusTotals[status] = 0;
            }
        }

        private void IncrementStatus(MigrationManifestEntryStatus status)
        {
            _statusTotals.AddOrUpdate(status, 1, (k, v) => v + 1);
        }

        #region - IMigrationManifestContentTypePartitionEditor Implementation -

        /// <inheritdoc />
        public IReadOnlyDictionary<ContentLocation, IMigrationManifestEntryEditor> BySourceLocation => _entriesBySourceLocation;

        /// <inheritdoc />
        public IReadOnlyDictionary<Guid, IMigrationManifestEntryEditor> BySourceId => _entriesBySourceId;

        /// <inheritdoc />
        public IReadOnlyDictionary<string, IMigrationManifestEntryEditor> BySourceContentUrl => _entriesBySourceContentUrl;

        /// <inheritdoc />
        public IReadOnlyDictionary<ContentLocation, IMigrationManifestEntryEditor> ByMappedLocation => _entriesByMappedLocation;

        /// <inheritdoc />
        public IReadOnlyDictionary<Guid, IMigrationManifestEntryEditor> ByDestinationId => _entriesByDestinationId;

        /// <inheritdoc />
        public IMigrationManifestContentTypePartitionEditor CreateEntries(IReadOnlyCollection<IMigrationManifestEntry> entriesToCopy)
        {

            _entriesBySourceLocation.EnsureCapacity(_entriesBySourceLocation.Count + entriesToCopy.Count);
            _entriesBySourceId.EnsureCapacity(_entriesBySourceId.Count + entriesToCopy.Where(e => e.Source.Id != Guid.Empty).Count());

            foreach (var entryToCopy in entriesToCopy)
            {
                var clonedEntry = new MigrationManifestEntry(this, entryToCopy);
                _entriesBySourceLocation.Add(clonedEntry.Source.Location, clonedEntry);

                if (clonedEntry.Source.Id != Guid.Empty)
                {
                    _entriesBySourceId.Add(clonedEntry.Source.Id, clonedEntry);
                }

                if (!string.IsNullOrEmpty(clonedEntry.Source.ContentUrl))
                {
                    _entriesBySourceContentUrl.Add(clonedEntry.Source.ContentUrl, clonedEntry);
                }

                IncrementStatus(clonedEntry.Status);
            }

            ExpectedTotalCount = Count;

            return this;
        }

        /// <inheritdoc />
        public IMigrationManifestEntryBuilder GetEntryBuilder(int totalItemCount)
        {
            _entriesBySourceLocation.EnsureCapacity(totalItemCount);
            return this;
        }

        /// <inheritdoc />
        public ImmutableArray<TResultItem> CreateEntries<TItem, TResultItem>(IReadOnlyCollection<TItem> sourceContentItems,
            Func<TItem, IMigrationManifestEntryEditor, TResultItem> resultFactory, int expectedTotalCount)
            where TItem : IContentReference
        {
            var results = ImmutableArray.CreateBuilder<TResultItem>(sourceContentItems.Count);

            foreach (var sourceItem in sourceContentItems)
            {
                var sourceReferenceStub = new ContentReferenceStub(sourceItem);
                if (!_entriesBySourceLocation.TryGetValue(sourceItem.Location, out var manifestEntry))
                {
                    _entriesBySourceLocation.Add(sourceItem.Location, manifestEntry = new MigrationManifestEntry(this, sourceReferenceStub));

                    if (sourceItem.Id != Guid.Empty)
                    {
                        _entriesBySourceId.Add(sourceItem.Id, manifestEntry);
                    }

                    if (!string.IsNullOrEmpty(sourceItem.ContentUrl))
                    {
                        _entriesBySourceContentUrl.Add(sourceItem.ContentUrl, manifestEntry);
                    }

                    IncrementStatus(manifestEntry.Status);
                }
                else
                {
                    //Update the source information in case source IDs/content URLs changed,
                    //but maintain any existing previous run information.
                    var existing = _entriesBySourceLocation[sourceItem.Location];
                    manifestEntry = new MigrationManifestEntry(this, existing, sourceReferenceStub);

                    _entriesBySourceLocation[sourceItem.Location] = manifestEntry;

                    _entriesBySourceId.Remove(existing.Source.Id);

                    if (sourceItem.Id != Guid.Empty)
                    {
                        _entriesBySourceId.Add(sourceItem.Id, manifestEntry);
                    }

                    if (!string.IsNullOrEmpty(existing.Source.ContentUrl))
                    {
                        _entriesBySourceContentUrl.Remove(existing.Source.ContentUrl);
                    }

                    if (!string.IsNullOrEmpty(sourceItem.ContentUrl))
                    {
                        _entriesBySourceContentUrl.Add(sourceItem.ContentUrl, manifestEntry);
                    }
                }

                var result = resultFactory(sourceItem, manifestEntry);
                results.Add(result);
            }

            //Set expected total count, but it should never be less than the actual count.
            ExpectedTotalCount = Count > expectedTotalCount ? Count : expectedTotalCount;

            return results.ToImmutable();
        }

        /// <inheritdoc />
        public async Task<IMigrationManifestEntryBuilder> MapEntriesAsync<TItem>(IEnumerable<TItem> sourceContentItems, IContentMappingRunner mapper, CancellationToken cancel)
            where TItem : IContentReference
        {
            foreach (var sourceItem in sourceContentItems)
            {
                var sourceLocation = sourceItem.Location;
                if (_entriesBySourceLocation.TryGetValue(sourceLocation, out var entry))
                {
                    var mapping = new ContentMappingContext<TItem>(sourceItem, entry.Source.Location);
                    mapping = await mapper.ExecuteAsync(mapping, cancel).ConfigureAwait(false);

                    entry.MapToDestination(mapping.MappedLocation);
                    _entriesByMappedLocation[mapping.MappedLocation] = entry;
                }
            }

            return this;
        }

        /// <inheritdoc />
        public void DestinationInfoUpdated(IMigrationManifestEntryEditor entry, IContentReference? oldDestinationInfo)
        {
            if (oldDestinationInfo is not null)
            {
                _entriesByDestinationId.TryRemove(oldDestinationInfo.Id, out _);
                _entriesByMappedLocation.TryRemove(oldDestinationInfo.Location, out _);
            }

            if (entry.Destination is not null && entry.Destination.Id != Guid.Empty)
            {
                _entriesByDestinationId[entry.Destination.Id] = entry;
            }

            _entriesByMappedLocation[entry.MappedLocation] = entry;
        }

        /// <inheritdoc />
        public void StatusUpdated(IMigrationManifestEntryEditor entry, MigrationManifestEntryStatus oldStatus)
        {
            _statusTotals.AddOrUpdate(oldStatus, 0, (k, v) => v - 1);
            IncrementStatus(entry.Status);
        }

        /// <inhertidoc />
        public virtual void MigrationFailed(IMigrationManifestEntryEditor entry) { }

        #endregion

        #region - IMigrationManifestContentTypePartition Implementation -

        /// <inheritdoc />
        public Type ContentType { get; }

        /// <inheritdoc />
        public int ExpectedTotalCount { get; private set; }

        /// <inheritdoc />
        public ImmutableDictionary<MigrationManifestEntryStatus, int> GetStatusTotals()
            // ConcurrentDictionary<T,V.ToArray is atomic.
            => _statusTotals.ToArray().ToImmutableDictionary();

        #region - IEquatable Implementation -

        /// <inheritdoc />
        public bool Equals(IMigrationManifestContentTypePartition? other)
        {
            if (other is null)
                return false;

            if (ContentType != other.ContentType)
                return false;

            return this.SequenceEqual(other);
        }

        /// <inheritdoc />
        public override bool Equals(object? other)
        {
            if (other is null)
                return false;

            var otherAsIMigrationManifestContentTypePartition = other as IMigrationManifestContentTypePartition;
            if (otherAsIMigrationManifestContentTypePartition == null)
                return false;
            else
                return this.Equals(otherAsIMigrationManifestContentTypePartition);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(_entriesBySourceLocation, ContentType);
        }

        /// <inheritdoc/>
        public static bool operator ==(MigrationManifestContentTypePartition? a, MigrationManifestContentTypePartition? b)
        {
            if (a is null && b is null) return true;
            if (a is not null && b is null) return false;
            if (a is null & b is not null) return false;

            return a!.Equals(b);
        }

        /// <inheritdoc/>
        public static bool operator !=(MigrationManifestContentTypePartition? a, MigrationManifestContentTypePartition? b)
        {
            if (a is null && b is null) return false;
            if (a is not null && b is null) return true;
            if (a is null & b is not null) return true;

            return !(a!.Equals(b));
        }

        #endregion

        #endregion

        #region - IReadOnlyCollection<IMigrationManifestEntryEditor> Implementation -

        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        public int Count => _entriesBySourceLocation.Count;

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<IMigrationManifestEntryEditor> GetEnumerator() => _entriesBySourceLocation.Values.GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator<IMigrationManifestEntry> IEnumerable<IMigrationManifestEntry>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }
}
