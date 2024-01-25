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
        public static bool SequenceEqual<T>(this IEnumerable<T> first, IEnumerable<T> second, Func<T, object> sort)
        {
            var firstSorted = first.OrderBy(sort);
            var secondSorted = second.OrderBy(sort);

            return firstSorted.SequenceEqual(secondSorted);
        }
    }
}
