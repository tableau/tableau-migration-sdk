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
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tableau.Migration.Engine.Options;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Options
{
    public class MigrationPlanOptionsBuilderTests
    {
        public class Roundtrip : AutoFixtureTestBase
        {
            private readonly MockServiceProvider _mockServices;
            private readonly MigrationPlanOptionsBuilder _builder;

            public Roundtrip()
            {
                _mockServices = new(AutoFixture);
                _builder = new();
            }

            [Fact]
            public void FromObject()
            {
                var opts = new TestPlanOptions { TestOption = 144 };

                var result = _builder.Configure(opts);

                Assert.Same(result, _builder);
                Assert.Same(opts, _builder.Build().Get<TestPlanOptions>(_mockServices.Object));
            }

            [Fact]
            public void FromDependencyInjection()
            {
                var opts = new TestPlanOptions { TestOption = 144 };
                _mockServices.Setup(x => x.GetService(typeof(TestPlanOptions))).Returns(opts);
                var optsFactory = (IServiceProvider s) => s.GetRequiredService<TestPlanOptions>();

                var result = _builder.Configure(optsFactory);

                Assert.Same(result, _builder);
                Assert.Same(opts, _builder.Build().Get<TestPlanOptions>(_mockServices.Object));
                _mockServices.Verify(x => x.GetService(typeof(TestPlanOptions)), Times.Once);
            }
        }
    }
}
