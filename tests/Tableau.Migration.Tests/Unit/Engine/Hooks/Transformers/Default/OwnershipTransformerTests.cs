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

using System.Threading;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Engine.Hooks.Transformers.Default;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Transformers.Default
{
    public class OwnershipTransformerTests
    {
        public class TestOwnershipType : TestContentType, IWithOwner
        {
            public IContentReference Owner { get; set; } = null!;
        }

        public class ExecuteAsync : AutoFixtureTestBase
        {
            [Fact]
            public async Task SetsMappedOwnerAsync()
            {
                var cancel = new CancellationToken();
                var ctx = Create<TestOwnershipType>();

                var mockUserFinder = Freeze<Mock<IDestinationContentReferenceFinder<IUser>>>();

                var mockFinderFactory = Freeze<Mock<IDestinationContentReferenceFinderFactory>>();
                mockFinderFactory.Setup(x => x.ForDestinationContentType<IUser>()).Returns(mockUserFinder.Object);

                var mappedRef = Create<IContentReference>();
                mockUserFinder.Setup(x => x.FindBySourceLocationAsync(ctx.Owner.Location, cancel))
                    .ReturnsAsync(mappedRef);

                var transformer = Create<OwnershipTransformer<TestOwnershipType>>();

                var result = await transformer.ExecuteAsync(ctx, cancel);

                Assert.NotNull(result);
                Assert.Same(ctx, result);

                Assert.Same(mappedRef, result.Owner);
            }
        }
    }
}
