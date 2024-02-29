﻿// Copyright (c) 2023, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the ""License"") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an ""AS IS"" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using System;
using System.Collections.Generic;
using System.Linq;

namespace Tableau.Migration.Tests
{
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Determine if two <see cref="IEnumerable{T}"/> sequences are equal ignoring ordering.
        /// </summary>
        /// <typeparam name="T">The collection item type.</typeparam>
        /// <param name="first">The first collection.</param>
        /// <param name="second">The second collection.</param>
        /// <param name="sort">The sorting to use to normalize ordering between the two collections.</param>
        /// <returns>True if the sequences are equal, false otherwise.</returns>
        public static bool SequenceEqual<T>(this IEnumerable<T>? first, IEnumerable<T>? second, Func<T, object?> sort)
        {
            if (first is null || second is null)
                return ReferenceEquals(first, second);

            var firstSorted = first.OrderBy(sort);
            var secondSorted = second.OrderBy(sort);

            return firstSorted.SequenceEqual(secondSorted);
        }
    }
}
