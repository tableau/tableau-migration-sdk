using Tableau.Migration.Api.Models;
using Tableau.Migration.Api.Rest.Models.Requests;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models
{
    public class CommitDataSourcePublishRequestTests
    {
        public abstract class CommitDataSourcePublishRequestTest : AutoFixtureTestBase
        { }

        public class Ctor : CommitDataSourcePublishRequestTest
        {
            [Fact]
            public void Initializes()
            {
                var options = Create<IPublishDataSourceOptions>();

                var request = new CommitDataSourcePublishRequest(options);

                Assert.NotNull(request.DataSource);

                Assert.Equal(options.Name, request.DataSource.Name);
                Assert.Equal(options.Description, request.DataSource.Description);
                Assert.Equal(options.UseRemoteQueryAgent, request.DataSource.UseRemoteQueryAgent);
                Assert.Equal(options.EncryptExtracts, request.DataSource.EncryptExtracts);

                Assert.NotNull(request.DataSource.Project);

                Assert.Equal(options.ProjectId, request.DataSource.Project.Id);
            }
        }
    }
}
