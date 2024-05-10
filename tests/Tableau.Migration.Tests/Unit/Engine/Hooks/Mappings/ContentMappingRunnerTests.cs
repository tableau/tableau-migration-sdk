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
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Engine.Hooks.Mappings;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Mappings
{
    public class ContentMappingRunnerTests
    {
        #region - Test Types -

        private class TestMapping : IContentMapping<IUser>
        {
            private readonly List<ContentMappingContext<IUser>> _contexts;
            private readonly ContentMappingContext<IUser> _result;

            public TestMapping(List<ContentMappingContext<IUser>> contexts, ContentMappingContext<IUser> result)
            {
                _contexts = contexts;
                _result = result;
            }

            public Task<ContentMappingContext<IUser>?> ExecuteAsync(ContentMappingContext<IUser> ctx, CancellationToken cancel)
            {
                _contexts.Add(ctx);

                return Task.FromResult<ContentMappingContext<IUser>?>(_result);
            }
        }

        #endregion

        #region - ExecuteAsync -

        public class ExecuteAsync : AutoFixtureTestBase
        {
            private readonly List<ContentMappingContext<IUser>> _mappingExecutionContexts;
            private readonly List<IMigrationHookFactory> _mappingFactories;

            private readonly IMigrationPlan _plan;

            private readonly ContentMappingRunner _runner;

            public ExecuteAsync()
            {
                _mappingExecutionContexts = new();
                _mappingFactories = new();

                var mockMappings = AutoFixture.Create<Mock<IMigrationHookFactoryCollection>>();
                mockMappings
                    .Setup(x => x.GetHooks<IContentMapping<IUser>>())
                    .Returns(() => _mappingFactories.ToImmutableArray());

                var mockPlan = AutoFixture.Create<Mock<IMigrationPlan>>();
                mockPlan
                    .SetupGet(x => x.Mappings)
                    .Returns(mockMappings.Object);

                _plan = mockPlan.Object;

                Assert.NotNull(_plan.Mappings);

                _runner = new(
                    _plan,
                    new Mock<IServiceProvider>().Object);
            }

            private void AddMappingWithResult(ContentMappingContext<IUser> result)
            {
                var mapping = new TestMapping(_mappingExecutionContexts, result);

                _mappingFactories.Add(new MigrationHookFactory(s => mapping));
            }

            [Fact]
            public async Task AllowsOrderedContextOverwriteAsync()
            {
                // Arrange
                var ctx1 = Create<ContentMappingContext<IUser>>();
                var ctx2 = Create<ContentMappingContext<IUser>>();
                var ctx3 = Create<ContentMappingContext<IUser>>();

                AddMappingWithResult(ctx1);
                AddMappingWithResult(ctx2);
                AddMappingWithResult(ctx3);

                var input = Create<ContentMappingContext<IUser>>();

                // Act
                var result = await _runner.ExecuteAsync<IUser>(input, default);

                // Asserts
                Assert.Equal(ctx3, result);
                Assert.Equal(new[] { input, ctx1, ctx2 }, _mappingExecutionContexts);
            }
        }

        #endregion
    }
}
