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
using Tableau.Migration.Engine.Hooks;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks
{
    public class MigrationHookRunnerTests
    {
        #region - Test Types -

        private class TestContext { }

        private interface ITestHook : IMigrationHook<TestContext> { }

        private class TestHook : ITestHook
        {
            private readonly List<TestContext> _contexts;
            private readonly TestContext? _result;

            public TestHook(List<TestContext> contexts, TestContext? result)
            {
                _contexts = contexts;
                _result = result;
            }

            public Task<TestContext?> ExecuteAsync(TestContext ctx, CancellationToken cancel)
            {
                _contexts.Add(ctx);
                return Task.FromResult(_result);
            }
        }

        #endregion

        #region - ExecuteAsync -

        public class ExecuteAsync : AutoFixtureTestBase
        {
            private readonly List<TestContext> _hookExecutionContexts;
            private readonly List<IMigrationHookFactory> _hookFactories;

            private readonly IMigrationPlan _plan;

            private readonly MigrationHookRunner _runner;

            public ExecuteAsync()
            {
                _hookExecutionContexts = new();
                _hookFactories = new();

                var mockHooks = AutoFixture.Create<Mock<IMigrationHookFactoryCollection>>();
                mockHooks.Setup(x => x.GetHooks<ITestHook>()).Returns(() => _hookFactories.ToImmutableArray());

                var mockPlan = AutoFixture.Create<Mock<IMigrationPlan>>();
                mockPlan.SetupGet(x => x.Hooks).Returns(mockHooks.Object);
                _plan = mockPlan.Object;

                _runner = new(_plan, new Mock<IServiceProvider>().Object);
            }

            private void AddHookWithResult(TestContext? result)
            {
                var hook = new TestHook(_hookExecutionContexts, result);
                _hookFactories.Add(new MigrationHookFactory(s => hook));
            }

            [Fact]
            public async Task AllowsOrderedContextOverwriteAsync()
            {
                var ctx1 = new TestContext();
                var ctx2 = new TestContext();
                var ctx3 = new TestContext();

                AddHookWithResult(ctx1);
                AddHookWithResult(ctx2);
                AddHookWithResult(ctx3);

                var input = new TestContext();

                var result = await _runner.ExecuteAsync<ITestHook, TestContext>(input, default);

                Assert.Same(ctx3, result);
                Assert.Equal(new[] { input, ctx1, ctx2 }, _hookExecutionContexts);
            }

            [Fact]
            public async Task NullResultReturnsInputAsync()
            {
                AddHookWithResult(null);
                AddHookWithResult(null);
                AddHookWithResult(null);

                var input = new TestContext();

                var result = await _runner.ExecuteAsync<ITestHook, TestContext>(input, default);

                Assert.Same(input, result);
                Assert.Equal(new[] { input, input, input }, _hookExecutionContexts);
            }
        }

        #endregion
    }
}
