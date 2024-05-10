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

namespace Tableau.Migration.Net
{
    /// <summary>
    /// Class that can build URL query strings.
    /// </summary>
    public interface IQueryStringBuilder
    {
        /// <summary>
        /// Gets whether any filters have been added to this instance.
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Adds the items from the specified <see cref="IDictionary{String, String}"/> whose keys have not been previously added.
        /// </summary>
        /// <param name="items">The items to add to the query string.</param>
        void AddOrUpdate(IDictionary<string, string> items);

        /// <summary>
        /// Adds the specified key and value if the key has not been previously added.
        /// </summary>
        /// <param name="key">The key to add to the query string.</param>
        /// <param name="value">The value to add to the query string.</param>
        void AddOrUpdate(string key, string value);

        /// <summary>
        /// Builds the resulting query string for the key/value pairs that have been added.
        /// </summary>
        string Build();
    }
}