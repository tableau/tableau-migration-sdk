using System;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Responses;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Models
{
    public class UpdateProjectResultTests
    {
        public class Ctor : AutoFixtureTestBase
        {
            [Fact]
            public void Initializes()
            {
                var response = new UpdateProjectResponse
                {
                    Item = Create<UpdateProjectResponse.ProjectType>()
                };

                response.Item.ParentProjectId = Guid.NewGuid().ToString();
                response.Item.ControllingPermissionsProjectId = Guid.NewGuid().ToString();
                response.Item.Owner = new() { Id = Guid.NewGuid() };

                var result = new UpdateProjectResult(response);

                Assert.Equal(response.Item.Id, result.Id);
                Assert.Equal(response.Item.GetParentProjectId(), result.ParentProjectId);
                Assert.Equal(response.Item.Name, result.Name);
                Assert.Equal(response.Item.Description, result.Description);
                Assert.Equal(response.Item.ContentPermissions, result.ContentPermissions);
                Assert.Equal(response.Item.GetControllingPermissionsProjectId(), result.ControllingPermissionsProjectId);
                Assert.Equal(response.Item.Owner.Id, result.OwnerId);
            }
        }
    }
}
