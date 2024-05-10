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

using System.Collections.Immutable;

namespace Tableau.Migration.Engine.Migrators.Batch
{
    /// <summary>
    /// <see cref="IResult"/> object for a migration action.
    /// </summary>
    public interface IContentBatchMigrationResult<TContent> : IResult
        where TContent : IContentReference
    {
        /// <summary>
        /// Gets whether or not to migrate the next batch, if any.
        /// </summary>
        bool PerformNextBatch { get; }

        /// <summary>
        /// Gets the migration result of each item in the batch, in the order they finished.
        /// </summary>
        IImmutableList<IContentItemMigrationResult<TContent>> ItemResults { get; }

        /// <summary>
        /// Creates a new <see cref="IContentBatchMigrationResult{TContent}"/> object while modifying the <see cref="PerformNextBatch"/> value.
        /// </summary>
        /// <param name="performNextBatch">Whether or not to migrate the next batch.</param>
        /// <returns>The new <see cref="IContentBatchMigrationResult{TContent}"/> object.</returns>
        IContentBatchMigrationResult<TContent> ForNextBatch(bool performNextBatch);
    }
}
