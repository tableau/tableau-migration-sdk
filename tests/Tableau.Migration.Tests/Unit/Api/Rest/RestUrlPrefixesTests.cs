using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Rest;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest
{
    public class RestUrlPrefixesTests
    {
        public abstract class RestUrlPrefixesTest : AutoFixtureTestBase
        {
            protected static readonly IImmutableDictionary<Type, string> PrefixDictionary =
                typeof(RestUrlPrefixes).GetFieldValue<IImmutableDictionary<Type, string>>("_urlPrefixesByType")!;

            protected static readonly IImmutableList<Type> ConcreteContentApiClientTypes =
                typeof(IContentApiClient).Assembly.GetTypes()
                    .Where(t =>
                        t.IsAssignableTo(typeof(IContentApiClient)) &&
                        !t.IsInterface &&
                        !t.IsAbstract)
                    .ToImmutableList();
        }

        public class Initialization : RestUrlPrefixesTest
        {
            [Fact]
            public void Covers_all_client_types()
            {
                foreach (var type in ConcreteContentApiClientTypes)
                {
                    Assert.Contains(type, PrefixDictionary);
                }
            }

            [Fact]
            public void No_duplicate_prefixes()
            {
                var unique = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                foreach (var item in PrefixDictionary.Values)
                    Assert.True(unique.Add(item));
            }
        }
    }
}
