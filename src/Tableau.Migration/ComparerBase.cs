﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Tableau.Migration
{
    internal abstract class ComparerBase<T> :
        IComparer<T>, IEqualityComparer<T>,
        IComparer<IEnumerable<T>>, IEqualityComparer<IEnumerable<T>>
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

        public abstract int CompareItems(T x, T y);

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
