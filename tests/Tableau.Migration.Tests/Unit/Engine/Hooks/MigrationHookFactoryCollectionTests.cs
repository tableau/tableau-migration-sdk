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
using Microsoft.Extensions.DependencyInjection;
using Tableau.Migration.Engine.Hooks;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks
{
    public class MigrationHookFactoryCollectionTests
    {
        #region - Test Types -

        private interface ITestHook : IMigrationHook<int> { }

        private interface IDifferentHook : IMigrationHook<Guid> { }

        #endregion

        #region - Empty -

        public class Empty
        {
            [Fact]
            public void IsEmpty()
            {
                var c = MigrationHookFactoryCollection.Empty;

                Assert.Empty(c.GetHooks<ITestHook>());
                Assert.Empty(c.GetHooks<IDifferentHook>());
            }
        }

        #endregion

        #region - GetHooks -

        public class GetHooks
        {
            [Fact]
            public void GetsAllHooksForType()
            {
                var fact1 = new MigrationHookFactory(s => s.GetRequiredService<ITestHook>());
                var fact2 = new MigrationHookFactory(s => s.GetRequiredService<ITestHook>());

                var dict = new Dictionary<Type, ImmutableArray<IMigrationHookFactory>>()
                {
                    { typeof(ITestHook), new IMigrationHookFactory[] { fact1, fact2 }.ToImmutableArray() }
                }.ToImmutableDictionary();

                var c = new MigrationHookFactoryCollection(dict);

                var results = c.GetHooks(typeof(ITestHook));

                Assert.Equal(dict[typeof(ITestHook)], results);

                var genericResults = c.GetHooks<ITestHook>();
                Assert.Equal(results, genericResults);

                var missingResults = c.GetHooks<IDifferentHook>();
                Assert.Empty(missingResults);
            }
        }

        #endregion
    }
}
