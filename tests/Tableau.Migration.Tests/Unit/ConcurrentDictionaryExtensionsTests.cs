using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Tableau.Migration.Tests.Unit
{
    public class ConcurrentDictionaryExtensionsTests
    {
        public abstract class ConcurrentDictionaryExtensionsTest : AutoFixtureTestBase
        {
            protected readonly ConcurrentDictionary<Guid, object> Dictionary = new();

            protected KeyValuePair<Guid, object> CreateItem()
                => new(Create<Guid>(), Create<object>());

            protected KeyValuePair<Guid, object> AddItem()
            {
                var kvp = CreateItem();

                Dictionary.TryAdd(kvp.Key, kvp.Value);

                return kvp;
            }
        }

        public class GetOrAddAsync : ConcurrentDictionaryExtensionsTest
        {
            [Fact]
            public async Task Gets()
            {
                var kvp = AddItem();

                var result = await Dictionary.GetOrAddAsync(kvp.Key, _ => throw new Exception());

                Assert.Same(kvp.Value, result);
            }

            [Fact]
            public async Task Adds()
            {
                var kvp = CreateItem();

                var result = await Dictionary.GetOrAddAsync(kvp.Key, _ => Task.FromResult(kvp.Value));

                Assert.Same(kvp.Value, result);
            }
        }
    }
}
