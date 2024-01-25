using System.Threading.Tasks;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Api.Publishing;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Publishing
{
    public class WorkbookPublisherTests
    {
        public abstract class WorkbookPublisherTest : FilePublisherTestBase<IWorkbookPublisher, IPublishWorkbookOptions, IResultWorkbook>
        {
            internal readonly WorkbookPublisher WorkbookPublisher;

            protected override IWorkbookPublisher Publisher => WorkbookPublisher;

            public WorkbookPublisherTest()
                : base(RestUrlPrefixes.Workbooks)
            {
                WorkbookPublisher = CreateService<WorkbookPublisher>();
            }
        }

        public class PublishAsync : WorkbookPublisherTest
        {
            [Fact]
            public async Task Publishes()
            {
                var initiateResponse = SetupSuccessResponse<FileUploadResponse, FileUploadResponse.FileUploadType>();
                var getWorkbookResponse = SetupSuccessResponse<WorkbookResponse, WorkbookResponse.WorkbookType>();

                var publishOptions = Create<IPublishWorkbookOptions>();

                var result = await Publisher.PublishAsync(publishOptions, Cancel);

                Assert.True(result.Success);

                AssertRequests(
                    initiateResponse.Item,
                    r =>
                    {
                        r.AssertQuery("workbookType", publishOptions.FileType);
                        r.AssertQuery("overwrite", publishOptions.Overwrite.ToString().ToLower());
                    });

                Assert.Equal(getWorkbookResponse.Item.Id, result.Value.Id);
            }
        }
    }
}
