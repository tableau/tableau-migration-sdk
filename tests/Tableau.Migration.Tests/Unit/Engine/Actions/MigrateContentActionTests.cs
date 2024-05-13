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
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Engine.Actions;
using Tableau.Migration.Engine.Migrators;
using Tableau.Migration.Engine.Pipelines;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Actions
{
    public class MigrateContentActionTests
    {
        #region - Test Classes -

        public class MigrateContentActionTest : AutoFixtureTestBase
        {
            protected readonly Mock<IMigrationPipeline> MockPipeline;
            protected readonly Mock<IContentMigrator<TestContentType>> MockContentMigrator;

            protected readonly MigrateContentAction<TestContentType> Action;

            public MigrateContentActionTest()
            {
                MockContentMigrator = Create<Mock<IContentMigrator<TestContentType>>>();

                MockPipeline = Create<Mock<IMigrationPipeline>>();
                MockPipeline.Setup(x => x.GetMigrator<TestContentType>())
                    .Returns(MockContentMigrator.Object);

                Action = new(MockPipeline.Object);
            }
        }

        #endregion

        #region - Ctor -

        public class Ctor : MigrateContentActionTest
        {
            [Fact]
            public void GetMigratorByContentType()
            {
                MockPipeline.Verify(x => x.GetMigrator<TestContentType>(), Times.Once());
            }
        }

        #endregion

        #region - ExecuteAsync -

        public class ExecuteAsync : MigrateContentActionTest
        {
            [Fact]
            public async Task MigratorSucceedsAsync()
            {
                MockContentMigrator.Setup(x => x.MigrateAsync(Cancel)).ReturnsAsync(Result.Succeeded());

                var result = await Action.ExecuteAsync(Cancel);

                result.AssertSuccess();
            }

            [Fact]
            public async Task MigratorFailsAsync()
            {
                var failure = Result.Failed(new[] { new Exception(), new Exception() });
                MockContentMigrator.Setup(x => x.MigrateAsync(Cancel)).ReturnsAsync(failure);

                var result = await Action.ExecuteAsync(Cancel);

                result.AssertFailure();
                Assert.Equal(failure.Errors, result.Errors);
            }
        }

        #endregion
    }
}
