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
using System.Linq;

namespace Tableau.Migration.Engine.Hooks.Filters.Default
{
    /// <summary>
    /// Options for <see cref="GroupAllUsersFilter"/>.
    /// </summary>
    public class GroupAllUsersFilterOptions
    {
        /// <summary>
        /// Gets or sets the translated names of the "All Users" group.
        /// </summary>
        public List<string> AllUsersGroupNames { get; init; } = new List<string>();

        /// <summary>
        /// Creates a new <see cref="GroupAllUsersFilterOptions"/> instance.
        /// </summary>
        public GroupAllUsersFilterOptions()
        { }

        /// <summary>
        /// Creates a new <see cref="GroupAllUsersFilterOptions"/> instance.
        /// </summary>
        /// <param name="allUsersGroupNames">The "All Users" group name translations.</param>
        public GroupAllUsersFilterOptions(IEnumerable<string> allUsersGroupNames)
        {
            AllUsersGroupNames.AddRange(allUsersGroupNames.Distinct());
        }
    }
}
