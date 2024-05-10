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

using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Hooks.Filters;
using Tableau.Migration.Engine.Hooks.Filters.Default;
using Tableau.Migration.Engine.Manifest;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Filters.Default
{
    public class ProjectSystemOwnershipFilterTests
    {
        public class ShouldMigrate : AutoFixtureTestBase
        {
            private readonly MockSharedResourcesLocalizer _mockLocalizer = new();
            private readonly Mock<ILogger<IContentFilter<IProject>>> _mockLogger = new();
            private readonly SystemOwnershipFilter<IProject> _filter;

            public ShouldMigrate()
            {
                _filter = new(_mockLocalizer.Object, _mockLogger.Object);
            }

            [Fact]
            public void False_when_External_Assets_Default_Project_translation()
            {
                var mockProject = Create<Mock<IProject>>();
                mockProject.SetupGet(g => g.Owner.Location).Returns(Constants.SystemUserLocation);

                var item = new ContentMigrationItem<IProject>(mockProject.Object, new Mock<IMigrationManifestEntryEditor>().Object);

                Assert.False(_filter.ShouldMigrate(item));
            }

            [Fact]
            public void True_when_not_External_Assets_Default_Project_translation()
            {
                var proj = Create<IProject>();

                var item = new ContentMigrationItem<IProject>(proj, new Mock<IMigrationManifestEntryEditor>().Object);

                Assert.True(_filter.ShouldMigrate(item));
            }
        }
    }
}
