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

using Moq;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Endpoints;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine
{
    public class IMigrationExtensionsTests
    {
        public abstract class IMigrationExtensionsTest : AutoFixtureTestBase
        {
            protected readonly Mock<IMigration> MockMigration = new();
        }

        public class TryGetSourceApiEndpoint : IMigrationExtensionsTest
        {
            [Fact]
            public void Returns_false_when_not_Api_endpoint()
            {
                MockMigration.SetupGet(m => m.Source).Returns(new Mock<ISourceEndpoint>().Object);

                Assert.False(MockMigration.Object.TryGetSourceApiEndpoint(out var endpoint));
                Assert.Null(endpoint);
            }

            [Fact]
            public void Returns_true_when_Api_endpoint()
            {
                var sourceEndpoint = new Mock<ISourceApiEndpoint>().Object;

                MockMigration.SetupGet(m => m.Source).Returns(sourceEndpoint);

                Assert.True(MockMigration.Object.TryGetSourceApiEndpoint(out var endpoint));
                Assert.Same(sourceEndpoint, endpoint);
            }
        }

        public class TryGetDestinationApiEndpoint : IMigrationExtensionsTest
        {
            [Fact]
            public void Returns_false_when_not_Api_endpoint()
            {
                MockMigration.SetupGet(m => m.Destination).Returns(new Mock<IDestinationEndpoint>().Object);

                Assert.False(MockMigration.Object.TryGetDestinationApiEndpoint(out var endpoint));
                Assert.Null(endpoint);
            }

            [Fact]
            public void Returns_true_when_Api_endpoint()
            {
                var destinationEndpoint = new Mock<IDestinationApiEndpoint>().Object;

                MockMigration.SetupGet(m => m.Destination).Returns(destinationEndpoint);

                Assert.True(MockMigration.Object.TryGetDestinationApiEndpoint(out var endpoint));
                Assert.Same(destinationEndpoint, endpoint);
            }
        }
    }
}
