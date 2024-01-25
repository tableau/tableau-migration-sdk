using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Tableau.Migration.Content
{
    /// <summary>
    /// <see cref="IEqualityComparer{ITag}"/> implementation that compares tags 
    /// by their string value with case sensitivity.
    /// </summary>
    public class TagLabelComparer : IEqualityComparer<ITag>
    {
        /// <summary>
        /// A singleton instance of the comparer.
        /// </summary>
        public static readonly TagLabelComparer Instance = new();

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>True if the specified objects are equal; otherwise, false.</returns>
        public bool Equals(ITag? x, ITag? y)
            => string.Equals(x?.Label, y?.Label, StringComparison.Ordinal);

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <param name="obj">The object for which a hash code is to be returned.</param>
        /// <returns>A hash code for the specified object.</returns>
        public int GetHashCode([DisallowNull] ITag obj)
            => obj.Label.GetHashCode(StringComparison.Ordinal);
    }
}
