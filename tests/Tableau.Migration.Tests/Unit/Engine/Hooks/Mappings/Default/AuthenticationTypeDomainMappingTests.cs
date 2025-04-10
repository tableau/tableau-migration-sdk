﻿//
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

using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Hooks.Mappings;
using Tableau.Migration.Engine.Hooks.Mappings.Default;
using Tableau.Migration.Engine.Options;
using Tableau.Migration.Resources;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Mappings.Default
{
    public class AuthenticationTypeDomainMappingTests
    {
        public class ExecuteAsync : AutoFixtureTestBase
        {
            private readonly Mock<ISharedResourcesLocalizer> _mockLocalizer = new();
            private readonly Mock<ILogger<AuthenticationTypeDomainMapping>> _mockLogger = new();

            [Fact]
            public async Task MapsUserDomainAsync()
            {
                var mockOptions = Create<Mock<IMigrationPlanOptionsProvider<AuthenticationTypeDomainMappingOptions>>>();
                mockOptions.Setup(x => x.Get()).Returns(new AuthenticationTypeDomainMappingOptions
                {
                    UserDomain = "userDomain"
                });

                var mapping = new AuthenticationTypeDomainMapping(mockOptions.Object, _mockLocalizer.Object, _mockLogger.Object);

                var ctx = Create<ContentMappingContext<IUser>>();
                var result = await mapping.ExecuteAsync(ctx, new());

                var expectedLoc = ContentLocation.ForUsername("userDomain", ctx.MappedLocation.Name);

                Assert.NotNull(result);
                Assert.NotSame(ctx, result);
                Assert.Same(ctx.ContentItem, result.ContentItem);
                Assert.Equal(expectedLoc, result.MappedLocation);
            }

            [Fact]
            public async Task MapsGroupDomainAsync()
            {
                var mockOptions = Create<Mock<IMigrationPlanOptionsProvider<AuthenticationTypeDomainMappingOptions>>>();
                mockOptions.Setup(x => x.Get()).Returns(new AuthenticationTypeDomainMappingOptions
                {
                    GroupDomain = "groupDomain"
                });

                var mapping = new AuthenticationTypeDomainMapping(mockOptions.Object, _mockLocalizer.Object, _mockLogger.Object);

                var ctx = Create<ContentMappingContext<IGroup>>();
                var result = await mapping.ExecuteAsync(ctx, new());

                var expectedLoc = ContentLocation.ForUsername("groupDomain", ctx.MappedLocation.Name);

                Assert.NotNull(result);
                Assert.NotSame(ctx, result);
                Assert.Same(ctx.ContentItem, result.ContentItem);
                Assert.Equal(expectedLoc, result.MappedLocation);
            }
        }
    }
}
