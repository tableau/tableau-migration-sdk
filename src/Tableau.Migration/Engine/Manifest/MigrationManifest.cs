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
using System.Linq;

namespace Tableau.Migration.Engine.Manifest
{
    /// <summary>
    /// Default <see cref="IMigrationManifest"/> implementation.
    /// </summary>
    public class MigrationManifest : IMigrationManifestEditor
    {
        private readonly MigrationManifestEntryCollection _entries;
        private readonly List<Exception> _errors;

        // This should be updated if the manifest changes and a previous manifest 
        // is not compatible with current version. Specifically if the serializer 
        // won't be able to deserialize it.

        /// <summary>
        /// The latest manifest version number.
        /// </summary>
        public const uint LatestManifestVersion = 4;

        /// <summary>
        /// Creates a new <see cref="MigrationManifest"/> object.
        /// </summary>
        /// <param name="copyEntriesManifest">
        /// An optional manifest to copy entries from.
        /// Null will initialize the manifest with an empty set of entries. 
        /// Top-level information is not copied.
        /// </param>
        public MigrationManifest(IMigrationManifest copyEntriesManifest)
            : this(Guid.NewGuid(), Guid.NewGuid(), copyEntriesManifest.PipelineProfile, copyEntriesManifest)
        { }

        /// <summary>
        /// Creates a new <see cref="MigrationManifest"/> object.
        /// </summary>
        /// <param name="pipelineProfile"><inheritdoc cref="IMigrationManifest.PipelineProfile"/></param>
        public MigrationManifest(PipelineProfile pipelineProfile)
            : this(Guid.NewGuid(), Guid.NewGuid(), pipelineProfile)
        { }

        /// <summary>
        /// Creates a new <see cref="MigrationManifest"/> object.
        /// </summary>
        /// <param name="planId"><inheritdoc cref="IMigrationManifest.PlanId"/></param>
        /// <param name="migrationId"><inheritdoc cref="IMigrationManifest.MigrationId"/></param>
        /// <param name="pipelineProfile"><inheritdoc cref="IMigrationManifest.PipelineProfile"/></param>
        /// <param name="copyEntriesManifest">
        /// An optional manifest to copy entries from.
        /// Null will initialize the manifest with an empty set of entries. 
        /// Top-level information is not copied.
        /// </param>
        public MigrationManifest(Guid planId, Guid migrationId, PipelineProfile pipelineProfile, IMigrationManifest? copyEntriesManifest = null)
        {
            _errors = new();

            PlanId = planId;
            MigrationId = migrationId;

            if (copyEntriesManifest != null)
            {
                if (copyEntriesManifest.PipelineProfile != pipelineProfile)
                {
                    throw new ArgumentException($"PipelineProfile must match the {nameof(copyEntriesManifest)}'s PipelineProfile");
                }
            }

            PipelineProfile = pipelineProfile;

            _entries = new(copyEntriesManifest?.Entries);
        }

        #region - IMigrationManifest Implementation -

        /// <inheritdoc />
        public Guid PlanId { get; }

        /// <inheritdoc />
        public Guid MigrationId { get; }

        /// <inheritdoc/>
        public PipelineProfile PipelineProfile { get; }

        /// <inheritdoc />
        public virtual uint ManifestVersion => LatestManifestVersion;

        /// <inheritdoc />
        IMigrationManifestEntryCollection IMigrationManifest.Entries => _entries;

        /// <inheritdoc />
        public IReadOnlyList<Exception> Errors => _errors;

        #endregion

        #region - IEquality Implementation -

        /// <inheritdoc />
        public bool Equals(IMigrationManifest? other)
        {
            if (other is null) return false;

            var equal =
                PlanId.Equals(other.PlanId) &&
                MigrationId.Equals(other.MigrationId) &&
                PipelineProfile.Equals(other.PipelineProfile) &&
                ManifestVersion.Equals(other.ManifestVersion) &&
                Entries.Equals(other.Entries) &&
                Errors.SequenceEqual(other.Errors, new ExceptionComparer());
            return equal;
        }

        /// <inheritdoc />
        public override bool Equals(object? other)
        {
            if (other is null)
                return false;

            var otherAsIMigrationManifest = other as IMigrationManifest;

            if (otherAsIMigrationManifest is null)
                return false;
            else
                return Equals(otherAsIMigrationManifest);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(PlanId, MigrationId, ManifestVersion, Entries);
        }

        /// <inheritdoc/>
        public static bool operator ==(MigrationManifest? a, MigrationManifest? b)
        {
            if (a is null && b is null) return true;
            if (a is not null && b is null) return false;
            if (a is null & b is not null) return false;

            return a!.Equals(b);
        }

        /// <inheritdoc/>
        public static bool operator !=(MigrationManifest? a, MigrationManifest? b)
        {
            if (a is null && b is null) return false;
            if (a is not null && b is null) return true;
            if (a is null & b is not null) return true;

            return !(a!.Equals(b));
        }

        #endregion

        #region - IMigrationManifestEditor Implementation -

        /// <inheritdoc />
        public virtual IMigrationManifestEntryCollectionEditor Entries => _entries;

        /// <inheritdoc />
        public virtual IMigrationManifestEditor AddErrors(params IEnumerable<Exception> errors)
        {
            _errors.AddRange(errors);
            return this;
        }

        /// <inheritdoc />
        public virtual IMigrationManifestEditor AddErrors(params Exception[] errors)
            => AddErrors((IEnumerable<Exception>)errors); //Overload for Python interop.

        #endregion
    }
}
