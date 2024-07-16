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
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Tableau.Migration
{
    internal static class IEnumerableExtensions
    {
        #region - IsNullOrEmpty -

        /// <summary>
        /// Determines if the specified <see cref="IEnumerable"/> is null or contains no items
        /// </summary>
        /// <param name="collection">the collection to evaluate</param>
        /// <returns>true if the collection is null or empty, false otherwise</returns>
        public static bool IsNullOrEmpty([NotNullWhen(false)] this IEnumerable? collection)
        {
            if (collection is null)
                return true;

            if (collection is ICollection c)
                return c.Count == 0;

            var enumerator = collection.GetEnumerator();

            try
            {
                return !enumerator.MoveNext();
            }
            finally
            {
                if (enumerator is IDisposable d)
                {
                    d.Dispose();
                }
            }
        }

        #endregion

        #region - ExceptNulls -

        /// <summary>
        /// Gets all <paramref name="items"/> except nulls.
        /// </summary>
        public static IEnumerable<T> ExceptNulls<T>(this IEnumerable<T?> items)
            where T : class
        {
            foreach (var i in items)
            {
                if (i is null)
                    continue;

                yield return i;
            }
        }

        /// <summary>
        /// Gets all <paramref name="items"/> except nulls.
        /// </summary>
        public static IEnumerable<T> ExceptNulls<T>(this IEnumerable<T?> items)
            where T : struct
        {
            foreach (var i in items)
            {
                if (i is null)
                    continue;

                yield return i.Value;
            }
        }

        /// <summary>
        /// Gets the selected <paramref name="items"/> except nulls.
        /// </summary>
        public static IEnumerable<TResult> ExceptNulls<TSource, TResult>(this IEnumerable<TSource?> items, Func<TSource, TResult?> selector)
            where TSource : class
            where TResult : class
            => items.ExceptNulls().Select(selector).ExceptNulls();

        
        #endregion
    }
}
