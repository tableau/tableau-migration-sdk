using System;
using Tableau.Migration.Api;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api
{
    public class TableauSiteConnectionConfigurationTests
    {
        public class Empty
        {
            [Fact]
            public void HasEmptyValues()
            {
                var empty = TableauSiteConnectionConfiguration.Empty;

                Assert.Equal("https://localhost/", empty.ServerUrl.ToString());
                Assert.Empty(empty.SiteContentUrl);
                Assert.Empty(empty.AccessTokenName);
                Assert.Empty(empty.AccessToken);
            }
        }

        public class Validate
        {
            [Fact]
            public void Valid()
            {
                var c = new TableauSiteConnectionConfiguration(new Uri("https://localhost"), "site", "tokenName", "token");

                var vr = c.Validate();

                vr.AssertSuccess();
            }

            [Fact]
            public void InvalidAccessTokenName()
            {
                var c = new TableauSiteConnectionConfiguration(new Uri("https://localhost"), "site", "", "token");

                var vr = c.Validate();

                vr.AssertFailure();
                Assert.Single(vr.Errors);
            }

            [Fact]
            public void InvalidAccessToken()
            {
                var c = new TableauSiteConnectionConfiguration(new Uri("https://localhost"), "site", "tokenName", "");

                var vr = c.Validate();

                vr.AssertFailure();
                Assert.Single(vr.Errors);
            }
        }
    }
}
