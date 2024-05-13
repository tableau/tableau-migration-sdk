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

namespace Tableau.Migration.Engine.Manifest
{
    /// <summary>
    /// <see cref="IMigrationManifest"/> that can be edited.
    /// Used while a migration is in progress.
    /// </summary>
    public interface IMigrationManifestEditor : IMigrationManifest
    {
        /// <summary>
        /// Gets an object to edit manifest entries with.
        /// </summary>
        new IMigrationManifestEntryCollectionEditor Entries { get; }

        /// <summary>
        /// Adds top-level errors that are not related to any Tableau content item from multiple <see cref="IResult"/> objects.
        /// </summary>
        /// <param name="results">The results to add errors from.</param>
        /// <returns>This manifest editor, for fluent API usage.</returns>
        public IMigrationManifestEditor AddErrors(IEnumerable<IResult> results)
        {
            foreach (var result in results)
            {
                AddErrors(result);
            }

            return this;
        }

        /// <summary>
        /// Adds top-level errors that are not related to any Tableau content item from a <see cref="IResult"/> object.
        /// </summary>
        /// <param name="result">The result to add errors from.</param>
        /// <returns>This manifest editor, for fluent API usage.</returns>
        public IMigrationManifestEditor AddErrors(IResult result) => AddErrors(result.Errors);

        /// <summary>
        /// Adds top-level errors that are not related to any Tableau content item.
        /// </summary>
        /// <param name="errors">The errors to add.</param>
        /// <returns>This manifest editor, for fluent API usage.</returns>
        IMigrationManifestEditor AddErrors(IEnumerable<Exception> errors);

        /// <summary>
        /// Adds top-level errors that are not related to any Tableau content item.
        /// </summary>
        /// <param name="errors">The errors to add.</param>
        /// <returns>This manifest editor, for fluent API usage.</returns>
        IMigrationManifestEditor AddErrors(params Exception[] errors);
    }
}
