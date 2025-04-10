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
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.Engine.Manifest.Logging;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Manifest
{
    public sealed class LoggingMigrationManifestTests
    {
        #region - Test Classes -

        public class LoggingMigrationManifestTest : AutoFixtureTestBase
        {
            protected readonly MockSharedResourcesLocalizer MockLocalizer;
            protected readonly Mock<ILoggerFactory> MockLoggerFactory;
            protected readonly Mock<ILogger<MigrationManifest>> MockLogger;

            protected readonly LoggingMigrationManifest Manifest;

            public LoggingMigrationManifestTest()
            {
                MockLocalizer = new();

                MockLogger = Create<Mock<ILogger<MigrationManifest>>>();
                MockLoggerFactory = Freeze<Mock<ILoggerFactory>>();
                MockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>()))
                    .Returns(MockLogger.Object);

                Manifest = new(MockLocalizer.Object, MockLoggerFactory.Object, Guid.NewGuid(), Guid.NewGuid(), PipelineProfile.ServerToCloud);
            }
        }

        #endregion

        #region - AddErrors -

        public class AddErrors : LoggingMigrationManifestTest
        {
            [Fact]
            public void AddSingleError()
            {
                var exception = new Exception();

                var result = Manifest.AddErrors(exception);

                Assert.Same(result, Manifest);
                var resultItem = Assert.Single(Manifest.Errors);
                Assert.Same(exception, resultItem);

                MockLogger.VerifyErrors(Times.Once);
            }

            [Fact]
            public void AddMultipleErrors()
            {
                var exceptions = new[] { new Exception(), new Exception() };

                var result = Manifest.AddErrors(exceptions);

                Assert.Same(result, Manifest);
                Assert.Equal(2, Manifest.Errors.Count);
                Assert.Contains(exceptions[0], Manifest.Errors);
                Assert.Contains(exceptions[1], Manifest.Errors);

                MockLogger.VerifyErrors(Times.Exactly(2));
            }
        }

        #endregion
    }
}
