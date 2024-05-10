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

using System;
using System.Collections;
using System.Collections.Generic;

namespace Tableau.Migration.Net.Rest.Filtering
{
    /// <summary>
    /// <para>
    /// Class representing a REST API filter
    /// </para>
    /// <para>
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_concepts_filtering_and_sorting.htm#filtering">Tableau API Reference</see> for more details.
    /// </para>
    /// </summary>
    public class Filter
    {
        /// <summary>
        /// Gets the filter's field.
        /// </summary>
        public string Field { get; }

        /// <summary>
        /// Gets the filter's operator.
        /// </summary>
        public string Operator { get; }

        /// <summary>
        /// Gets the filter's value.
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Gets the filter's expression for use in query strings;
        /// </summary>
        public string Expression { get; }

        /// <summary>
        /// Creates a new <see cref="Filter"/> instance.
        /// </summary>
        /// <param name="field">The filter's field.</param>
        /// <param name="operator">The filter's operator.</param>
        /// <param name="value">The filter's value.</param>
        public Filter(string field, string @operator, object value)
        {
            Field = Guard.AgainstNullEmptyOrWhiteSpace(field, nameof(field));
            Operator = Guard.AgainstNullEmptyOrWhiteSpace(@operator, nameof(@operator));
            Value = value;
            Expression = $"{Field}:{Operator}:{FormatValue(Value)}";
        }

        /// <summary>
        /// Creates a new <see cref="Filter"/> instance.
        /// </summary>
        /// <param name="field">The filter's field.</param>
        /// <param name="operator">The filter's operator.</param>
        /// <param name="values">The filter's values.</param>
        public Filter(string field, string @operator, IEnumerable<string> values)
            : this(field, @operator, (object)values)
        { }

        /// <summary>
        /// Creates a new <see cref="Filter"/> instance.
        /// </summary>
        /// <param name="field">The filter's field.</param>
        /// <param name="operator">The filter's operator.</param>
        /// <param name="value">The filter's value.</param>
        public Filter(string field, string @operator, string value)
            : this(field, @operator, (object)value)
        { }

        /// <summary>
        /// Creates a new <see cref="Filter"/> instance.
        /// </summary>
        /// <param name="field">The filter's field.</param>
        /// <param name="operator">The filter's operator.</param>
        /// <param name="values">The filter's values.</param>
        public Filter(string field, string @operator, IEnumerable<DateTime> values)
            : this(field, @operator, (object)values)
        { }

        /// <summary>
        /// Creates a new <see cref="Filter"/> instance.
        /// </summary>
        /// <param name="field">The filter's field.</param>
        /// <param name="operator">The filter's operator.</param>
        /// <param name="value">The filter's value.</param>
        public Filter(string field, string @operator, DateTime value)
            : this(field, @operator, (object)value)
        { }

        /// <summary>
        /// Creates a new <see cref="Filter"/> instance.
        /// </summary>
        /// <param name="field">The filter's field.</param>
        /// <param name="operator">The filter's operator.</param>
        /// <param name="values">The filter's values.</param>
        public Filter(string field, FilterOperator @operator, IEnumerable<string> values)
            : this(field, @operator.Value, (object)values)
        { }

        /// <summary>
        /// Creates a new <see cref="Filter"/> instance.
        /// </summary>
        /// <param name="field">The filter's field.</param>
        /// <param name="operator">The filter's operator.</param>
        /// <param name="value">The filter's value.</param>
        public Filter(string field, FilterOperator @operator, string value)
            : this(field, @operator.Value, value)
        { }

        /// <summary>
        /// Creates a new <see cref="Filter"/> instance.
        /// </summary>
        /// <param name="field">The filter's field.</param>
        /// <param name="operator">The filter's operator.</param>
        /// <param name="value">The filter's value.</param>
        public Filter(string field, FilterOperator @operator, DateTime value)
            : this(field, @operator.Value, value)
        { }

        /// <summary>
        /// Creates a new <see cref="Filter"/> instance.
        /// </summary>
        /// <param name="field">The filter's field.</param>
        /// <param name="operator">The filter's operator.</param>
        /// <param name="values">The filter's values.</param>
        public Filter(string field, FilterOperator @operator, IEnumerable<DateTime> values)
            : this(field, @operator.Value, values)
        { }

        private string FormatValue(object? value)
        {
            if (value is null)
                return string.Empty;

            if (value is string stringValue)
                return stringValue;

            if (value is DateTime dateTimeValue)
                return FormatValue(dateTimeValue.EnsureUtcKind().ToIso8601());

            if (value is IEnumerable enumerable)
            {
                var formatted = new List<string>();

                foreach (var item in enumerable)
                {
                    formatted.Add(FormatValue(item));
                }

                return $"[{string.Join(",", formatted)}]";
            }

            var toStringValue = value.ToString();

            if (toStringValue is null)
                return string.Empty;

            return FormatValue(toStringValue);
        }

        /// <summary>
        /// Indicates whether this value and a specified value are equal.
        /// </summary>
        /// <param name="other">The value to compare the current value.</param>
        /// <returns>true if <paramref name="other"/> and this value represents the same value; otherwise, false.</returns>
        public override bool Equals(object? other)
        {
            return other is Filter filter && Expression == filter.Expression;
        }

        /// <summary>
        /// Returns the hash code for this value.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode() => Expression.GetHashCode();
    }
}
