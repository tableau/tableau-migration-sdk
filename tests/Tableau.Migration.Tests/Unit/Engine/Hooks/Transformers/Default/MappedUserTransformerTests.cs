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

using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Engine.Hooks.Transformers.Default;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Transformers.Default
{
    public class MappedUserTransformerTests
    {
        public abstract class MappedUserTransformerTest : AutoFixtureTestBase
        {
            protected readonly Mock<IDestinationContentReferenceFinderFactory> MockDestinationFinderFactory = new();
            protected readonly Mock<ILogger<MappedUserTransformer>> MockLogger = new();
            protected readonly MockSharedResourcesLocalizer MockSharedResourcesLocalizer = new();
            protected readonly Mock<IDestinationContentReferenceFinder<IUser>> MockUserContentFinder = new();

            protected readonly MappedUserTransformer Transformer;

            public MappedUserTransformerTest()
            {
                MockDestinationFinderFactory.Setup(p => p.ForDestinationContentType<IUser>()).Returns(MockUserContentFinder.Object);

                Transformer = new(MockDestinationFinderFactory.Object, MockLogger.Object, MockSharedResourcesLocalizer.Object);
            }
        }

        public class ExecuteAsync : MappedUserTransformerTest
        {
            [Fact]
            public async Task Returns_null_when_user_not_found()
            {
                var result = await Transformer.ExecuteAsync(Create<IContentReference>(), Cancel);

                Assert.Null(result);
            }

            [Fact]
            public async Task Returns_destination_user_when_found()
            {
                var sourceUser = Create<IContentReference>();
                var destinationUser = Create<IContentReference>();

                MockUserContentFinder.Setup(f => f.FindBySourceLocationAsync(sourceUser.Location, Cancel)).ReturnsAsync(destinationUser);

                var result = await Transformer.ExecuteAsync(sourceUser, Cancel);

                Assert.Same(destinationUser, result);
            }
        }
    }
}
