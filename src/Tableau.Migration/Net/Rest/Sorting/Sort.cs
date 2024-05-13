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

using System.ComponentModel;

namespace Tableau.Migration.Net.Rest.Sorting
{
    /// <summary>
    /// <para>
    /// Class representing a REST API sort
    /// </para>
    /// <para>
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_concepts_filtering_and_sorting.htm#sorting">Tableau API Reference</see> for more details.
    /// </para>
    /// </summary>
    public class Sort
    {
        /// <summary>
        /// Gets the field to sort on.
        /// </summary>
        public string Field { get; }

        /// <summary>
        /// Gets the direction to sort.
        /// </summary>
        public ListSortDirection Direction { get; }

        /// <summary>
        /// Gets the sort's expression for use in query strings.
        /// </summary>
        public string Expression { get; }

        /// <summary>
        /// Creates a new <see cref="Sort"/> instance.
        /// </summary>
        /// <param name="field">The field to sort on.</param>
        /// <param name="direction">The direction to sort.</param>
        public Sort(string field, ListSortDirection direction)
        {
            Field = Guard.AgainstNullEmptyOrWhiteSpace(field, nameof(field));
            Direction = direction;
            Expression = $"{Field}:{Direction.GetQueryStringValue()}";
        }

        /// <summary>
        /// Creates a new <see cref="Sort"/> instance.
        /// </summary>
        /// <param name="field">The field to sort on.</param>
        /// <param name="ascending">Try to sort ascending, false to sort descending.</param>
        public Sort(string field, bool ascending)
            : this(field, ascending ? ListSortDirection.Ascending : ListSortDirection.Descending)
        { }
    }
}
