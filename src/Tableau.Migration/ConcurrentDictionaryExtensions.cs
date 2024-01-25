using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Tableau.Migration
{
    internal static class ConcurrentDictionaryExtensions
    {
        public static async Task<TValue> GetOrAddAsync<TKey, TValue>(
            this ConcurrentDictionary<TKey, TValue> dictionary,
            TKey key,
            Func<TKey, Task<TValue>> valueFactory)
            where TKey : notnull
        {
            if (dictionary.TryGetValue(key, out TValue? value))
                return value;

            return dictionary.GetOrAdd(key, await valueFactory(key).ConfigureAwait(false));
        }
    }
}
