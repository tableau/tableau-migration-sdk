using System;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Simulation;
using Tableau.Migration.Net;
using Tableau.Migration.Net.Simulation;

namespace Tableau.Migration.Tests.Simulation
{
    /// <summary>
    /// Base implementation for test classes that require simulation servers.
    /// </summary>
    public abstract class SimulationTestBase : AutoFixtureTestBase, IAsyncDisposable
    {
        protected readonly ServiceProvider ServiceProvider;

        protected readonly ITableauApiSimulatorCollection ApiSimulators;

        protected IHttpContentSerializer Serializer { get; }

        protected SimulationTestBase()
        {
            var services = new ServiceCollection();

            ConfigureServices(services);

            // Register simulation HTTP handling so mocked responses are handled.
            services
                .AddSingleton<ITableauApiSimulatorCollection, TableauApiSimulatorCollection>()
                .AddTransient<SimulationHttpHandler>();

            services
                .AddHttpClient(nameof(DefaultHttpClient))
                .AddHttpMessageHandler<SimulationHttpHandler>();

            ServiceProvider = services.BuildServiceProvider();

            Serializer = ServiceProvider.GetRequiredService<IHttpContentSerializer>();
            ApiSimulators = ServiceProvider.GetRequiredService<ITableauApiSimulatorCollection>();
        }

        /// <summary>
        /// Configures DI for the test class.
        /// </summary>
        protected virtual IServiceCollection ConfigureServices(IServiceCollection services) => services;

        /// <summary>
        /// Registers a <see cref="TableauApiSimulator"/> for HTTP request/response mocking.
        /// </summary>
        /// <param name="serverUri">The URI of the server.</param>
        /// /// <param name="defaultSignedInUser">Default signed in user. If null, no user will be created or signed in.</param>
        protected TableauApiSimulator RegisterApiSimulator(Uri serverUri,
            UsersResponse.UserType? defaultSignedInUser, string defaultDomain = Constants.LocalDomain)
        {
            var simulator = new TableauApiSimulator(serverUri, Serializer, defaultSignedInUser, defaultDomain);
            ApiSimulators.AddOrUpdate(simulator);
            return simulator;
        }

        protected TableauSiteConnectionConfiguration BuildSiteConnectionConfiguration(TableauApiSimulator simulator)
        {
            return AutoFixture
                .Build<TableauSiteConnectionConfiguration>()
                .With(c => c.ServerUrl, simulator.ServerUrl)
                .Create();
        }

        /// <summary>
        /// Registers a <see cref="TableauApiSimulator"/> for HTTP request/response mocking.
        /// </summary>
        /// <param name="serverUrl">The URL of the server.</param>
        /// <param name="defaultSignedInUser">Default signed in user. If null, no user will be created or signed in.</param>
        protected TableauApiSimulator RegisterApiSimulator(string serverUrl, UsersResponse.UserType? defaultSignedInUser)
            => RegisterApiSimulator(new Uri(serverUrl), defaultSignedInUser);

        /// <summary>
        /// Registers a Tableau Cloud <see cref="TableauApiSimulator"/> for HTTP request/response mocking.
        /// </summary>
        /// <param name="defaultSignedInUser">Default signed in user. If null, no user will be created or signed in.</param>
        protected TableauApiSimulator RegisterCloudApiSimulator(string podUrl, UsersResponse.UserType? defaultSignedInUser)
            => RegisterApiSimulator(new Uri(podUrl), defaultSignedInUser, Constants.TableauIdWithMfaDomain);

        public async ValueTask DisposeAsync()
        {
            await ServiceProvider.DisposeAsync();
            GC.SuppressFinalize(this);
        }
    }
}
