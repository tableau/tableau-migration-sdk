//
//  Copyright (c) 2026, Salesforce, Inc.
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

using Tableau.Migration.Paging;

namespace Tableau.Migration.Engine.Endpoints
{
    /// <summary>
    /// Interface for an object that can retrieve a pager to load content items to consider for migration.
    /// These items are then mapped, filtered, and migrated.
    /// </summary>
    /// <typeparam name="TContent">The content type to get the pager for.</typeparam>
    public interface IMigrationContentLoader<TContent>
    {
        /// <summary>
        /// Gets a pager for content to consider for migration.
        /// </summary>
        /// <param name="pageSize">The page size to configure the pager for.</param>
        /// <returns>The pager.</returns>
        IPager<TContent> GetMigrationContentPager(int pageSize);
    }
}
