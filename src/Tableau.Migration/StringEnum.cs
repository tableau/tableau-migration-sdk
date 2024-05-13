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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace Tableau.Migration
{
    /// <summary>
    /// Class representing enum-like string values.
    /// </summary>
    /// <typeparam name="T">The type that contains the values.</typeparam>
    public abstract class StringEnum<T>
    {
        private static readonly Lazy<IImmutableList<string>> _all;

        static StringEnum()
        {
            _all = new Lazy<IImmutableList<string>>(() =>
            {
                var values = typeof(T)
                    .GetFields(BindingFlags.Static | BindingFlags.Public)
                    .Where(f => f.FieldType == typeof(string))
                    .OrderBy(f => f.MetadataToken)
                    .Select(f => f.GetValue(null)?.ToString())
                    .ExceptNulls();

                if (!values.Any())
                    throw new ArgumentException($"Type {typeof(T).Name} does not have any public static string fields.", nameof(T));

                return values.ToImmutableArray();
            });
        }

        /// <summary>
        /// Gets a collection of all values.
        /// </summary>
        /// <param name="exclude">The values to exclude.</param>
        public static IImmutableList<string> GetAll(params string[] exclude)
            => GetAll((IEnumerable<string>)exclude);

        /// <summary>
        /// Gets a collection of all values.
        /// </summary>
        /// <param name="exclude">The values to exclude.</param>
        public static IImmutableList<string> GetAll(IEnumerable<string> exclude)
            => exclude.IsNullOrEmpty()
                ? _all.Value
                : _all.Value.SkipWhile(v => exclude.Any(e => IsAMatch(v, e))).ToImmutableArray();

        /// <summary>
        /// Finds whether or not two values match, case insensitively.
        /// </summary>
        /// <param name="first">The first value to test.</param>
        /// <param name="second">The second value to test.</param>
        /// <returns>True if matched; otherwise false.</returns>
        public static bool IsAMatch(string? first, string? second)
            => String.Equals(first, second, StringComparison.OrdinalIgnoreCase);
    }
}
