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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Manifest
{
    /// <summary>
    /// Default <see cref="IMigrationManifestEntryCollection"/> implementation.
    /// </summary>
    public class MigrationManifestEntryCollection : IMigrationManifestEntryCollection, IMigrationManifestEntryCollectionEditor
    {
        private readonly ISharedResourcesLocalizer _localizer;
        private readonly ILogger<MigrationManifestContentTypePartition> _partitionLogger;

        private readonly List<MigrationManifestContentTypePartition> _partitions = new();

        /// <summary>
        /// Creates a new <see cref="MigrationManifestEntryCollection"/> object.
        /// </summary>
        /// <param name="localizer">A localizer.</param>
        /// <param name="loggerFactory">A logger factory.</param>
        /// <param name="copy">An optional collection to deep copy entries from.</param>
        public MigrationManifestEntryCollection(ISharedResourcesLocalizer localizer, ILoggerFactory loggerFactory,
            IMigrationManifestEntryCollection? copy = null)
        {
            _localizer = localizer;
            _partitionLogger = loggerFactory.CreateLogger<MigrationManifestContentTypePartition>();

            copy?.CopyTo(this);
        }

        #region - IMigrationManifestEntryCollection Implementation -

        /// <inheritdoc />
        public IMigrationManifestContentTypePartition ForContentType<TContent>() => ForContentType(typeof(TContent));

        /// <inheritdoc />
        public IMigrationManifestContentTypePartition ForContentType(Type contentType) => GetOrCreatePartition(contentType);

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<IMigrationManifestEntry> GetEnumerator()
        {
            foreach (var partition in _partitions)
            {
                foreach (var entry in partition)
                {
                    yield return entry;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc />
        public void CopyTo(IMigrationManifestEntryCollectionEditor copyTo)
        {
            foreach (var partition in _partitions)
            {
                copyTo.GetOrCreatePartition(partition.ContentType).CreateEntries(partition);
            }
        }

        /// <inheritdoc />
        public IEnumerable<Type> GetPartitionTypes()
        {
            return _partitions.Select(p => p.ContentType);
        }

        /// <inheritdoc />
        public virtual bool Equals(IMigrationManifestEntryCollection? other)
        {
            if (other is null)
                return false;

            // Check if they contain the same types
            if (this.GetPartitionTypes().SequenceEqual(other.GetPartitionTypes()) == false)
                return false;

            // They have the same types, so compare the entries in each partition
            foreach (var partition in _partitions)
            {
                if (this.ForContentType(partition.ContentType).Equals(other.ForContentType(partition.ContentType)) == false)
                    return false;
            }

            return true;
        }

        /// <inheritdoc />
        public override bool Equals(object? other)
        {
            if (other is null)
                return false;

            IMigrationManifestEntryCollection? entryCollectionObj = other as IMigrationManifestEntryCollection;
            if (entryCollectionObj is null)
                return false;
            else
                return Equals(entryCollectionObj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return _partitions.GetHashCode();
        }

        /// <inheritdoc/>
        public static bool operator ==(MigrationManifestEntryCollection? a, MigrationManifestEntryCollection? b)
        {
            if (a is null && b is null) return true;
            if (a is not null && b is null) return false;
            if (a is null & b is not null) return false;

            return a!.Equals(b);
        }

        /// <inheritdoc/>
        public static bool operator !=(MigrationManifestEntryCollection? a, MigrationManifestEntryCollection? b)
        {
            if (a is null && b is null) return false;
            if (a is not null && b is null) return true;
            if (a is null & b is not null) return true;

            return !(a!.Equals(b));
        }

        #endregion

        #region - IMigrationManifestBuilder Implementation -

        /// <inheritdoc />
        public IMigrationManifestContentTypePartitionEditor GetOrCreatePartition<TContent>() => GetOrCreatePartition(typeof(TContent));

        /// <inheritdoc />
        public IMigrationManifestContentTypePartitionEditor GetOrCreatePartition(Type contentType)
        {
            foreach (var partition in _partitions)
            {
                if (partition.ContentType == contentType)
                {
                    return partition;
                }
            }

            var newPartition = new MigrationManifestContentTypePartition(contentType, _localizer, _partitionLogger);
            _partitions.Add(newPartition);

            return newPartition;
        }

        #endregion
    }
}
