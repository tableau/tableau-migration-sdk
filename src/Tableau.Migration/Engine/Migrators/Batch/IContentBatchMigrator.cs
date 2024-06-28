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
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Engine.Migrators.Batch
{
    /// <summary>
    /// Interface for an object that migrates a batch of content items.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    public interface IContentBatchMigrator<TContent>
        where TContent : class, IContentReference
    {
        /// <summary>
        /// Migrates a batch of content items.
        /// </summary>
        /// <param name="itemBatch">The batch of content items to migrate.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The results of the batch migration.</returns>
        Task<IContentBatchMigrationResult<TContent>> MigrateAsync(ImmutableArray<ContentMigrationItem<TContent>> itemBatch, CancellationToken cancel);
    }
}
