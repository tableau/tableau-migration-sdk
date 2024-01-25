﻿using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Content.Files;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Endpoints.Search;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Endpoints
{
    public class TableauApiDestinationEndpointTests
    {
        public class TableauApiDestinationEndpointTest : TableauApiEndpointTestBase<TableauApiDestinationEndpoint>
        {
            protected readonly TableauApiDestinationEndpoint Endpoint;

            public TableauApiDestinationEndpointTest()
            {
                Endpoint = new(MigrationServices.GetRequiredService<IServiceScopeFactory>(),
                    Create<ITableauApiEndpointConfiguration>(),
                    Create<ManifestDestinationContentReferenceFinderFactory>(),
                    Create<IContentFileStore>()
                );
            }
        }

        #region - PublishAsync -

        public class PublishAsync : TableauApiDestinationEndpointTest
        {
            private readonly Mock<IPublishApiClient<TestContentType>> _mockPublishClient;

            public PublishAsync()
            {
                _mockPublishClient = Create<Mock<IPublishApiClient<TestContentType>>>();
                MockSiteApi.Setup(x => x.GetPublishApiClient<TestContentType, TestContentType>()).Returns(_mockPublishClient.Object);
            }

            [Fact]
            public async Task UsesPublishClientAsync()
            {
                var publishResult = Freeze<IResult<TestContentType>>();

                await Endpoint.InitializeAsync(Cancel);

                var item = new TestContentType();

                var result = await Endpoint.PublishAsync<TestContentType, TestContentType>(item, Cancel);

                Assert.Same(publishResult, result);

                MockSiteApi.Verify(x => x.GetPublishApiClient<TestContentType, TestContentType>(), Times.Once);
                _mockPublishClient.Verify(x => x.PublishAsync(item, Cancel), Times.Once);
            }
        }

        #endregion

        #region - PublishBatchAsync -

        public class PublishBatchAsync : TableauApiDestinationEndpointTest
        {
            private readonly Mock<IBatchPublishApiClient<TestContentType>> _mockBatchPublishClient;

            public PublishBatchAsync()
            {
                _mockBatchPublishClient = Create<Mock<IBatchPublishApiClient<TestContentType>>>();
                MockSiteApi.Setup(x => x.GetBatchPublishApiClient<TestContentType>()).Returns(_mockBatchPublishClient.Object);
            }

            [Fact]
            public async Task UsesBatchPublishClientAsync()
            {
                var publishBatchResult = Freeze<IResult>();

                await Endpoint.InitializeAsync(Cancel);

                var items = new TestContentType[] { new(), new(), new() };

                var result = await Endpoint.PublishBatchAsync(items, Cancel);

                Assert.Same(publishBatchResult, result);

                MockSiteApi.Verify(x => x.GetBatchPublishApiClient<TestContentType>(), Times.Once);
                _mockBatchPublishClient.Verify(x => x.PublishBatchAsync(items, Cancel), Times.Once);
            }
        }

        #endregion
    }
}
