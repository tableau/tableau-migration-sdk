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
using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Content.Files;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Resources;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Endpoints
{
    public class MigrationEndpointFactoryTests
    {
        public class MigrationEndpointFactoryTest : AutoFixtureTestBase, IDisposable
        {
            protected readonly ServiceProvider MigrationServices;

            protected readonly MigrationEndpointFactory _factory;

            public MigrationEndpointFactoryTest()
            {
                var migrationServiceCollection = new ServiceCollection()
                    .AddLocalization()
                    .AddSharedResourcesLocalization()
                    .AddMigrationApiClient();

                MigrationServices = migrationServiceCollection.BuildServiceProvider();
                var serviceScopeFactory = MigrationServices.GetRequiredService<IServiceScopeFactory>();

                _factory = new(serviceScopeFactory,
                    Create<ManifestSourceContentReferenceFinderFactory>(),
                    Create<ManifestDestinationContentReferenceFinderFactory>(),
                    Create<IContentFileStore>(),
                    Create<ISharedResourcesLocalizer>()
                );
            }

            public void Dispose()
            {
                MigrationServices.Dispose();
                GC.SuppressFinalize(this);
            }
        }

        public class CreateDestination : MigrationEndpointFactoryTest
        {
            [Fact]
            public void CreatesTableauApiDestinationEndpoint()
            {
                var mockPlan = new Mock<IMigrationPlan>();
                mockPlan.SetupGet(x => x.Destination).Returns(TableauApiEndpointConfiguration.Empty);

                var destination = _factory.CreateDestination(mockPlan.Object);

                var apiDestination = Assert.IsType<TableauApiDestinationEndpoint>(destination);
            }

            [Fact]
            public void ThrowsOnUnsupportedEndpointType()
            {
                var plan = AutoFixture.Create<IMigrationPlan>();

                Assert.Throws<ArgumentException>(() => _factory.CreateDestination(plan));
            }
        }

        public class CreateSource : MigrationEndpointFactoryTest
        {
            [Fact]
            public void CreatesTableauApiSourceEndpoint()
            {
                var mockPlan = new Mock<IMigrationPlan>();
                mockPlan.SetupGet(x => x.Source).Returns(TableauApiEndpointConfiguration.Empty);

                var source = _factory.CreateSource(mockPlan.Object);

                var apiSource = Assert.IsType<TableauApiSourceEndpoint>(source);
            }

            [Fact]
            public void ThrowsOnUnsupportedEndpointType()
            {
                var plan = AutoFixture.Create<IMigrationPlan>();

                Assert.Throws<ArgumentException>(() => _factory.CreateSource(plan));
            }
        }
    }
}
