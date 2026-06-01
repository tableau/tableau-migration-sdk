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

namespace Tableau.Migration.Engine.Hooks.Filters
{
    /// <summary>
    /// Enumeration of the various filter states for a content item.
    /// </summary>
    public enum FilterStatus
    {
        /// <summary>
        /// The content item will attempt to migrate.
        /// </summary>
        Migrate = 0,

        /// <summary>
        /// The content item will not migrate, but items that reference this one will still migrate.
        /// </summary>
        Skip,

        /// <summary>
        /// The content item will not migrate, and items that reference this one will also not migrate.
        /// </summary>
        CascadeSkip
    }
}
