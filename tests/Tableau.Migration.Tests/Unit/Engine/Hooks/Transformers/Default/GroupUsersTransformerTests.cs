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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Engine.Hooks.Transformers.Default;
using Tableau.Migration.Resources;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Transformers.Default
{
    public class GroupUsersTransformerTests
    {
        public abstract class GroupUsersTransformerTest : AutoFixtureTestBase
        {
            protected readonly Mock<IDestinationContentReferenceFinderFactory> MockDestinationContentReferenceFinderFactory = new();
            protected readonly Mock<IDestinationContentReferenceFinder<IUser>> MockUserContentFinder = new();

            protected readonly GroupUsersTransformer Transformer;

            public GroupUsersTransformerTest()
            {
                MockDestinationContentReferenceFinderFactory.Setup(p => p.ForDestinationContentType<IUser>()).Returns(MockUserContentFinder.Object);

                Transformer = new(MockDestinationContentReferenceFinderFactory.Object,
                    Create<ISharedResourcesLocalizer>(), Create<ILogger<GroupUsersTransformer>>());
            }
        }

        public class ExecuteAsync : GroupUsersTransformerTest
        {
            [Fact]
            public async Task ReturnsSameObjectAsync()
            {
                var group = Create<IPublishableGroup>();
                group.Users = CreateMany<GroupUser>().Cast<IGroupUser>().ToList();

                var destinationUsers = new List<IContentReference>();
                foreach (var user in group.Users)
                {
                    var destinationUser = Create<IContentReference>();
                    destinationUsers.Add(destinationUser);

                    MockUserContentFinder.Setup(x => x.FindBySourceLocationAsync(user.User.Location, Cancel))
                        .ReturnsAsync(destinationUser);
                }

                var result = await Transformer.TransformAsync(group, Cancel);

                Assert.NotNull(result);
                Assert.Same(group, result);
                Assert.Equal(destinationUsers, result.Users.Select(u => u.User).ToList());
            }

            [Fact]
            public async Task ThrowsWhenUserFoundAsync()
            {
                var group = Create<IPublishableGroup>();

                await Assert.ThrowsAsync<Exception>(() => Transformer.TransformAsync(group, Cancel));
            }
        }
    }
}
