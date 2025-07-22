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

using System;
using System.Collections.Immutable;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Endpoints.Caching;
using Tableau.Migration.Engine.Endpoints.ContentClients;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Endpoints
{
    public sealed class IMigrationEndpointTests
    {
        public abstract class IMigrationEndpointTest : AutoFixtureTestBase
        {
            protected readonly Mock<IMigrationEndpoint> MockEndpoint;

            protected IMigrationEndpoint Endpoint => MockEndpoint.Object;

            public IMigrationEndpointTest()
            {
                MockEndpoint = new Mock<IMigrationEndpoint>();
                MockEndpoint.CallBase = true;
            }
        }

        public sealed class GetViewCache : IMigrationEndpointTest
        {
            [Fact]
            public void GetsViewCache()
            {
                var client = Endpoint.GetViewCache();

                MockEndpoint.Verify(x => x.GetEndpointCache<IEndpointViewCache, Guid, IView>(), Times.Once);
            }
        }

        public sealed class GetWorkbookViewsCache : IMigrationEndpointTest
        {
            [Fact]
            public void GetsWorkbookViewsCache()
            {
                var client = Endpoint.GetWorkbookViewsCache();

                MockEndpoint.Verify(x => x.GetEndpointCache<IEndpointWorkbookViewsCache, Guid, IImmutableList<IView>>(), Times.Once);
            }
        }

        public sealed class GetFavoritesContentClient : IMigrationEndpointTest
        {
            [Fact]
            public void GetsTypedContentClient()
            {
                var client = Endpoint.GetFavoritesContentClient();

                MockEndpoint.Verify(x => x.GetContentClient<IFavoritesContentClient, IFavorite>(), Times.Once);
            }
        }

        public sealed class GetViewsContentClient : IMigrationEndpointTest
        {
            [Fact]
            public void GetsTypedContentClient()
            {
                var client = Endpoint.GetViewsContentClient();

                MockEndpoint.Verify(x => x.GetContentClient<IViewsContentClient, IView>(), Times.Once);
            }
        }

        public sealed class GetWorkbooksContentClient : IMigrationEndpointTest
        {
            [Fact]
            public void GetsTypedContentClient()
            {
                var client = Endpoint.GetWorkbookContentClient();

                MockEndpoint.Verify(x => x.GetContentClient<IWorkbooksContentClient, IWorkbook>(), Times.Once);
            }
        }
    }
}
