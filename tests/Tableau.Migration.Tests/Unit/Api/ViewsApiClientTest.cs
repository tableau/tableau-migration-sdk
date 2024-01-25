using System;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Content.Permissions;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api
{
    public class ViewsApiClientTest : AutoFixtureTestBase
    {
        internal readonly Mock<IViewsApiClient> MockPermissionsClient;


        public ViewsApiClientTest()
        {
            MockPermissionsClient = new Mock<IViewsApiClient>();
        }

        #region - Test Helpers -

        public void SetupGetPermissionsAsync(bool success, IPermissions? permissions = null)
        {
            var setup = MockPermissionsClient
                .Setup(c => c.Permissions.GetPermissionsAsync(
                    It.IsAny<Guid>(),
                    Cancel));

            if (success)
            {
                Assert.NotNull(permissions);

                setup.Returns(
                    Task.FromResult<IResult<IPermissions>>(
                        Result<IPermissions>.Create(Result.Succeeded(), permissions)));
                return;
            }

            setup.Returns(Task.FromResult<IResult<IPermissions>>(Result<IPermissions>.Failed(new Exception())));
        }

        public void VerifyGetPermissionsAsync(Times times)
        {
            MockPermissionsClient
                .Verify(c => c.Permissions.GetPermissionsAsync(
                        It.IsAny<Guid>(),
                        Cancel),
                    times);
        }

        #endregion

        public class GetPermissionsAsync : ViewsApiClientTest
        {
            [Fact]
            public async Task Success()
            {
                var sourcePermissions = Create<IPermissions>();
                var destinationPermissions = Create<IPermissions>();

                SetupGetPermissionsAsync(true, destinationPermissions);

                var result = await MockPermissionsClient.Object.Permissions.GetPermissionsAsync(
                    Guid.NewGuid(),
                    Cancel);

                Assert.True(result.Success);

                // Get permissions is called once.
                VerifyGetPermissionsAsync(Times.Once());
            }
        }
    }
}