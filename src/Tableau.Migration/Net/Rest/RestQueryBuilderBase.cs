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

namespace Tableau.Migration.Net.Rest
{
    /// <summary>
    /// Base class for REST query string builders.
    /// </summary>
    /// <typeparam name="TItem">The type of item to format in the query string.</typeparam>
    internal abstract class RestQueryBuilderBase<TItem>
    {
        protected readonly HashSet<TItem> _items = new();

        /// <summary>
        /// Gets whether the builder contains any items.
        /// </summary>
        public bool IsEmpty => _items.Count == 0;

        /// <summary>
        /// Gets the query string key/value pair for the added items.
        /// </summary>
        /// <returns>A key/value pair for the added items.</returns>
        protected abstract IDictionary<string, string>? BuildQueryString();

        /// <summary>
        /// Builds the string value for the items for use in query strings.
        /// </summary>
        /// <returns>The formatted string representation of the items.</returns>
        public string Build()
        {
            var query = BuildQueryString();

            if (query is null)
                return string.Empty;

            return string.Join("&", query.Select(q => string.Join("=", q.Key, q.Value)));
        }

        /// <summary>
        /// Appends the added items to the specified <see cref="IQueryStringBuilder"/> instance.
        /// </summary>
        /// <param name="builder">The <see cref="IQueryStringBuilder"/> to append to.</param>
        public void AppendQueryString(IQueryStringBuilder builder)
        {
            var query = BuildQueryString();

            if (query is null)
                return;

            foreach (var kvp in query)
                builder.AddOrUpdate(kvp.Key, kvp.Value);
        }
    }
}
