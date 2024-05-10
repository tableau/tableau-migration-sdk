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

namespace Tableau.Migration.Net.Rest.Sorting
{
    /// <summary>
    /// <para>
    /// Interface for a class that can build REST API sort query strings.
    /// </para>
    /// <para>
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_concepts_filtering_and_sorting.htm#sorting">Tableau API Reference</see> for more details.
    /// </para>
    /// </summary>
    public interface ISortBuilder
    {
        /// <summary>
        /// Gets whether the builder contains any sorts.
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Adds a sort to the builder.
        /// </summary>
        /// <param name="sort">The sort to add.</param>
        /// <returns>The current <see cref="ISortBuilder"/> instance.</returns>
        ISortBuilder AddSort(Sort sort);

        /// <summary>
        /// Adds sorts to the builder.
        /// </summary>
        /// <param name="sorts">The sorts to add.</param>
        /// <returns>The current <see cref="ISortBuilder"/> instance.</returns>
        ISortBuilder AddSorts(params Sort[] sorts);

        /// <summary>
        /// Builds the string value for the sorts for use in query strings.
        /// </summary>
        /// <returns>The formatted string representation of the sorts.</returns>
        string Build();

        /// <summary>
        /// Appends the added sorts to the specified <see cref="IQueryStringBuilder"/> instance.
        /// </summary>
        /// <param name="builder">The <see cref="IQueryStringBuilder"/> to append to.</param>
        void AppendQueryString(IQueryStringBuilder builder);
    }
}