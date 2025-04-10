//
//  Copyright (c) 2025, Salesforce, Inc.
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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Actions;
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Engine.Hooks.ActionCompleted;
using Tableau.Migration.Engine.Pipelines;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.ActionCompleted
{
    public class SubscriptionsEnabledActionCompletedHookTests : AutoFixtureTestBase
    {
        protected readonly Mock<ISubscriptionsCapabilityManager> MockSubCapManager;
        protected readonly Mock<IMigrationPipelineRunner> MockPipelineRunner;
        protected readonly SubscriptionsEnabledActionCompletedHook Hook;

        public SubscriptionsEnabledActionCompletedHookTests()
        {
            MockPipelineRunner = new Mock<IMigrationPipelineRunner>();
            MockSubCapManager = new Mock<ISubscriptionsCapabilityManager>();
            MockSubCapManager.Setup(scm => scm.SetMigrationCapabilityAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IResult>(Result.Succeeded()));
            MockSubCapManager.Setup(scm => scm.IsMigrationCapabilityDisabled())
                .Returns(false);

            Hook = new SubscriptionsEnabledActionCompletedHook(MockPipelineRunner.Object, MockSubCapManager.Object);
        }

        public class ExecuteAsync : SubscriptionsEnabledActionCompletedHookTests
        {
            [Fact]
            public async Task Skips_for_non_workbook()
            {
                var migrateContentAction = Create<MigrateContentAction<IUser>>();
                MockPipelineRunner.Setup(pr => pr.CurrentAction).Returns(migrateContentAction);

                var context = Create<MigrationActionResult>();

                var result = await Hook.ExecuteAsync(context, new CancellationToken());

                Assert.NotNull(result);
                Assert.True(result.Success);
                Assert.Same(context, result);
                MockSubCapManager.Verify(x => x.IsMigrationCapabilityDisabled(), Times.Never);
                MockSubCapManager.Verify(
                    x => x.SetMigrationCapabilityAsync(It.IsAny<CancellationToken>()),
                    Times.Never);
            }

            [Fact]
            public async Task Runs_for_workbook()
            {
                var migrateContentAction = Create<MigrateContentAction<IWorkbook>>();
                MockPipelineRunner.Setup(pr => pr.CurrentAction).Returns(migrateContentAction);

                var context = Create<MigrationActionResult>();

                var result = await Hook.ExecuteAsync(context, new CancellationToken());

                Assert.NotNull(result);
                Assert.True(result.Success);
                Assert.Same(context, result);
                MockSubCapManager.Verify(x => x.IsMigrationCapabilityDisabled(), Times.Once);
                MockSubCapManager.Verify(
                    x => x.SetMigrationCapabilityAsync(It.IsAny<CancellationToken>()),
                    Times.Once);
            }

            [Fact]
            public async Task Handles_failure()
            {
                var migrateContentAction = Create<MigrateContentAction<IWorkbook>>();
                MockPipelineRunner.Setup(pr => pr.CurrentAction).Returns(migrateContentAction);

                var errors = CreateMany<Exception>();
                MockSubCapManager.Setup(scm => scm.SetMigrationCapabilityAsync(It.IsAny<CancellationToken>()))
                    .Returns(Task.FromResult<IResult>(Result.Failed(errors)));

                var context = Create<MigrationActionResult>();

                var result = await Hook.ExecuteAsync(context, new CancellationToken());

                Assert.NotNull(result);
                Assert.False(result.Success);
                Assert.NotSame(context, result);
                Assert.Equal(errors.Count(), result.Errors.Count);
                MockSubCapManager.Verify(
                    x => x.IsMigrationCapabilityDisabled(),
                    Times.Once);

                MockSubCapManager.Verify(
                    x => x.SetMigrationCapabilityAsync(It.IsAny<CancellationToken>()),
                    Times.Once);
            }
        }

    }
}
