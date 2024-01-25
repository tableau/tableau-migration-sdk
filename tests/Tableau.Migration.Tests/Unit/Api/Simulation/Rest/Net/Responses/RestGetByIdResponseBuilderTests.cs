using System;
using System.Net.Http;
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Simulation;
using Tableau.Migration.Api.Simulation.Rest.Net.Responses;
using Tableau.Migration.Net.Rest;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Simulation.Rest.Net.Responses
{
    public class RestGetByIdResponseBuilderTests
    {
        public abstract class RestGetByIdResponseBuilderTest : ResponseBuilderTestBase
        { }

        public class RespondAsync : RestGetByIdResponseBuilderTest
        {
            [Fact]
            public async Task Creates_response()
            {
                var data = new TableauData(Create<UsersResponse.UserType>());

                const int SITE_COUNT = 10;

                var id = Guid.Empty;
                for (var i = 0; i < SITE_COUNT; i++)
                {
                    var site = Create<SiteResponse.SiteType>();
                    data.Sites.Add(site);
                    if (i == 4)
                        id = site.Id;
                }

                var builder = new RestGetByIdResponseBuilder<SiteResponse, SiteResponse.SiteType>(
                    data,
                    Serializer,
                    (d, _) => d.Sites,
                    false);

                var response = await builder.RespondAsync(
                    new HttpRequestMessage(HttpMethod.Get, $"https://localhost/api/1.0/sites/1c46d050-5d08-4a4a-9ff2-4e58978d6e2b/workbooks/{id.ToUrlSegment()}/"),
                    Cancel);

                Assert.NotNull(response.Content);

                var deserialized = await Serializer.DeserializeAsync<SiteResponse>(response.Content, Cancel);

                Assert.NotNull(deserialized);

                Assert.Null(deserialized.Error);
                Assert.NotNull(deserialized.Item);
                Assert.Equal(id, deserialized.Item.Id);
            }
        }
    }
}
