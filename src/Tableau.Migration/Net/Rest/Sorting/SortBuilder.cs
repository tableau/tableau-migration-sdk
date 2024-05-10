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

namespace Tableau.Migration.Net.Rest.Sorting
{
    /// <summary>
    /// <para>
    /// Class that can build REST API sort query strings.
    /// </para>
    /// <para>
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_concepts_filtering_and_sorting.htm#sorting">Tableau API Reference</see> for more details.
    /// </para>
    /// </summary>
    internal sealed class SortBuilder : RestQueryBuilderBase<Sort>, ISortBuilder
    {
        /// <inheritdoc/>
        public ISortBuilder AddSort(Sort sort)
        {
            _items.Add(sort);
            return this;
        }

        /// <inheritdoc/>
        public ISortBuilder AddSorts(params Sort[] sorts)
        {
            Guard.AgainstNull(sorts, nameof(sorts));

            foreach (var sort in sorts)
                AddSort(sort);

            return this;
        }

        /// <inheritdoc/>
        protected override IDictionary<string, string>? BuildQueryString()
        {
            if (IsEmpty)
                return null;

            return new Dictionary<string, string>
            {
                ["sort"] = string.Join(",", _items.Select(s => s.Expression))
            };
        }
    }
}
