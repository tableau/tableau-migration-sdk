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

using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Endpoints.Search;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Endpoints.Search
{
    public class BulkDestinationProjectCacheTests
    {
        public class LoadStoreAsync : BulkDestinationCacheTest<BulkDestinationProjectCache, IProject>
        {
            public LoadStoreAsync() 
            {
            }

            [Fact]
            public async Task PopulatesAllPagesFromProjectsAsync()
            {
                //Projects uses a breadth-first pager so that parent project paths
                //are built in the correct order.
                //A side-effect from that non-full/partial pages are returned when
                //a hierarchy boundary is reached.
                //Thus the cache shouldn't look for partial pages to optimize call counts,
                //and should compare to the total result count instead.

                var mockProjects = CreateMany<Mock<IProject>>().ToImmutableArray();

                var mockChildProject = mockProjects[1];
                mockChildProject.SetupGet(x => x.Location).Returns(new ContentLocation(mockProjects[0].Object.Name, mockChildProject.Object.Name));

                EndpointContent = mockProjects.Select(m => m.Object).ToList();
                ContentTypesOptions.BatchSize = EndpointContent.Count;
                
                var item = EndpointContent[1];

                var result = await Cache.ForLocationAsync(item.Location, Cancel);

                Assert.Equal(EndpointContent.Count, Cache.Count);
            }
        }

        public class IsProjectLockedAsync : BulkDestinationCacheTest<BulkDestinationProjectCache, IProject>
        {
            [Fact]
            public async Task NotLockedAsync()
            {
                var item = EndpointContent[1];

                //Populate cache
                await Cache.ForLocationAsync(item.Location, Cancel);

                var result = await Cache.IsProjectLockedAsync(item.Id, Cancel);

                Assert.False(result);
            }

            [Fact]
            public async Task FromStoreLoadAsync()
            {
                var mockLockedItem = Create<Mock<IProject>>();
                mockLockedItem.SetupGet(x => x.ContentPermissions).Returns(ContentPermissions.LockedToProject);

                EndpointContent[1] = mockLockedItem.Object;

                //Populate cache
                await Cache.ForLocationAsync(mockLockedItem.Object.Location, Cancel);

                var result = await Cache.IsProjectLockedAsync(mockLockedItem.Object.Id, Cancel);

                Assert.True(result);
            }

            [Fact]
            public async Task LockedWithoutNestedAsync()
            {
                var mockLockedItem = Create<Mock<IProject>>();
                mockLockedItem.SetupGet(x => x.ContentPermissions).Returns(ContentPermissions.LockedToProjectWithoutNested);

                EndpointContent[1] = mockLockedItem.Object;

                //Populate cache
                await Cache.ForLocationAsync(mockLockedItem.Object.Location, Cancel);

                var result = await Cache.IsProjectLockedAsync(mockLockedItem.Object.Id, Cancel);

                Assert.True(result);
            }

            [Fact]
            public async Task UpdatedAfterStoreLoadAsync()
            {
                var mockLockedItem = Create<Mock<IProject>>();

                EndpointContent[1] = mockLockedItem.Object;

                //Populate cache
                await Cache.ForLocationAsync(mockLockedItem.Object.Location, Cancel);

                var result = await Cache.IsProjectLockedAsync(mockLockedItem.Object.Id, Cancel);

                Assert.False(result);

                mockLockedItem.SetupGet(x => x.ContentPermissions).Returns(ContentPermissions.LockedToProject);
                Cache.UpdateLockedProjectCache(mockLockedItem.Object);

                result = await Cache.IsProjectLockedAsync(mockLockedItem.Object.Id, Cancel);

                Assert.True(result);
            }
        }
    }
}
