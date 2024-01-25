using System;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Api.Rest.Models.Responses;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Models
{
    public class SignInResultTests
    {
        public class Ctor : AutoFixtureTestBase
        {
            [Fact]
            public void ThrowsWhenCredentialsNull()
            {
                var response = Create<SignInResponse>();
                response.Item = null;

                var exception = Assert.Throws<ArgumentNullException>(() => new SignInResult(response));

                Assert.Equal("response.Item", exception.ParamName);
            }

            [Fact]
            public void ThrowsWhenCredentialsUserNull()
            {
                var response = Create<SignInResponse>();
                response.Item!.User = null;

                var exception = Assert.Throws<ArgumentNullException>(() => new SignInResult(response));

                Assert.Equal("response.Item.User", exception.ParamName);
            }

            [Fact]
            public void ThrowsWhenCredentialsSiteNull()
            {
                var response = Create<SignInResponse>();
                response.Item!.Site = null;

                var exception = Assert.Throws<ArgumentNullException>(() => new SignInResult(response));

                Assert.Equal("response.Item.Site", exception.ParamName);
            }

            [Theory]
            [NullEmptyWhiteSpaceData]
            public void ThrowsWhenCredentialsTokenNullEmptyOrWhiteSpace(string? token)
            {
                var response = Create<SignInResponse>();
                response.Item!.Token = token;

                var exception = Assert.Throws<ArgumentException>(() => new SignInResult(response));

                Assert.Equal("response.Item.Token", exception.ParamName);
            }

            [Fact]
            public void ThrowsWhenCredentialsUserIdEmpty()
            {
                var response = Create<SignInResponse>();
                response.Item!.User!.Id = Guid.Empty;

                var exception = Assert.Throws<ArgumentException>(() => new SignInResult(response));

                Assert.Equal("response.Item.User.Id", exception.ParamName);
            }

            [Fact]
            public void ThrowsWhenCredentialsSiteIdEmpty()
            {
                var response = Create<SignInResponse>();
                response.Item!.Site!.Id = Guid.Empty;

                var exception = Assert.Throws<ArgumentException>(() => new SignInResult(response));

                Assert.Equal("response.Item.Site.Id", exception.ParamName);
            }

            [Theory]
            [InlineData(null)]
            [InlineData(" ")]
            public void ThrowsWhenCredentialsSiteContentUrlNullOrWhiteSpace(string? contentUrl)
            {
                var response = Create<SignInResponse>();
                response.Item!.Site!.ContentUrl = contentUrl;

                var exception = Assert.Throws<ArgumentException>(() => new SignInResult(response));

                Assert.Equal("response.Item.Site.ContentUrl", exception.ParamName);
            }

            [Fact]
            public void AllowsEmptySiteContentUrl()
            {
                var response = Create<SignInResponse>();
                response.Item!.Site!.ContentUrl = String.Empty;

                _ = new SignInResult(response);
            }
        }
    }
}
