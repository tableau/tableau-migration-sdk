using Tableau.Migration.Api.Models;
using Tableau.Migration.Api.Rest.Models.Requests;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models
{
    public class CommitWorkbookPublishRequestTests
    {
        public abstract class CommitWorkbookPublishRequestTest : AutoFixtureTestBase
        { }

        public class Ctor : CommitWorkbookPublishRequestTest
        {
            [Fact]
            public void Initializes()
            {
                var options = Create<IPublishWorkbookOptions>();

                var request = new CommitWorkbookPublishRequest(options);

                Assert.NotNull(request.Workbook);

                Assert.Equal(options.Name, request.Workbook.Name);
                Assert.Equal(options.Description, request.Workbook.Description);
                Assert.Equal(options.ShowTabs, request.Workbook.ShowTabs);
                Assert.Equal(options.ThumbnailsUserId, request.Workbook.ThumbnailsUserId);
                Assert.Equal(options.EncryptExtracts, request.Workbook.EncryptExtracts);

                Assert.NotNull(request.Workbook.Project);

                Assert.Equal(options.ProjectId, request.Workbook.Project.Id);
            }
        }
    }
}
