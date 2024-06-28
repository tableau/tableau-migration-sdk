//
//  Copyright (c) 2024, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the "License") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

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
        /// <param name="isTableauServer">Indicates whether the current Tableau Data is for Tableau Server (true) or Tableau Cloud (false).</param>
        /// <param name="defaultSignedInUser">Default signed in user. If null, no user will be created or signed in.</param>
        /// <param name="defaultDomain">The default domain of the site.</param>
        protected TableauApiSimulator RegisterApiSimulator(
            Uri serverUri,
            bool isTableauServer,
            UsersResponse.UserType? defaultSignedInUser, 
            string defaultDomain = Constants.LocalDomain)
        {
            var simulator = new TableauApiSimulator(
                serverUri, 
                Serializer, 
                isTableauServer, 
                defaultSignedInUser, 
                defaultDomain);
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
        /// Registers a Tableau Server <see cref="TableauApiSimulator"/> for HTTP request/response mocking.
        /// </summary>
        /// <param name="serverUrl">The URL of the server.</param>
        /// <param name="defaultSignedInUser">Default signed in user. If null, no user will be created or signed in.</param>
        protected TableauApiSimulator RegisterTableauServerApiSimulator(
            string serverUrl, 
            UsersResponse.UserType? defaultSignedInUser)
            => RegisterApiSimulator(
                new Uri(serverUrl),
                true,
                defaultSignedInUser);

        /// <summary>
        /// Registers a Tableau Cloud <see cref="TableauApiSimulator"/> for HTTP request/response mocking.
        /// </summary>
        /// <param name="defaultSignedInUser">Default signed in user. If null, no user will be created or signed in.</param>
        protected TableauApiSimulator RegisterTableauCloudApiSimulator(
            string podUrl, 
            UsersResponse.UserType? defaultSignedInUser)
            => RegisterApiSimulator(
                new Uri(podUrl),
                false,
                defaultSignedInUser, 
                Constants.TableauIdWithMfaDomain);

        public async ValueTask DisposeAsync()
        {
            await ServiceProvider.DisposeAsync();
            GC.SuppressFinalize(this);
        }
    }
}
