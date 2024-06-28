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

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Engine.Hooks.Filters
{
    /// <summary>
    /// Interface for an object that can run filters.
    /// </summary>
    public interface IContentFilterRunner
    {
        /// <summary>
        /// Executes all filters for the content type in order.
        /// </summary>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <param name="itemsToFilter">The items to filter.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>A lazy-evaluated collection of filterered items.</returns>
        Task<IEnumerable<ContentMigrationItem<TContent>>> ExecuteAsync<TContent>(IEnumerable<ContentMigrationItem<TContent>> itemsToFilter, CancellationToken cancel)
            where TContent : IContentReference;
    }
}
