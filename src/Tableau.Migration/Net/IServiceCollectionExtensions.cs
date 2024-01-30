// Copyright (c) 2023, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the ""License"") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an ""AS IS"" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;
using Tableau.Migration.Api;
using Tableau.Migration.Config;
using Tableau.Migration.Net.Handlers;
using Tableau.Migration.Net.Policies;
using Tableau.Migration.Net.Rest;
using Tableau.Migration.Net.Simulation;

namespace Tableau.Migration.Net
{
    /// <summary>
    /// Static class containing Http Services extension methods for <see cref="IServiceCollection"/> objects.
    /// </summary>
    internal static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Add all required services to use the <see cref="IHttpClient"/>
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        internal static IServiceCollection AddHttpServices(
            this IServiceCollection services)
        {
            services.AddSharedResourcesLocalization();

            services.TryAddSingleton<ITableauSerializer>(TableauSerializer.Instance);
            services.AddSingleton<IResponseSimulatorProvider, NullResponseSimulatorProvider>();

            services
                .AddSingleton<IUserAgentSuffixProvider, UserAgentSuffixProvider>()
                .AddSingleton<IMigrationSdk, MigrationSdk>()
                .AddSingleton<IConfigReader, ConfigReader>()
                .AddSingleton<IHttpContentSerializer, HttpContentSerializer>()
                .AddSingleton<INetworkTraceRedactor, NetworkTraceRedactor>()
                .AddTransient<INetworkTraceLogger, NetworkTraceLogger>()
                .AddScoped<HttpPolicyWrapBuilder>()
                .AddScoped<IHttpPolicyWrapBuilder, SimpleCachedHttpPolicyWrapBuilder>()
                // All Handlers must be transients
                // Their lifetime will be managed by the IHttpClientFactory
                .AddTransient(provider =>
                {
                    var policyBuilder = provider.GetRequiredService<IHttpPolicyWrapBuilder>();

                    return new PolicyHttpMessageHandler(
                        httpRequest => policyBuilder.GetRequestPolicies(
                            httpRequest));
                })
                .AddTransient<UserAgentHttpMessageHandler>()
                .AddTransient<AuthenticationHandler>()
                .AddTransient<LoggingHandler>()
                .AddTransient<SimulationHttpHandler>()
                // Keeping a single HttpClient instance alive for a long duration is a common pattern used before the inception
                // of IHttpClientFactory. This pattern becomes unnecessary after migrating to IHttpClientFactory.
                // Source: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-7.0#httpclient-and-lifetime-management
                .AddTransient<IHttpClient>(provider =>
                {
                    var clientFactory = provider.GetRequiredService<IHttpClientFactory>();

                    return new DefaultHttpClient(
                        clientFactory.CreateClient(
                            nameof(DefaultHttpClient)),
                        provider.GetRequiredService<IHttpContentSerializer>());
                })
                .AddScoped<RetryPolicyBuilder>()
                .AddScoped<MaxConcurrencyPolicyBuilder>()
                .AddScoped<ClientThrottlePolicyBuilder>()
                .AddScoped<ServerThrottlePolicyBuilder>()
                .AddScoped<RequestTimeoutPolicyBuilder>()
                .AddScoped<SimpleCachedRetryPolicyBuilder>()
                .AddScoped<SimpleCachedMaxConcurrencyPolicyBuilder>()
                .AddScoped<SimpleCachedClientThrottlePolicyBuilder>()
                .AddScoped<SimpleCachedServerThrottlePolicyBuilder>()
                .AddScoped<SimpleCachedRequestTimeoutPolicyBuilder>()
                // Policies builders. The order here is important for the dependency injection
                // It injects all policies as an enumerator, on the same order they are registered here.
                .AddScoped<IHttpPolicyBuilder, SimpleCachedRetryPolicyBuilder>()
                .AddScoped<IHttpPolicyBuilder, SimpleCachedMaxConcurrencyPolicyBuilder>()
                .AddScoped<IHttpPolicyBuilder, SimpleCachedServerThrottlePolicyBuilder>()
                .AddScoped<IHttpPolicyBuilder, SimpleCachedClientThrottlePolicyBuilder>()
                .AddScoped<IHttpPolicyBuilder, SimpleCachedRequestTimeoutPolicyBuilder>();

            services
                // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-7.0#httpclient-and-lifetime-management
                // The default handler lifetime is two minutes. The default value can be overridden on a per named client basis
                .AddScopedHttpClient(nameof(DefaultHttpClient))
                // From microsoft documentation:
                // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-7.0#outgoing-request-middleware
                // Multiple handlers can be registered in the order that they should execute.
                // Each handler wraps the next handler until the final HttpClientHandler executes the request.
                .AddHttpMessageHandler<UserAgentHttpMessageHandler>()
                .AddHttpMessageHandler<PolicyHttpMessageHandler>()
                .AddHttpMessageHandler<AuthenticationHandler>()
                .AddHttpMessageHandler<LoggingHandler>()
                .AddHttpMessageHandler<SimulationHttpHandler>(); //Must be last for simulation to function.

            //Bootstrap and scope state tracking services.
            services
                .AddScoped<RequestBuilderFactoryInput>()
                .AddScoped<IRequestBuilderFactoryInput>(p => p.GetRequiredService<RequestBuilderFactoryInput>())
                .AddScoped<IRequestBuilderFactoryInputInitializer>(p => p.GetRequiredService<RequestBuilderFactoryInput>());

            services
                .AddScoped<IAuthenticationTokenProvider, AuthenticationTokenProvider>()
                .AddScoped<ITableauServerVersionProvider, TableauServerVersionProvider>()
                .AddScoped<IServerSessionProvider, ServerSessionProvider>()
                .AddScoped<IHttpRequestBuilderFactory, HttpRequestBuilderFactory>()
                .AddScoped<IRestRequestBuilderFactory, RestRequestBuilderFactory>()
                .AddScoped<IHttpStreamProcessor, HttpStreamProcessor>();

            return services;
        }

