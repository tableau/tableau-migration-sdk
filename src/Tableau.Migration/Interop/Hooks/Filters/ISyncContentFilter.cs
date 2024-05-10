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
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Hooks.Filters;

namespace Tableau.Migration.Interop.Hooks.Filters
{
    /// <summary>
    /// Interface for an object that can synchronously filter content of a specific content type, to determine which content to migrate.
    /// </summary>
    /// <typeparam name="TContent"><inheritdoc/></typeparam>
    public interface ISyncContentFilter<TContent>
        : ISyncMigrationHook<IEnumerable<ContentMigrationItem<TContent>>>, IContentFilter<TContent>
        where TContent : IContentReference
    {
        /// <summary>
        /// Executes a filter callback.
        /// </summary>
        /// <param name="ctx">The input context from the migration engine or previous filter.</param>
        /// <returns>
        /// The context, 
        /// potentially modified to pass on to the next filter or migration engine, 
        /// or null to continue passing the same context as <paramref name="ctx"/>.
        /// </returns>
        new IEnumerable<ContentMigrationItem<TContent>>? Execute(IEnumerable<ContentMigrationItem<TContent>> ctx);
    }
}
