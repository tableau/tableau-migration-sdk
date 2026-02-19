//
//  Copyright (c) 2026, Salesforce, Inc.
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

using Moq;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Services;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Endpoints
{
    public sealed class MigrationPlanEndpointBuilderTests
    {
        public sealed class Ctor : AutoFixtureTestBase
        {
            [Fact]
            public void Initializes()
            {
                var mockMigrationServiceBuilderFactory = Freeze<Mock<IMigrationServiceBuilderFactory>>();

                var b = new MigrationPlanEndpointBuilder(mockMigrationServiceBuilderFactory.Object);

                mockMigrationServiceBuilderFactory.Verify(x => x.Create(MigrationServices.SupportedEndpointServices), Times.Once);
                Assert.Same(b.Services, b.Configuration.Services);
            }
        }
    }
}
