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
using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.Engine.Hooks.Filters;
using Tableau.Migration.Engine.Manifest;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Filters
{
    public sealed class FilterStatusExtensionsTests
    {
        public sealed class ApplyAfterHook : AutoFixtureTestBase
        {
            private const string TEST_HOOK_NAME = "testHookName";

            private readonly Mock<IMigrationManifestEntryEditor> _mockManifestEntry;
            private readonly Mock<ILogger> _mockLogger;
            private readonly MockSharedResourcesLocalizer _mockLocalizer;

            public ApplyAfterHook()
            {
                _mockManifestEntry = Freeze<Mock<IMigrationManifestEntryEditor>>();
                _mockLogger = Freeze<Mock<ILogger>>();
                _mockLocalizer = Freeze<MockSharedResourcesLocalizer>();
            }

            private void TestApplyAfterHook(FilterStatus status)
            {
                status.ApplyAfterHook(_mockManifestEntry.Object, TEST_HOOK_NAME, _mockLogger.Object, _mockLocalizer.Object);
            }

            [Fact]
            public void ResetsNoFilter()
            {
                TestApplyAfterHook(FilterStatus.Migrate);

                _mockManifestEntry.Verify(x => x.ResetStatus(), Times.Once);
                _mockManifestEntry.VerifyNoOtherCalls();
            }

            [Fact]
            public void SkipNoCascade()
            {
                TestApplyAfterHook(FilterStatus.Skip);

                _mockManifestEntry.Verify(x => x.SetSkipped(false, TEST_HOOK_NAME), Times.Once);
                _mockManifestEntry.Verify(x => x.ResetStatus(), Times.Never);

                _mockLogger.VerifyLogging(LogLevel.Debug, Times.Once);
            }

            [Fact]
            public void SkipCascade()
            {
                TestApplyAfterHook(FilterStatus.CascadeSkip);

                _mockManifestEntry.Verify(x => x.SetSkipped(true, TEST_HOOK_NAME), Times.Once);
                _mockManifestEntry.Verify(x => x.ResetStatus(), Times.Never);

                _mockLogger.VerifyLogging(LogLevel.Debug, Times.Once);
            }

            [Fact]
            public void Unsupported()
            {
                Assert.Throws<NotSupportedException>(() => TestApplyAfterHook((FilterStatus)Int32.MaxValue));
            }
        }
    }
}
