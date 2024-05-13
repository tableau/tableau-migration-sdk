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

namespace Tableau.Migration.Net.Rest.Filtering
{
    /// <summary>
    /// Class representing a filter operator for REST API requests.
    /// </summary>
    public readonly record struct FilterOperator
    {
        /// <summary>
        /// <para>
        /// Gets a <see cref="FilterOperator"/> instance representing the "eq" (equals) operator.
        /// </para>
        /// <para>
        /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_concepts_filtering_and_sorting.htm#filter-expression-notes">Tableau API Reference</see> for more details.
        /// </para>
        /// </summary>
        public static FilterOperator Equal { get; } = new("eq");

        /// <summary>
        /// <para>
        /// Gets a <see cref="FilterOperator"/> instance representing the "cieq" (case-insensitive equals) operator.
        /// </para>
        /// <para>
        /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_concepts_filtering_and_sorting.htm#filter-expression-notes">Tableau API Reference</see> for more details.
        /// </para>
        /// </summary>
        public static FilterOperator CaseInsensitiveEqual { get; } = new("cieq");

        /// <summary>
        /// <para>
        /// Gets a <see cref="FilterOperator"/> instance representing the "in" (any of [list]) operator.
        /// </para>
        /// <para>
        /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_concepts_filtering_and_sorting.htm#filter-expression-notes">Tableau API Reference</see> for more details.
        /// </para>
        /// </summary>
        public static FilterOperator In { get; } = new("in");

        /// <summary>
        /// <para>
        /// Gets a <see cref="FilterOperator"/> instance representing the "gt" (greater than) operator.
        /// </para>
        /// <para>
        /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_concepts_filtering_and_sorting.htm#filter-expression-notes">Tableau API Reference</see> for more details.
        /// </para>
        /// </summary>
        public static FilterOperator GreaterThan { get; } = new("gt");

        /// <summary>
        /// <para>
        /// Gets a <see cref="FilterOperator"/> instance representing the "gte" (greater than or equal) operator.
        /// </para>
        /// <para>
        /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_concepts_filtering_and_sorting.htm#filter-expression-notes">Tableau API Reference</see> for more details.
        /// </para>
        /// </summary>
        public static FilterOperator GreaterThanOrEqual { get; } = new("gte");

        /// <summary>
        /// <para>
        /// Gets a <see cref="FilterOperator"/> instance representing the "lt" (less than) operator.
        /// </para>
        /// <para>
        /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_concepts_filtering_and_sorting.htm#filter-expression-notes">Tableau API Reference</see> for more details.
        /// </para>
        /// </summary>
        public static FilterOperator LessThan { get; } = new("lt");

        /// <summary>
        /// <para>
        /// Gets a <see cref="FilterOperator"/> instance representing the "lte" (less than or equal) operator.
        /// </para>
        /// <para>
        /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_concepts_filtering_and_sorting.htm#filter-expression-notes">Tableau API Reference</see> for more details.
        /// </para>
        /// </summary>
        public static FilterOperator LessThanOrEqual { get; } = new("lte");

        /// <summary>
        /// <para>
        /// Gets a <see cref="FilterOperator"/> instance representing the "has" (includes) operator.
        /// </para>
        /// <para>
        /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_concepts_filtering_and_sorting.htm#filter-expression-notes">Tableau API Reference</see> for more details.
        /// </para>
        /// </summary>
        public static FilterOperator Has { get; } = new("has");

        /// <inheritdoc/>
        public string Value { get; }

        /// <summary>
        /// Creates a new <see cref="FilterOperator"/> instance.
        /// </summary>
        /// <param name="value">The operator's query string value.</param>
        public FilterOperator(string value)
        {
            Value = Guard.AgainstNullEmptyOrWhiteSpace(value, nameof(value));
        }

        /// <summary>
        /// Gets the <see cref="Value"/> of the current instance.
        /// </summary>
        /// <returns>The operator's value.</returns>
        public override string ToString() => Value;
    }
}
