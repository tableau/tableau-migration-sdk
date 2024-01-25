using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Simulation;
using Tableau.Migration.Engine.Endpoints;

namespace Tableau.Migration.Tests.Simulation
{
    /// <summary>
    /// <see cref="SimulationTestBase"/> implementation for test classes that require a source and multiple destination servers (i.e. for migrations testing).
    /// </summary>
    public abstract class MultiDestinationSimulationTestBase : SimulationTestBase
    {
        protected TableauApiSimulator SourceApi { get; }

        protected TableauSiteConnectionConfiguration SourceSiteConfig { get; }

        public TableauApiEndpointConfiguration SourceEndpointConfig { get; }

        protected ImmutableArray<TableauApiSimulator> DestinationApis { get; }

        protected ImmutableArray<TableauSiteConnectionConfiguration> DestinationSiteConfigs { get; }

        protected ImmutableArray<TableauApiEndpointConfiguration> DestinationEndpointConfigs { get; }

        public MultiDestinationSimulationTestBase(string sourceUrl, IEnumerable<string> destinationUrls)
        {
            SourceApi = RegisterApiSimulator(sourceUrl, Create<UsersResponse.UserType>());
            SourceSiteConfig = BuildSiteConnectionConfiguration(SourceApi);
            SourceEndpointConfig = new(SourceSiteConfig);

            DestinationApis = destinationUrls
                .Select(u => RegisterApiSimulator(u, Create<UsersResponse.UserType>()))
                .ToImmutableArray();

            DestinationSiteConfigs = DestinationApis
                .Select(a => BuildSiteConnectionConfiguration(a))
                .ToImmutableArray();

            DestinationEndpointConfigs = DestinationSiteConfigs
                .Select(c => new TableauApiEndpointConfiguration(c))
                .ToImmutableArray();
        }
    }
}
