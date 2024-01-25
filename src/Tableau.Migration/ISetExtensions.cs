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
