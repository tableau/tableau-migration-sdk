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
using System.Collections.Immutable;
using System.Linq;
using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.Resources;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Manifest
{
    public class MigrationManifestFactoryTests
    {
        private interface ITestContent : IContentReference
        { }

        public class Create : AutoFixtureTestBase
        {
            private readonly Mock<ISharedResourcesLocalizer> _mockLocalizer;
            private readonly Mock<ILoggerFactory> _mockLoggerFactory;
            private readonly MigrationManifestFactory _factory;

            private readonly Mock<IMigrationInput> _mockInput;
            private readonly Guid _planId;

            private readonly Guid _migrationId;

            public Create()
            {
                _mockLocalizer = Freeze<Mock<ISharedResourcesLocalizer>>();
                _mockLoggerFactory = Freeze<Mock<ILoggerFactory>>();
                _factory = Create<MigrationManifestFactory>();

                _mockInput = Create<Mock<IMigrationInput>>();

                _migrationId = Guid.NewGuid();

                _planId = Guid.NewGuid();
                _mockInput.SetupGet(x => x.Plan.PlanId).Returns(_planId);
                _mockInput.SetupGet(x => x.MigrationId).Returns(_migrationId);
            }

            [Fact]
            public void InitializesEmptyManifestWithInput()
            {
                // Customize the AutoFixture instance to remove the customization for IMigrationManifest that was created from AutoFixtureTestBase/FixtureFactory
                AutoFixture.Customize<IMigrationManifest>(c => c.FromFactory(() =>
                    new MigrationManifest(AutoFixture.Create<ISharedResourcesLocalizer>(), AutoFixture.Create<ILoggerFactory>(), Guid.NewGuid(), Guid.NewGuid())));

                var manifest = _factory.Create(_mockInput.Object, _migrationId);

                Assert.Equal(_mockInput.Object.Plan.PlanId, manifest.PlanId);
                Assert.Equal(_migrationId, manifest.MigrationId);

                Assert.Empty(manifest.Entries);
            }

            [Fact]
            public void InitializesEmptyManifestWithoutInput()
            {
                var manifest = _factory.Create(_planId, _migrationId);

                Assert.Equal(_planId, manifest.PlanId);
                Assert.Equal(_migrationId, manifest.MigrationId);

                Assert.Empty(manifest.Entries);
            }

            [Fact]
            public void CreatesInstancesWithInput()
            {
                var manifest1 = _factory.Create(_mockInput.Object, _migrationId);
                var manifest2 = _factory.Create(_mockInput.Object, _migrationId);

                Assert.NotSame(manifest1, manifest2);
            }

            [Fact]
            public void CreatesInstancesWithoutInput()
            {
                var manifest1 = _factory.Create(_planId, _migrationId);
                var manifest2 = _factory.Create(_planId, _migrationId);

                Assert.NotSame(manifest1, manifest2);
            }

            [Fact]
            public void CopiesFromPreviousManifest()
            {
                var previousEntries = CreateMany<IMigrationManifestEntry>().ToImmutableArray();

                var previousManifest = Create<MigrationManifest>();
                previousManifest.Entries
                    .GetOrCreatePartition<ITestContent>()
                    .CreateEntries(previousEntries);

                _mockInput.SetupGet(x => x.PreviousManifest).Returns(previousManifest);

                var manifest = _factory.Create(_mockInput.Object, _migrationId);

                Assert.Equal(previousManifest.Entries.Count(), manifest.Entries.Count());
            }
        }
    }
}
