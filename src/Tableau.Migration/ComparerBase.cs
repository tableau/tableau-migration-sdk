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
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Tableau.Migration
{
    internal abstract class ComparerBase<T> :
        IComparer<T?>, IEqualityComparer<T?>,
        IComparer<IEnumerable<T>?>, IEqualityComparer<IEnumerable<T>?>
        where T : notnull
    {
        public int Compare(IEnumerable<T>? x, IEnumerable<T>? y)
        {
            return Compare(x, y,
                (x1, y1) =>
                {
                    using var xEnumerator = Sort(x1).GetEnumerator();
                    using var yEnumerator = Sort(y1).GetEnumerator();

                    var xExists = xEnumerator.MoveNext();
                    var yExists = yEnumerator.MoveNext();

                    while (xExists && yExists)
                    {
                        var result = Compare(xEnumerator.Current, yEnumerator.Current);

                        if (result != 0)
                            return result;

                        xExists = xEnumerator.MoveNext();
                        yExists = yEnumerator.MoveNext();
                    }

                    if (xExists && !yExists)
                        return 1;

                    if (!xExists && yExists)
                        return -1;

                    return 0;
                });
        }

        protected static int CompareValues<TValue>(T x, T y, Func<T, TValue> getValue, IComparer<TValue>? comparer = null)
        {
            comparer ??= Comparer<TValue>.Default;

            var xValue = getValue(x);
            var yValue = getValue(y);

            return comparer.Compare(xValue, yValue);
        }

        protected static int CompareValues(T x, T y, Func<T, string?> getValue, StringComparison comparison)
            => CompareValues(x, y, getValue, StringComparer.FromComparison(comparison));

        protected static int CompareValues(params Func<int>[] comparisons)
        {
            Guard.AgainstNullOrEmpty(comparisons, nameof(comparisons));

            var result = 0;

            foreach (var comparison in comparisons)
            {
                result = comparison();

                if (result != 0)
                    return result;
            }

            return result;
        }

        private static int Compare<TItem>(TItem? x, TItem? y, Func<TItem, TItem, int> compare)
        {
            if (x is null && y is null)
                return 0;

            if (ReferenceEquals(x, y))
                return 0;

            if (x is null)
                return -1;

            if (y is null)
                return 1;

            return compare(x, y);
        }

        private IEnumerable<T> Sort(IEnumerable<T> collection) => collection.OrderBy(c => c, this);

        public int Compare(T? x, T? y) => Compare(x, y, CompareItems);

        protected abstract int CompareItems(T x, T y);

        public bool Equals(T? x, T? y) => Compare(x, y) == 0;

        public virtual int GetHashCode([DisallowNull] T obj) => obj.GetHashCode();

        public bool Equals(IEnumerable<T>? x, IEnumerable<T>? y) => Compare(x, y) == 0;

        public int GetHashCode([DisallowNull] IEnumerable<T> obj)
        {
            var hashCodes = obj.Select(i => i is not null ? GetHashCode(i) : 0).OrderBy(h => h);

            var result = new HashCode();

            foreach (var hashCode in hashCodes)
                result.Add(hashCode);

            return result.ToHashCode();
        }
    }
}
