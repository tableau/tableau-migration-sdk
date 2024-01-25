using System;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content
{
    public class UserTests
    {
        public class Ctor : AutoFixtureTestBase
        {
            protected UsersResponse.UserType CreateTestResponse()
            {
                return new UsersResponse.UserType
                {
                    Domain = new()
                    {
                        Name = Create<string>()
                    },
                    Id = Create<Guid>(),
                    Name = Create<string>(),
                    FullName = Create<string>(),
                    Email = Create<string>(),
                    SiteRole = Create<string>(),
                    Language = Create<string>(),
                    Locale = Create<string?>(),
                    AuthSetting = Create<string?>(),
                };
            }

            [Fact]
            public void DomainObjectRequired()
            {
                var response = CreateTestResponse();
                response.Domain = null;

                Assert.Throws<ArgumentNullException>(() => new User(response));
            }

            [Fact]
            public void EmptyId()
            {
                var response = CreateTestResponse();
                response.Id = Guid.Empty;

                Assert.Throws<ArgumentException>(() => new User(response));
            }

            [Theory]
            [NullEmptyWhiteSpaceData]
            public void UsernameRequired(string? name)
            {
                var response = CreateTestResponse();
                response.Name = name;

                Assert.Throws<ArgumentException>(() => new User(response));
            }

            [Theory]
            [NullEmptyWhiteSpaceData]
            public void DomainNameRequired(string? name)
            {
                var response = CreateTestResponse();
                response.Domain!.Name = name;

                Assert.Throws<ArgumentException>(() => new User(response));
            }

            [Fact]
            public void BuildsLocation()
            {
                var response = CreateTestResponse();

                var user = new User(response);

                Assert.Equal(ContentLocation.ForUsername(response.Domain?.Name!, response.Name!), user.Location);
            }

            [Theory]
            [NullEmptyWhiteSpaceData]
            public void AuthTypeOptional(string? authSetting)
            {
                var response = CreateTestResponse();
                response.AuthSetting = authSetting;

                var user = new User(response);

                Assert.Equal(authSetting, user.AuthenticationType);
            }
        }
    }
}
