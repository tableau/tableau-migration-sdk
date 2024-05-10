//
//  Copyright (c) 2024, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the "License") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

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
