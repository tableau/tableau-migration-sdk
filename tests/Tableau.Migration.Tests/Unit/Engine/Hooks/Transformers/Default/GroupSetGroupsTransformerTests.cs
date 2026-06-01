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
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Engine.Hooks.Transformers.Default;
using Tableau.Migration.Engine.Manifest;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Transformers.Default
{
    public sealed class GroupSetGroupsTransformerTests
    {
        public abstract class GroupSetGroupsTransformerTest : AutoFixtureTestBase
        {
            protected readonly Mock<IDestinationContentReferenceFinder<IGroup>> MockGroupFinder;

            protected readonly GroupSetGroupsTransformer Transformer;

            protected Dictionary<ContentLocation, IContentReference> GroupMappings { get; set; } = new();
            protected Dictionary<ContentLocation, MigrationManifestEntryStatus> StatusMappings { get; set; } = new();

            public GroupSetGroupsTransformerTest()
            {
                MockGroupFinder = Freeze<Mock<IDestinationContentReferenceFinder<IGroup>>>();
                MockGroupFinder.Setup(x => x.FindResultBySourceLocationAsync(It.IsAny<ContentLocation>(), Cancel))
                    .ReturnsAsync((ContentLocation loc, CancellationToken c) =>
                    {
                        if (GroupMappings.TryGetValue(loc, out var result))
                        {
                            if(StatusMappings.TryGetValue(loc, out var status))
                            {
                                return new(status, result);
                            }

                            return new(MigrationManifestEntryStatus.Migrated, result);
                        }

                        return DestinationContentReferenceResult.Empty;
                    });

                var mockFinderFactory = Freeze<Mock<IDestinationContentReferenceFinderFactory>>();
                mockFinderFactory.Setup(x => x.ForDestinationContentType<IGroup>()).Returns(MockGroupFinder.Object);

                Freeze(new MockSharedResourcesLocalizer().Object);

                Transformer = Create<GroupSetGroupsTransformer>();
            }
        }

        public class TransformAsync : GroupSetGroupsTransformerTest
        {
            [Fact]
            public async Task TransformsGroupsAsync()
            {
                var groupSet = Create<IPublishableGroupSet>();
                GroupMappings = groupSet.Groups.ToDictionary(g => g.Location, g => Create<IContentReference>());

                var result = await Transformer.TransformAsync(groupSet, Cancel);

                Assert.NotNull(result);
                Assert.Same(groupSet, result);

                Assert.Equal(GroupMappings.Values, result.Groups);
            }

            [Fact]
            public async Task AutomaticCascadeFilterManyToManyAsync()
            {
                var groupSet = Create<IPublishableGroupSet>();
                GroupMappings = groupSet.Groups.ToDictionary(g => g.Location, g => Create<IContentReference>());
                StatusMappings = groupSet.Groups.ToDictionary(g => g.Location, g => MigrationManifestEntryStatus.Skipped);

                var result = await Transformer.TransformAsync(groupSet, Cancel);

                Assert.NotNull(result);
                Assert.Same(groupSet, result);

                Assert.Empty(result.Groups);
            }

            [Fact]
            public async Task ThrowsOnMissingGroupsAsync()
            {
                var groupSet = Create<IPublishableGroupSet>();

                GroupMappings = groupSet.Groups
                    .Skip(1)
                    .SkipLast(1)
                    .ToDictionary(g => g.Location, g => Create<IContentReference>());

                var missingGroups = ImmutableArray.Create(groupSet.Groups.First(), groupSet.Groups.Last());

                var ex = await Assert.ThrowsAsync<Exception>(() => Transformer.TransformAsync(groupSet, Cancel));

                foreach (var missingGroup in missingGroups)
                {
                    Assert.Contains(missingGroup.Location.ToString(), ex.Message);
                }
            }
        }
    }
}
