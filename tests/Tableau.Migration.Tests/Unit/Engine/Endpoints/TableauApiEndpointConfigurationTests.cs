using System;
using Tableau.Migration.Api;
using Tableau.Migration.Engine.Endpoints;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Endpoints
{
    public class TableauApiEndpointConfigurationTests
    {
        public class Empty
        {
            [Fact]
            public void HasEmptySiteConnectionConfig()
            {
                var empty = TableauApiEndpointConfiguration.Empty;
                Assert.Equal(TableauSiteConnectionConfiguration.Empty, empty.SiteConnectionConfiguration);
            }
        }

        public class Validate
        {
            [Fact]
            public void ValidSiteConnectionConfig()
            {
                var c = new TableauApiEndpointConfiguration(new(new Uri("https://localhost"), "site", "tokenName", "token"));

                var vr = c.Validate();

                vr.AssertSuccess();
            }

            [Fact]
            public void InvalidSiteConnectionConfig()
            {
                var c = new TableauApiEndpointConfiguration(new(new Uri("https://localhost"), "", "tokenName", ""));

                var vr = c.Validate();

                vr.AssertFailure();
                Assert.Single(vr.Errors);
            }
        }
    }
}
