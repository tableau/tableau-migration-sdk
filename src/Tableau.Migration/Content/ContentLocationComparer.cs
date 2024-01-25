using System.Collections.Generic;

namespace Tableau.Migration.Content
{
    internal sealed class ContentLocationComparer<TContent> : IComparer<TContent>
        where TContent : IContentReference
    {
        /// <summary>
        /// Gets a static singleton instance of the comparer.
        /// </summary>
        public static readonly ContentLocationComparer<TContent> Instance = new();

        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than,
        /// equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// A signed integer that indicates the relative values of x and y, as shown in the
        /// following table.
        /// Value – Meaning
        /// Less than zero –x is less than y.
        /// Zero –x equals y.
        /// Greater than zero –x is greater than y.
        /// </returns>
        public int Compare(TContent? x, TContent? y)
        {
            if (x is null && y is null)
            {
                return 0;
            }
            else if (x is null)
            {
                return -1;
            }
            else if (y is null)
            {
                return 1;
            }

            return x.Location.CompareTo(y.Location);
        }
    }
}
