using System.Threading.Tasks;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Api.Publishing;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Publishing
{
    public class DataSourcePublisherTests
    {
        public abstract class DataSourcePublisherTest : FilePublisherTestBase<IDataSourcePublisher, IPublishDataSourceOptions, IDataSource>
        {
            internal readonly DataSourcePublisher DataSourcePublisher;

            protected override IDataSourcePublisher Publisher => DataSourcePublisher;

            public DataSourcePublisherTest()
                : base(RestUrlPrefixes.DataSources)
            {
                DataSourcePublisher = CreateService<DataSourcePublisher>();
            }
        }

        public class PublishAsync : DataSourcePublisherTest
        {
            [Fact]
            public async Task Publishes()
            {
                var initiateResponse = SetupSuccessResponse<FileUploadResponse, FileUploadResponse.FileUploadType>();
                var getDataSourceResponse = SetupSuccessResponse<DataSourceResponse, DataSourceResponse.DataSourceType>();

                var publishOptions = Create<IPublishDataSourceOptions>();

                var result = await DataSourcePublisher.PublishAsync(publishOptions, Cancel);

                Assert.True(result.Success);

                AssertRequests(
                    initiateResponse.Item,
                    r =>
                    {
                        r.AssertQuery("datasourceType", publishOptions.FileType);
                        r.AssertQuery("overwrite", publishOptions.Overwrite.ToString().ToLower());
                    });

                Assert.Equal(getDataSourceResponse.Item.Id, result.Value.Id);
            }
        }
    }
}
