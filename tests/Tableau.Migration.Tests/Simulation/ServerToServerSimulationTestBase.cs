using Tableau.Migration.Api;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Simulation;
using Tableau.Migration.Engine.Endpoints;

namespace Tableau.Migration.Tests.Simulation
{
    /// <summary>
    /// <see cref="SimulationTestBase"/> implementation for test classes that require source and cloud destination servers (i.e. for migrations testing).
    /// </summary>
    public abstract class ServerToServerSimulationTestBase : SimulationTestBase
    {
        protected TableauApiSimulator SourceApi { get; }

        protected TableauSiteConnectionConfiguration SourceSiteConfig { get; }

        public TableauApiEndpointConfiguration SourceEndpointConfig { get; }

        protected TableauApiSimulator DestinationApi { get; }

        protected TableauSiteConnectionConfiguration DestinationSiteConfig { get; }

        public TableauApiEndpointConfiguration DestinationEndpointConfig { get; }

        public ServerToServerSimulationTestBase(string sourceUrl = "https://source", string destinationUrl = "https://destination")
        {
            SourceApi = RegisterApiSimulator(sourceUrl, Create<UsersResponse.UserType>());
            SourceSiteConfig = BuildSiteConnectionConfiguration(SourceApi);
            SourceEndpointConfig = new(SourceSiteConfig);

            DestinationApi = RegisterApiSimulator(destinationUrl, Create<UsersResponse.UserType>());
            DestinationSiteConfig = BuildSiteConnectionConfiguration(DestinationApi);
            DestinationEndpointConfig = new(DestinationSiteConfig);
        }
    }
}
