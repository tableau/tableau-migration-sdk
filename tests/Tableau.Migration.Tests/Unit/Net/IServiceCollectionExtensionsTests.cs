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

using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tableau.Migration.Api;
using Tableau.Migration.Config;
using Tableau.Migration.Net;
using Tableau.Migration.Net.Handlers;
using Tableau.Migration.Net.Resilience;
using Tableau.Migration.Net.Rest;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net
{
    public class IServiceCollectionExtensionsTests
    {
        public class AddHttpServices : IServiceCollectionExtensionsTestBase
        {
            protected override void ConfigureServices(IServiceCollection services)
                => services
                    .AddLocalization()
                    .AddSharedResourcesLocalization()
                    .AddHttpServices();

            [Fact]
            public async Task Registers_expected_services()
            {
                // Please, respect the same order on the method IServiceCollectionExtensions.AddHttpServices
                await AssertServiceAsync<ITableauSerializer, TableauSerializer>(ServiceLifetime.Singleton);

                await AssertServiceAsync<IMigrationSdk, MigrationSdk>(ServiceLifetime.Singleton);
                await AssertServiceAsync<IConfigReader, ConfigReader>(ServiceLifetime.Singleton);
                await AssertServiceAsync<IUserAgentProvider, UserAgentProvider>(ServiceLifetime.Singleton);
                await AssertServiceAsync<IHttpContentSerializer, HttpContentSerializer>(ServiceLifetime.Singleton);
                await AssertServiceAsync<INetworkTraceRedactor, NetworkTraceRedactor>(ServiceLifetime.Singleton);
                await AssertServiceAsync<INetworkTraceLogger, NetworkTraceLogger>(ServiceLifetime.Transient);
                await AssertServiceAsync<UserAgentHttpMessageHandler>(ServiceLifetime.Transient);
                await AssertServiceAsync<AuthenticationHandler>(ServiceLifetime.Transient);
                await AssertServiceAsync<LoggingHandler>(ServiceLifetime.Transient);
                await AssertServiceAsync<IHttpClient, DefaultHttpClient>(ServiceLifetime.Transient);

                var defaultHttpClientFactoryType = Migration.Net.IServiceCollectionExtensions.GetDefaultHttpClientFactoryType();
                await AssertServiceAsync<IHttpClientFactory>(defaultHttpClientFactoryType, ServiceLifetime.Scoped);
                await AssertServiceAsync<IHttpMessageHandlerFactory>(defaultHttpClientFactoryType, ServiceLifetime.Scoped);

                await AssertServiceAsync<RequestBuilderFactoryInput>(ServiceLifetime.Scoped);
                await AssertServiceAsync<IRequestBuilderFactoryInput, RequestBuilderFactoryInput>(ServiceLifetime.Scoped);
                await AssertServiceAsync<IRequestBuilderFactoryInputInitializer, RequestBuilderFactoryInput>(ServiceLifetime.Scoped);

                await AssertServiceAsync<IAuthenticationTokenProvider, AuthenticationTokenProvider>(ServiceLifetime.Scoped);
                await AssertServiceAsync<ITableauServerVersionProvider, TableauServerVersionProvider>(ServiceLifetime.Scoped);
                await AssertServiceAsync<IServerSessionProvider, ServerSessionProvider>(ServiceLifetime.Scoped);
                await AssertServiceAsync<IHttpRequestBuilderFactory, HttpRequestBuilderFactory>(ServiceLifetime.Scoped);
                await AssertServiceAsync<IRestRequestBuilderFactory, RestRequestBuilderFactory>(ServiceLifetime.Scoped);

                var strategyBuilders = ServiceProvider.GetServices<IResilienceStrategyBuilder>();

                Assert.NotNull(strategyBuilders);
                Assert.Equal(5, strategyBuilders.Count());
                Assert.Collection(strategyBuilders,
                    b => Assert.IsType<RetryStrategyBuilder>(b),
                    b => Assert.IsType<MaxConcurrencyStrategyBuilder>(b),
                    b => Assert.IsType<ServerThrottleStrategyBuilder>(b),
                    b => Assert.IsType<ClientThrottleStrategyBuilder>(b),
                    b => Assert.IsType<RequestTimeoutStrategyBuilder>(b));
            }

            [Fact]
            public async Task ServiceProvider_GetTwoIHttpClient_ReturnsNotSameObject()
            {
                await using var scope = ServiceProvider.CreateAsyncScope();

                // Act
                var firstClient = scope.ServiceProvider.GetRequiredService<IHttpClient>();
                var secondClient = scope.ServiceProvider.GetRequiredService<IHttpClient>();

                // Assert
                Assert.NotNull(firstClient);
                Assert.NotSame(firstClient, secondClient);

                var innerHttpClient1 = firstClient.GetInnerClient();
                var innerHttpClient2 = secondClient.GetInnerClient();

                Assert.NotSame(innerHttpClient1, innerHttpClient2);
            }

            [Fact]
            public void Uses_existing_ITableauSerializer()
            {
                var existingSerializer = new TableauSerializer();

                Services.AddSingleton<ITableauSerializer>(existingSerializer);

                var serializer = ServiceProvider.GetService<ITableauSerializer>();

                Assert.NotNull(serializer);
                Assert.IsType<TableauSerializer>(serializer);
                Assert.Same(existingSerializer, serializer);
            }

            public class ScopedService
            { }

            public class ScopedHandler : DelegatingHandler
            {
                public readonly ScopedService Service;

                public ScopedHandler(ScopedService service)
                {
                    Service = service;
                }
            }

            [Fact]
            public async Task Uses_scoped_services_in_handlers()
            {
                var services = new ServiceCollection()
                    .AddScoped<ScopedService>()
                    .AddTransient<ScopedHandler>();

                services
                    .AddLocalization()
                    .AddSharedResourcesLocalization()
                    .AddHttpServices();

                services.AddHttpClient(nameof(DefaultHttpClient))
                    .AddHttpMessageHandler<ScopedHandler>();

                var provider = services.BuildServiceProvider();

                // First scope
                await using var scope1 = provider.CreateAsyncScope();

                var scope1Client1 = scope1.ServiceProvider.GetRequiredService<IHttpClient>();
                var scope1Client2 = scope1.ServiceProvider.GetRequiredService<IHttpClient>();

                var scope1InnerClient1 = scope1Client1.GetInnerClient();
                var scope1InnerClient2 = scope1Client2.GetInnerClient();

                var scope1Client1Handlers = scope1InnerClient1.GetMessageHandler().Flatten();
                var scope1Client2Handlers = scope1InnerClient2.GetMessageHandler().Flatten();

                var scope1InnerClient1Handler = Assert.Single(scope1Client1Handlers.OfType<ScopedHandler>());
                var scope1InnerClient2Handler = Assert.Single(scope1Client2Handlers.OfType<ScopedHandler>());

                Assert.Same(scope1InnerClient1Handler.Service, scope1InnerClient2Handler.Service);

                // Second scope
                await using var scope2 = provider.CreateAsyncScope();

                var scope2Client1 = scope2.ServiceProvider.GetRequiredService<IHttpClient>();
                var scope2Client2 = scope2.ServiceProvider.GetRequiredService<IHttpClient>();

                var scope2InnerClient1 = scope2Client1.GetInnerClient();
                var scope2InnerClient2 = scope2Client2.GetInnerClient();

                var scope2Client1Handlers = scope2InnerClient1.GetMessageHandler().Flatten();
                var scope2Client2Handlers = scope2InnerClient2.GetMessageHandler().Flatten();

                var scope2InnerClient1Handler = Assert.Single(scope2Client1Handlers.OfType<ScopedHandler>());
                var scope2InnerClient2Handler = Assert.Single(scope2Client2Handlers.OfType<ScopedHandler>());

                Assert.Same(scope2InnerClient1Handler.Service, scope2InnerClient2Handler.Service);

                Assert.NotSame(scope1InnerClient1Handler.Service, scope2InnerClient1Handler.Service);
            }
        }

        public class AddScopedHttpClient : IServiceCollectionExtensionsTestBase
        {
            protected override void ConfigureServices(IServiceCollection services) => services.AddScopedHttpClient(Create<string>());

            [Fact]
            public async Task Registers_scoped_client_factories()
            {
                await using var scope1 = ServiceProvider.CreateAsyncScope();
                var scope1ClientFactory1 = scope1.ServiceProvider.GetRequiredService<IHttpClientFactory>();
                var scope1ClientFactory2 = scope1.ServiceProvider.GetRequiredService<IHttpClientFactory>();

                Assert.Same(scope1ClientFactory1, scope1ClientFactory2);

                await using var scope2 = ServiceProvider.CreateAsyncScope();
                var scope2ClientFactory1 = scope2.ServiceProvider.GetRequiredService<IHttpClientFactory>();
                var scope2ClientFactory2 = scope2.ServiceProvider.GetRequiredService<IHttpClientFactory>();

                Assert.Same(scope2ClientFactory1, scope2ClientFactory2);

                Assert.NotSame(scope1ClientFactory1, scope2ClientFactory1);
            }

            [Fact]
            public async Task Registers_scoped_handler_factories()
            {
                await using var scope1 = ServiceProvider.CreateAsyncScope();
                var scope1HandlerFactory1 = scope1.ServiceProvider.GetRequiredService<IHttpMessageHandlerFactory>();
                var scope1HandlerFactory2 = scope1.ServiceProvider.GetRequiredService<IHttpMessageHandlerFactory>();

                Assert.Same(scope1HandlerFactory1, scope1HandlerFactory2);

                await using var scope2 = ServiceProvider.CreateAsyncScope();
                var scope2HandlerFactory1 = scope2.ServiceProvider.GetRequiredService<IHttpMessageHandlerFactory>();
                var scope2HandlerFactory2 = scope2.ServiceProvider.GetRequiredService<IHttpMessageHandlerFactory>();

                Assert.Same(scope2HandlerFactory1, scope2HandlerFactory2);

                Assert.NotSame(scope1HandlerFactory1, scope2HandlerFactory1);
            }

            [Fact]
            public async Task Client_factory_and_handler_factory_are_same_instance()
            {
                await using var scope = ServiceProvider.CreateAsyncScope();
                var clientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
                var handlerFactory = scope.ServiceProvider.GetRequiredService<IHttpMessageHandlerFactory>();

                Assert.Same(clientFactory, handlerFactory);
            }
        }
    }
}
