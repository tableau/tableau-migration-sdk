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

namespace Tableau.Migration.Engine.Migrators.Batch
{
    /// <summary>
    /// Default <see cref="IContentBatchMigrationResult{TContent}"/> implementation.
    /// </summary>
    internal record ContentBatchMigrationResult<TContent> : Result, IContentBatchMigrationResult<TContent>
        where TContent : IContentReference
    {
        /// <inheritdoc />
        public bool PerformNextBatch { get; }

        /// <inheritdoc />
        public IImmutableList<IContentItemMigrationResult<TContent>> ItemResults { get; }

        protected ContentBatchMigrationResult(bool success, bool performNextBatch, IImmutableList<IContentItemMigrationResult<TContent>> itemResults, IEnumerable<Exception> errors)
            : base(success, errors)
        {
            PerformNextBatch = performNextBatch;
            ItemResults = itemResults;
        }

        protected ContentBatchMigrationResult(IResult baseResult, bool performNextBatch, IImmutableList<IContentItemMigrationResult<TContent>> itemResults)
            : this(baseResult.Success, performNextBatch, itemResults, baseResult.Errors)
        { }

        protected ContentBatchMigrationResult(bool success, bool performNextBatch, IImmutableList<IContentItemMigrationResult<TContent>> itemResults, params Exception[] errors)
            : this(success, performNextBatch, itemResults, (IEnumerable<Exception>)errors)
        { }

        /// <summary>
        /// Creates a new <see cref="ContentBatchMigrationResult{TContent}"/> instance for successful operations.
        /// </summary>
        /// <param name="itemResults">The migration result of each item in the batch, in the order they finished.</param>
        /// <param name="performNextBatch">Whether or not to migrate the next batch, if any.</param>
        /// <returns>A new <see cref="ContentBatchMigrationResult{TContent}"/> instance.</returns>
        public static ContentBatchMigrationResult<TContent> Succeeded(ImmutableArray<IContentItemMigrationResult<TContent>> itemResults, bool performNextBatch = true)
            => new(true, performNextBatch, itemResults);

        /// <summary>
        /// Creates a new <see cref="ContentBatchMigrationResult{TContent}"/> instance for successful operations.
        /// </summary>
        /// <param name="itemResults">The migration result of each item in the batch, in the order they finished.</param>
        /// <param name="errors">The errors encountered during the operation.</param>
        /// <param name="performNextBatch">Whether or not to migrate the next batch, if any.</param>
        /// <returns>A new <see cref="ContentBatchMigrationResult{TContent}"/> instance.</returns>
        public static ContentBatchMigrationResult<TContent> Failed(ImmutableArray<IContentItemMigrationResult<TContent>> itemResults, IEnumerable<Exception> errors, bool performNextBatch = true)
            => new(false, performNextBatch, itemResults, errors);

        /// <inheritdoc />
        public IContentBatchMigrationResult<TContent> ForNextBatch(bool performNextBatch)
            => new ContentBatchMigrationResult<TContent>(Success, performNextBatch, ItemResults, Errors);
    }
}
