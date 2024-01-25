using System.Collections.Concurrent;
using Moq;
using Tableau.Migration.Api.Permissions;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Config;
using Tableau.Migration.Net;
using Tableau.Migration.Net.Rest;
using Tableau.Migration.Resources;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Permissions
{
    public class PermissionsApiClientFactoryTests
    {
        public abstract class PermissionsApiClientFactoryTest : AutoFixtureTestBase
        {
            protected readonly Mock<IRestRequestBuilderFactory> MockRestRequestBuilderFactory = new();
            protected readonly Mock<IHttpContentSerializer> MockSerializer = new();
            protected readonly Mock<ISharedResourcesLocalizer> MockSharedResourcesLocalizer = new();
            protected readonly Mock<IConfigReader> MockConfigReader = new();

            protected readonly Mock<IPermissionsUriBuilder> MockUriBuilder = new();

            internal readonly PermissionsApiClientFactory Factory;

            public PermissionsApiClientFactoryTest()
            {
                Factory = new(
                    MockRestRequestBuilderFactory.Object,
                    MockSerializer.Object,
                    MockSharedResourcesLocalizer.Object,
                    MockConfigReader.Object);
            }
        }

        public class Create : PermissionsApiClientFactoryTest
        {
            [Fact]
            public void Creates()
            {
                var client = Factory.Create(MockUriBuilder.Object);

                Assert.Same(MockRestRequestBuilderFactory.Object, client.GetFieldValue("_restRequestBuilderFactory"));
                Assert.Same(MockSerializer.Object, client.GetFieldValue("_serializer"));
                Assert.Same(MockUriBuilder.Object, client.GetFieldValue("_uriBuilder"));
                Assert.Same(MockSharedResourcesLocalizer.Object, client.GetFieldValue("_sharedResourcesLocalizer"));
            }
        }

        public class CreateDefaultPermissionsClient : PermissionsApiClientFactoryTest
        {
            [Fact]
            public void Creates()
            {
                var sdkOptions = new MigrationSdkOptions
                {
                    DefaultPermissionsContentTypes = new(CreateMany<string>(10))
                };

                MockConfigReader.Setup(r => r.Get()).Returns(sdkOptions);

                var client = Factory.CreateDefaultPermissionsClient();

                Assert.Same(Factory, client.GetFieldValue("_permissionsClientFactory"));

                var contentTypeClients = Assert.IsType<ConcurrentDictionary<string, IPermissionsApiClient>>(client.GetFieldValue("_contentTypeClients"));

                foreach (var contentTypeUrlSegment in sdkOptions.DefaultPermissionsContentTypes.UrlSegments)
                    Assert.Contains(contentTypeUrlSegment, contentTypeClients.Keys);
            }
        }
    }
}
