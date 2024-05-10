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
using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.Engine;
using Tableau.Migration.Resources;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine
{
    public class MigrationInputTests
    {
        public class MigrationInputTest : AutoFixtureTestBase
        {
            protected readonly Mock<ILogger<MigrationInput>> MockLog;

            protected MigrationInput Input;

            public MigrationInputTest()
            {
                MockLog = Create<Mock<ILogger<MigrationInput>>>();

                Input = new(MockLog.Object, Create<ISharedResourcesLocalizer>());
            }
        }

        public class Ctor : MigrationInputTest
        {
            [Fact]
            public void GeneratesMigrationId()
            {
                Assert.NotEqual(Guid.Empty, Input.MigrationId);
            }
        }

        public class Plan : MigrationInputTest
        {
            [Fact]
            public void ExceptionBeforeInitialized()
            {
                Assert.Throws<InvalidOperationException>(() => Input.Plan);
            }
        }

        public class Initialize : MigrationInputTest
        {
            [Fact]
            public void InitializesWithExpectedPreviousManifest()
            {
                var plan = Create<IMigrationPlan>();

                var mockPreviousManifest = Create<Mock<IMigrationManifest>>();
                mockPreviousManifest.SetupGet(x => x.PlanId).Returns(plan.PlanId);

                Input.Initialize(plan, mockPreviousManifest.Object);

                Assert.Same(plan, Input.Plan);
                Assert.Same(mockPreviousManifest.Object, Input.PreviousManifest);

                MockLog.VerifyWarnings(Times.Never);
            }

            [Fact]
            public void WarnsWithMismatchPreviousManifest()
            {
                var plan = Create<IMigrationPlan>();

                var mockPreviousManifest = Create<Mock<IMigrationManifest>>();
                mockPreviousManifest.SetupGet(x => x.PlanId).Returns(Guid.NewGuid());

                Input.Initialize(plan, mockPreviousManifest.Object);

                Assert.Same(plan, Input.Plan);
                Assert.Same(mockPreviousManifest.Object, Input.PreviousManifest);

                MockLog.VerifyWarnings(Times.Once);
            }

            [Fact]
            public void InitializesWithoutPreviousManifest()
            {
                var plan = Create<IMigrationPlan>();

                Input.Initialize(plan, null);

                Assert.Same(plan, Input.Plan);
                Assert.Null(Input.PreviousManifest);

                MockLog.VerifyWarnings(Times.Never);
            }
        }
    }
}
