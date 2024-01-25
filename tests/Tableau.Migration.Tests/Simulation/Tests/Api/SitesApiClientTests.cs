using System;
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models.Responses;
using Xunit;

namespace Tableau.Migration.Tests.Simulation.Tests.Api
{
    public class SitesApiClientTests
    {
        public class SitesApiClientTest : ApiClientTestBase
        { }

        public class GetSiteAsync : SitesApiClientTest
        {
            [Fact]
            public async Task SuccessWithIdAsync()
            {
                // Arrange 
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var site = Create<SiteResponse.SiteType>();
                Api.Data.Sites.Add(site);

                // Act
                var result = await sitesClient.GetSiteAsync(site.Id, Cancel);

                // Assert
                result.AssertSuccess();
                Assert.NotNull(result.Value);
            }

            [Fact]
            public async Task NotFoundWithIdAsync()
            {
                // Arrange 
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                // Act
                var result = await sitesClient.GetSiteAsync(Guid.NewGuid(), Cancel);

                // Assert
                result.AssertFailure();
                Assert.Null(result.Value);

                var error = Assert.Single(result.Errors);
                Assert.IsType<RestException>(error);
            }

            [Fact]
            public async Task SuccessWithContentUrlAsync()
            {
                // Arrange 
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var site = Create<SiteResponse.SiteType>();
                Api.Data.Sites.Add(site);

                // Act
                var result = await sitesClient.GetSiteAsync(site.ContentUrl!, Cancel);

                // Assert
                result.AssertSuccess();
                Assert.NotNull(result.Value);
            }

            [Fact]
            public async Task NotFoundWithContentUrlAsync()
            {
                // Arrange 
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                // Act
                var result = await sitesClient.GetSiteAsync("myContentUrl", Cancel);

                // Assert
                result.AssertFailure();
                Assert.Null(result.Value);

                var error = Assert.Single(result.Errors);
                Assert.IsType<RestException>(error);
            }
        }
    }
}
