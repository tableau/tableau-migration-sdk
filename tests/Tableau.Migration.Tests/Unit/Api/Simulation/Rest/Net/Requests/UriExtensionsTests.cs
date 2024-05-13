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
using System.Linq;
using Tableau.Migration.Api.Simulation.Rest.Net.Requests;
using Tableau.Migration.Net.Rest;
using Tableau.Migration.Net.Rest.Filtering;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Simulation.Rest.Net.Requests
{
    public class UriExtensionsTests
    {
        public abstract class UriExtensionsTest : AutoFixtureTestBase
        {
            protected Uri CreateUri(
                string? relativeUri = null,
                string? query = null,
                bool ensureAbsoluteUri = true)
            {
                relativeUri ??= Create<string>();

                if (!String.IsNullOrWhiteSpace(query))
                    relativeUri = $"{relativeUri}?{query.TrimStart('?')}";

                var uri = new Uri(relativeUri, UriKind.RelativeOrAbsolute);

                if (!uri.IsAbsoluteUri && ensureAbsoluteUri)
                    return uri.EnsureAbsoluteUri();

                return uri;
            }
        }

        public class ParseFilters : UriExtensionsTest
        {
            protected static string CreateQuery(IEnumerable<Filter> filters) => $"filter={String.Join(",", filters.Select(f => f.Expression))}";

            protected static string CreateQuery(Filter filter) => CreateQuery(new[] { filter });

            [Theory]
            [NullEmptyWhiteSpaceData]
            public void Returns_empty_collection_when_query_empty(string? query)
            {
                var uri = CreateUri($"{Create<string>()}?{query}");

                Assert.Empty(uri.ParseFilters());
            }

            [Fact]
            public void Parses_filter()
            {
                var filter = Create<Filter>();

                var uri = CreateUri(query: CreateQuery(filter));

                var results = uri.ParseFilters();

                var result = Assert.Single(results);

                Assert.Equal(filter, result);
            }

            [Fact]
            public void Parses_multiple_filters()
            {
                var filters = CreateMany<Filter>(3).ToList();

                var uri = CreateUri(query: CreateQuery(filters));

                var results = uri.ParseFilters();

                Assert.True(filters.SequenceEqual(results));
            }

            [Fact]
            public void Uses_last_field_filter()
            {
                var filter1 = Create<Filter>();

                var filter2 = new Filter(filter1.Field, Create<string>(), Create<string>());

                var filters = new List<Filter> { filter1, filter2 };

                var uri = CreateUri(query: CreateQuery(filters));

                var results = uri.ParseFilters();

                var result = Assert.Single(results);

                Assert.Equal(filter2, result);
            }
        }
    }
}
