using System;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Models;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api
{
    public class ServerSessionProviderTests
    {
        public abstract class ServerSessionProviderTest : AutoFixtureTestBase
        {
            protected readonly Mock<IAuthenticationTokenProvider> MockTokenProvider = new();
            protected readonly Mock<ITableauServerVersionProvider> MockVersionProvider = new();

            internal readonly ServerSessionProvider Provider;

            public ServerSessionProviderTest()
            {
                Provider = new(MockVersionProvider.Object, MockTokenProvider.Object);
            }
        }

        public class AuthenticationToken : ServerSessionProviderTest
        {
            [Fact]
            public void Returns_token()
            {
                var token = Create<string>();

                MockTokenProvider.SetupGet(p => p.Token).Returns(token);

                Assert.Equal(token, Provider.AuthenticationToken);
            }
        }

        public class Version : ServerSessionProviderTest
        {
            [Fact]
            public void Returns_version()
            {
                var version = Create<TableauServerVersion>();

                MockVersionProvider.SetupGet(p => p.Version).Returns(version);

                Assert.Equal(version, Provider.Version);
            }
        }

        public class SetCurrentUserAndSite
        {
            public class ISignInResult_Overload : ServerSessionProviderTest
            {
                [Fact]
                public void Sets_user_and_site()
                {
                    var signInResult = Create<ISignInResult>();

                    Provider.SetCurrentUserAndSite(signInResult);

                    Assert.Equal(signInResult.UserId, Provider.UserId);
                    Assert.Equal(signInResult.SiteContentUrl, Provider.SiteContentUrl);
                    Assert.Equal(signInResult.SiteId, Provider.SiteId);

                    MockTokenProvider.Verify(p => p.Set(signInResult.Token), Times.Once);
                }
            }

            public class Values_Overload : ServerSessionProviderTest
            {
                [Fact]
                public void Sets_user_and_site()
                {
                    var userId = Create<Guid>();
                    var siteContentUrl = Create<string>();
                    var siteId = Create<Guid>();
                    var token = Create<string>();

                    Provider.SetCurrentUserAndSite(userId, siteId, siteContentUrl, token);

                    Assert.Equal(userId, Provider.UserId);
                    Assert.Equal(siteContentUrl, Provider.SiteContentUrl);
                    Assert.Equal(siteId, Provider.SiteId);

                    MockTokenProvider.Verify(p => p.Set(token), Times.Once);
                }
            }
        }

        public class ClearCurrentUserAndSite : ServerSessionProviderTest
        {
            [Fact]
            public void Clears_user()
            {
                var signInResult = Create<ISignInResult>();

                Provider.SetCurrentUserAndSite(signInResult);

                Provider.ClearCurrentUserAndSite();

                Assert.Null(Provider.UserId);
                Assert.Null(Provider.SiteContentUrl);
                Assert.Null(Provider.SiteId);

                MockTokenProvider.Verify(p => p.Clear(), Times.Once);
            }
        }

        public class SetVersion : ServerSessionProviderTest
        {
            [Fact]
            public void Sets_version()
            {
                var version = Create<TableauServerVersion>();

                Provider.SetVersion(version);

                MockVersionProvider.Verify(p => p.Set(version), Times.Once);
            }
        }
    }
}