        /// <summary>
        /// Custom implementation of <see cref="HttpClientFactoryServiceCollectionExtensions.AddHttpClient(IServiceCollection, string)"/>
        /// to register the HTTP client factory as scoped instead of singleton so application DI scopes are respected. 
        /// </summary>
        internal static IHttpClientBuilder AddScopedHttpClient(this IServiceCollection services, string name)
        {
            // Add default HTTP client/factory infrastructure
            var builder = services.AddHttpClient(name);

            var addClientFactory = false;
            var addHandlerFactory = false;

            // Remove registered HTTP client factory singleton instances
            for (var i = 0; i != builder.Services.Count; i++)
            {
                var descriptor = builder.Services[i];

                if (descriptor.ServiceType == typeof(IHttpClientFactory) && descriptor.Lifetime is ServiceLifetime.Singleton)
                {
                    builder.Services.Remove(descriptor);
                    i--;
                    addClientFactory = true;
                }
                else if (descriptor.ServiceType == typeof(IHttpMessageHandlerFactory) && descriptor.Lifetime is ServiceLifetime.Singleton)
                {
                    builder.Services.Remove(descriptor);
                    i--;
                    addHandlerFactory = true;
                }

                if (addClientFactory && addHandlerFactory) // Bail early if we've found what we need.
                    break;
            }

            var defaultHttpClientFactoryType = GetDefaultHttpClientFactoryType();

            // Re-register the HTTP client factory as scoped.
            if (addClientFactory)
            {
                builder.Services.AddScoped(CreateClientFactoryInstance<IHttpClientFactory>);
            }

            if (addHandlerFactory)
            {
                builder.Services.AddScoped(provider => (IHttpMessageHandlerFactory)provider.GetRequiredService<IHttpClientFactory>());
            }

            // Override default implicit scope creation so our scopes are used.
            builder.Services.Configure<HttpClientFactoryOptions>(name, options => options.SuppressHandlerScope = true);

            return builder;

            TService CreateClientFactoryInstance<TService>(IServiceProvider serviceProvider)
                where TService : class
            {
                return (ActivatorUtilities.CreateInstance(
                        serviceProvider,
                        defaultHttpClientFactoryType,
                        serviceProvider,
                        serviceProvider.GetRequiredService<IServiceScopeFactory>(),
                        serviceProvider.GetRequiredService<IOptionsMonitor<HttpClientFactoryOptions>>(),
                        serviceProvider.GetRequiredService<IEnumerable<IHttpMessageHandlerBuilderFilter>>())
                    as TService)!;
            }
        }

        /// <summary>
        /// Gets the internal default HTTP client factory type, or throws if it can't be found.
        /// </summary>
        internal static Type GetDefaultHttpClientFactoryType()
        {
            var httpAssembly = typeof(HttpClientFactoryOptions).Assembly;
            var defaultHttpClientFactoryTypeName = "Microsoft.Extensions.Http.DefaultHttpClientFactory";

            return httpAssembly.GetType(defaultHttpClientFactoryTypeName)
                ?? throw new InvalidOperationException($"Could not find type {defaultHttpClientFactoryTypeName} in assembly {httpAssembly.FullName}.");
        }
    }
}
