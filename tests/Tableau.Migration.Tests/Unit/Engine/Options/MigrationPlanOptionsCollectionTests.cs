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
using Moq;
using Tableau.Migration.Engine.Options;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Options
{
    public class MigrationPlanOptionsCollectionTests
    {
        public class Get : AutoFixtureTestBase
        {
            private readonly MockServiceProvider _mockServices;

            public Get()
            {
                _mockServices = new(AutoFixture);
            }

            [Fact]
            public void RunsFactory()
            {
                var opts = new TestPlanOptions { TestOption = 3 };
                _mockServices.Setup(x => x.GetService(typeof(TestPlanOptions))).Returns(opts);

                var c = new MigrationPlanOptionsCollection(new Dictionary<Type, Func<IServiceProvider, object?>>
                {
                    { typeof(TestPlanOptions), (services) => services.GetRequiredService<TestPlanOptions>() }
                }.ToImmutableDictionary());

                var result = c.Get<TestPlanOptions>(_mockServices.Object);

                Assert.Same(opts, result);
                _mockServices.Verify(x => x.GetService(typeof(TestPlanOptions)), Times.Once);
            }

            [Fact]
            public void FactoryReturnsNull()
            {
                var c = new MigrationPlanOptionsCollection(new Dictionary<Type, Func<IServiceProvider, object?>>
                {
                    { typeof(TestPlanOptions), (services) => null }
                }.ToImmutableDictionary());

                var result = c.Get<TestPlanOptions>(_mockServices.Object);

                Assert.Null(result);
            }

            [Fact]
            public void NoFactory()
            {
                var c = MigrationPlanOptionsCollection.Empty;

                var result = c.Get<TestPlanOptions>(_mockServices.Object);

                Assert.Null(result);
            }
        }
    }
}
