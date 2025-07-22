//
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
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Endpoints.Caching;
using Tableau.Migration.Engine.Hooks.PostPublish;
using Tableau.Migration.Engine.Hooks.PostPublish.Default;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.PostPublish.Default
{
    public sealed class PopulateViewCachePostPublishHookTests
    {
        public abstract class PopulateViewCachePostPublishHookTest : AutoFixtureTestBase
        {
            protected readonly Mock<IEndpointViewCache> MockSourceViewCache;
            protected readonly Mock<IEndpointWorkbookViewsCache> MockSourceWorkbookViewsCache;

            protected readonly Mock<IEndpointViewCache> MockDestinationViewCache;
            protected readonly Mock<IEndpointWorkbookViewsCache> MockDestinationWorkbookViewsCache;

            internal readonly PopulateViewCachePostPublishHook Hook;

            public PopulateViewCachePostPublishHookTest()
            {
                MockSourceViewCache = Create<Mock<IEndpointViewCache>>();
                MockSourceWorkbookViewsCache = Create<Mock<IEndpointWorkbookViewsCache>>();

                MockDestinationViewCache = Create<Mock<IEndpointViewCache>>();
                MockDestinationWorkbookViewsCache = Create<Mock<IEndpointWorkbookViewsCache>>();

                var mockSource = Freeze<Mock<ISourceEndpoint>>();
                mockSource.Setup(x => x.GetViewCache()).Returns(MockSourceViewCache.Object);
                mockSource.Setup(x => x.GetWorkbookViewsCache()).Returns(MockSourceWorkbookViewsCache.Object);

                var mockDestination = Freeze<Mock<IDestinationEndpoint>>();
                mockDestination.Setup(x => x.GetViewCache()).Returns(MockDestinationViewCache.Object);
                mockDestination.Setup(x => x.GetWorkbookViewsCache()).Returns(MockDestinationWorkbookViewsCache.Object);

                Hook = Create<PopulateViewCachePostPublishHook>();
            }
        }

        public sealed class ExecuteAsync : PopulateViewCachePostPublishHookTest
        {
            [Fact]
            public async Task PopulatesSourceAndDestinationViewCachesAsync()
            {
                var ctx = Create<ContentItemPostPublishContext<IPublishableWorkbook, IWorkbookDetails>>();

                var result = await Hook.ExecuteAsync(ctx, Cancel);

                Assert.Same(ctx, result);

                foreach(var sourceView in ctx.PublishedItem.Views)
                {
                    MockSourceViewCache.Verify(x => x.Add(sourceView.Id, sourceView), Times.Once);
                }

                MockSourceWorkbookViewsCache.Verify(x => x.Add(ctx.PublishedItem.Id, ctx.PublishedItem.Views), Times.Once);

                foreach (var destinationView in ctx.DestinationItem.Views)
                {
                    MockDestinationViewCache.Verify(x => x.Add(destinationView.Id, destinationView), Times.Once);
                }

                MockDestinationWorkbookViewsCache.Verify(x => x.Add(ctx.DestinationItem.Id, ctx.DestinationItem.Views), Times.Once);
            }
        }
    }
}
