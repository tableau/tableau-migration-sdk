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
