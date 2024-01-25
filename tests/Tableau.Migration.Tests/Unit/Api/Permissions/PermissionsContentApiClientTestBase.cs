using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Permissions;

namespace Tableau.Migration.Tests.Unit.Api.Permissions
{
    public abstract class PermissionsContentApiClientTestBase : ApiTestBase
    {
        internal readonly Mock<IPermissionsApiClient> MockPermissionsClient = new();

        public string UrlPrefix { get; }

        public PermissionsContentApiClientTestBase()
        {
            UrlPrefix = Create<string>();
            Setup(MockPermissionsClientFactory, MockPermissionsClient);
        }

        internal static void Setup(Mock<IPermissionsApiClientFactory> mockFactory, Mock<IPermissionsApiClient> mockClient)
        {
            mockFactory
                .Setup(f => f.Create(It.IsAny<IContentApiClient>()))
                .Returns(mockClient.Object);
        }
    }

    public abstract class PermissionsApiClientTestBase<TApiClient> : ApiClientTestBase<TApiClient>
        where TApiClient : class, IContentApiClient, IPermissionsContentApiClient
    {
        internal readonly Mock<IPermissionsApiClient> MockPermissionsClient = new();

        public PermissionsApiClientTestBase()
        {
            PermissionsContentApiClientTestBase.Setup(MockPermissionsClientFactory, MockPermissionsClient);
        }
    }
}
