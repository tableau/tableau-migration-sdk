using System.Collections.Generic;
using System.Threading.Tasks;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Permissions;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Content.Permissions;
using Tableau.Migration.Tests.Content.Permissions;
using Xunit;

namespace Tableau.Migration.Tests.Simulation.Tests.Api
{
    public abstract class PermissionsContentApiClientTestBase<TApiClient, TContent> : ApiClientTestBase<TApiClient, TContent>
        where TApiClient : IContentApiClient, IPermissionsContentApiClient
        where TContent : IRestIdentifiable, INamedContent, new()
    {
        protected abstract ICollection<TContent> GetContentData();

        [Fact]
        public async Task GetPermissionsAsync()
        {
            // Arrange 
            await using var sitesClient = await GetSitesClientAsync(Cancel);

            var permissionsClient = GetApiClient();

            var permissions = Create<PermissionsType>();

            var contentItem = Api.Data.AddContentTypePermissions(UrlPrefix, GetContentData, CreateContentItem, permissions);

            // Act
            var result = await permissionsClient.Permissions.GetPermissionsAsync(contentItem.Id, Cancel);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Value);
            Assert.True(IPermissionsComparer.Instance.Equals(permissions, result.Value));
        }

        [Fact]
        public async Task CreatePermissionsAsync()
        {
            // Arrange 
            await using var sitesClient = await GetSitesClientAsync(Cancel);

            var permissionsClient = GetApiClient();

            var content = CreateContentItem();

            GetContentData().Add(content);

            var permissions = Create<IPermissions>();
            permissions.ParentId = content.Id;

            // Act
            var result = await permissionsClient.Permissions.CreatePermissionsAsync(content.Id, permissions, Cancel);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Value);
            Assert.True(IPermissionsComparer.Instance.Equals(permissions, result.Value));
        }
    }
}
