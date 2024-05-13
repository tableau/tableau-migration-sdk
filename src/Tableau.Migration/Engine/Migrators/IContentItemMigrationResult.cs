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

using Tableau.Migration.Engine.Manifest;

namespace Tableau.Migration.Engine.Migrators
{
    /// <summary>
    /// <see cref="IResult"/> object for a content item migration action.
    /// </summary>
    public interface IContentItemMigrationResult<TContent> : IResult
    {
        /// <summary>
        /// Gets whether or not the current migration batch should continue.
        /// </summary>
        bool ContinueBatch { get; }

        /// <summary>
        /// Gets whether the item migration was canceled.
        /// </summary>
        bool IsCanceled { get; }

        /// <summary>
        /// Gets the manifest entry for the content item.
        /// </summary>
        IMigrationManifestEntry ManifestEntry { get; }

        /// <summary>
        /// Creates a new <see cref="IContentItemMigrationResult{TContent}"/> object while modifying the <see cref="ContinueBatch"/> value.
        /// </summary>
        /// <param name="continueBatch">Whether or not the current migration batch should continue.</param>
        /// <returns>The new <see cref="IContentItemMigrationResult{TContent}"/> object.</returns>
        IContentItemMigrationResult<TContent> ForContinueBatch(bool continueBatch);
    }
}
