using Tableau.Migration.Api;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Simulation;
using Tableau.Migration.Engine.Endpoints;

namespace Tableau.Migration.Tests.Simulation
{
    /// <summary>
    /// <see cref="SimulationTestBase"/> implementation for test classes that require a single server (i.e. for API testing).
    /// </summary>
    public abstract class SingleServerSimulationTestBase : SimulationTestBase
    {
        protected TableauApiSimulator Api { get; }

        protected TableauSiteConnectionConfiguration SiteConfig { get; }

        protected TableauApiEndpointConfiguration EndpointConfig { get; }

        public SingleServerSimulationTestBase(bool isCloud = false)
        {
            if (isCloud)
            {
                Api = RegisterCloudApiSimulator("https://destination", Create<UsersResponse.UserType>());
            }
            else
            {
                Api = RegisterApiSimulator("https://server", Create<UsersResponse.UserType>());
            }

            SiteConfig = BuildSiteConnectionConfiguration(Api);
            EndpointConfig = new(SiteConfig);
        }
    }
}
