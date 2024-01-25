using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Hooks.Transformers.Default;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Transformers.Default
{
    public class UserTableauCloudAuthenticationTypeTransformerTests
    {
        public class ExecuteAsync : OptionsHookTestBase<UserAuthenticationTypeTransformerOptions>
        {
            [Fact]
            public async Task SetsServerDefaultAuthType()
            {
                var mockUser = new Mock<IUser>();

                Options = new UserAuthenticationTypeTransformerOptions
                {
                    AuthenticationType = AuthenticationTypes.TableauIdWithMfa
                };

                var t = new UserAuthenticationTypeTransformer(MockOptionsProvider.Object);

                var resultUser = await t.ExecuteAsync(mockUser.Object, default);

                Assert.Same(resultUser, mockUser.Object);
                mockUser.VerifySet(x => x.AuthenticationType = AuthenticationTypes.TableauIdWithMfa, Times.Once);
            }
        }
    }
}
