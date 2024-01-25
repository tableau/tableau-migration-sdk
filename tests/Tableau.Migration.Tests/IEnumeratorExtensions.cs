using System.Collections;
using System.Collections.Generic;

namespace Tableau.Migration.Tests
{
    public static class IEnumeratorExtensions
    {
        public static IEnumerable<T> ReadAll<T>(this IEnumerator enumerator)
        {
            while (enumerator.MoveNext())
            {
                yield return (T)enumerator.Current;
            }
        }
    }
}
