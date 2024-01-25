using System;
using System.Collections.Generic;
using System.Linq;

namespace Tableau.Migration.Content
{
    internal static class ITagExtensions
    {
        public static IList<ITag> ToTagList<T>(this IEnumerable<T>? e, Func<T, ITag> factory)
        {
            if(e.IsNullOrEmpty())
            {
                return new List<ITag>();
            }

            return e.Select(factory).ToList();
        }
    }
}
