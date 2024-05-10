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

namespace Tableau.Migration.Engine.Manifest
{
    /// <summary>
    /// Interface for an object that can create <see cref="IMigrationManifest"/> objects.
    /// </summary>
    public interface IMigrationManifestFactory
    {
        /// <summary>
        /// Creates a new <see cref="IMigrationManifest"/> object.
        /// </summary>
        /// <param name="input">A migration input to use for initialization.</param>
        /// <param name="migrationId">The unique ID of the <see cref="IMigration"/> to include in the manifest.</param>
        /// <returns>The created <see cref="IMigrationManifestEditor"/> object.</returns>
        IMigrationManifestEditor Create(IMigrationInput input, Guid migrationId);

        /// <summary>
        /// Creates a new <see cref="IMigrationManifest"/> object.
        /// </summary>
        /// <param name="planId">The unique ID of the <see cref="IMigrationPlan"/> that the migration is running.</param>
        /// <param name="migrationId">The unique ID of the <see cref="IMigration"/> to include in the manifest.</param>
        /// <returns>The created <see cref="IMigrationManifestEditor"/> object.</returns>
        IMigrationManifestEditor Create(Guid planId, Guid migrationId);
    }
}
