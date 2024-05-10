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
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Hooks.Mappings;
using Microsoft.Extensions.Logging;
using Xunit;
using Moq;
using Tableau.Migration.Resources;


namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Mappings
{
    public class ContentMappingBaseTests : AutoFixtureTestBase
    {
        internal class StubUserMapping : ContentMappingBase<IUser>
        {
            private readonly ContentMappingContext<IUser> _searchLocation;
            private readonly ContentMappingContext<IUser> _replaceLocation;

            public StubUserMapping(
                ContentMappingContext<IUser> searchLocation, 
                ContentMappingContext<IUser> replaceLocation,
                ISharedResourcesLocalizer localizer,
                ILogger<StubUserMapping> logger) 
                    : base(localizer, logger)
            {
                _searchLocation = searchLocation;
                _replaceLocation = replaceLocation;
            }

            public override Task<ContentMappingContext<IUser>?> MapAsync(ContentMappingContext<IUser> ctx, CancellationToken cancel)
            {
                if (ctx == _searchLocation)
                {
                    return _replaceLocation.ToTask();
                }

                return ctx.ToTask();
            }
        }

        [Fact]
        public async Task Map()
        {
            // Arrange
            // Create mock data
            var mappedLoc = Create<ContentMappingContext<IUser>>();
            var unmappedLoc = Create<ContentMappingContext<IUser>>();
            var replaceLoc = Create<ContentMappingContext<IUser>>();
            var mockLogger = new Mock<ILogger<StubUserMapping>>();
            var mockLocalizer = new MockSharedResourcesLocalizer();

            mockLogger.Setup(x => x.IsEnabled(LogLevel.Debug)).Returns(true);

            var mapping = new StubUserMapping(mappedLoc, replaceLoc, mockLocalizer.Object, mockLogger.Object);

            // Act
            var mappedResult = await mapping.ExecuteAsync(mappedLoc, default);
            var unmappedResult = await mapping.ExecuteAsync(unmappedLoc, default);

            // Asserts
            Assert.Equal(replaceLoc, mappedResult);
            Assert.Equal(unmappedLoc, unmappedResult);

            // Verify we got at least 1 debug log message
            mockLogger.VerifyLogging(LogLevel.Debug, Times.AtLeastOnce());
        }
    }
}
