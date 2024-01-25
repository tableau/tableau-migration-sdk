using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Requests;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Simulation;
using Tableau.Migration.Api.Simulation.Rest.Net.Responses;
using Tableau.Migration.Content.Permissions;
using Tableau.Migration.Net;
using Tableau.Migration.Net.Rest;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Simulation.Rest.Net.Responses
{
    public class RestPermissionsCreateResponseBuilderTests
    {
        public abstract class RestPermissionsCreateResponseBuilderTest : ResponseBuilderTestBase
        { }

        public class RespondAsync : RestPermissionsCreateResponseBuilderTest
        {
            [Fact]
            public async Task DenyProjectLeaderErrorAsync()
            {
                var data = new TableauData(Create<UsersResponse.UserType>());

                var siteId = data.SignIn!.Site!.Id;
                var proj = data.AddProject(new()
                {
                    Id = Guid.NewGuid()
                });

                var builder = new RestPermissionsCreateResponseBuilder<ProjectsResponse.ProjectType>(
                    data,
                    Serializer,
                    "projects",
                    d => d.Projects);

                var capability = new Capability(new() { Name = PermissionsCapabilityNames.ProjectLeader, Mode = PermissionsCapabilityModes.Deny });
                var grantee = new GranteeCapability(GranteeType.User, data.SignIn.User!.Id, new[] { capability });
                var permissions = new Migration.Content.Permissions.Permissions(proj.Id, new[] { grantee });
                var requestContent = new PermissionsAddRequest(permissions);

                var request = new HttpRequestMessage(HttpMethod.Put, $"https://localhost/api/1.0/sites/{siteId.ToUrlSegment()}/projects/{proj.Id.ToUrlSegment()}/permissions");
                request.Content = Serializer.Serialize(requestContent, MediaTypes.Xml);
                request.Headers.TryAddWithoutValidation(RestHeaders.AuthenticationToken, data.SignIn.Token);

                var response = await builder.RespondAsync(request, Cancel);

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                Assert.NotNull(response.Content);

                var responseContent = await Serializer.DeserializeAsync<PermissionsResponse>(response.Content, Cancel);
                Assert.NotNull(responseContent);
                Assert.NotNull(responseContent.Error);
                Assert.Equal("400009", responseContent.Error.Code);
            }
        }
    }
}
