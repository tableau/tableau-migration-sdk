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

namespace Tableau.Migration
{
    /// <summary>
    /// Static class containing extension methods for <see cref="ISet{T}"/> objects.
    /// </summary>
    internal static class ISetExtensions
    {
        /// <summary>
        /// Adds the specified values to the specified <see cref="ISet{T}"/>.
        /// </summary>
        /// <typeparam name="T">The value type</typeparam>
        /// <param name="set">The <see cref="ISet{T}"/> to add items to.</param>
        /// <param name="values">The values to add.</param>
        /// <returns>The count of items added to the hash set.</returns>
        public static int AddRange<T>(this ISet<T> set, IEnumerable<T> values)
        {
            int addedCount = 0;

            foreach (var value in values)
            {
                if (set.Add(value))
                    addedCount++;
            }

            return addedCount;
        }
    }
}
