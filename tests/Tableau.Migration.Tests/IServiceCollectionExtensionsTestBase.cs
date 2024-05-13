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
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Tableau.Migration.Tests
{
    public abstract class IServiceCollectionExtensionsTestBase : AutoFixtureTestBase
    {
        protected readonly IServiceCollection Services = new ServiceCollection();

        protected IServiceProvider ServiceProvider => Services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateScopes = true,
            ValidateOnBuild = true
        });

        public IServiceCollectionExtensionsTestBase()
        {
            ConfigureServices(Services);
        }

        protected async Task AssertServiceAsync<TService, TImplementation>(ServiceLifetime? lifetime = null)
            where TService : class
            => await AssertServiceAsync<TService>(typeof(TImplementation), lifetime);

        protected async Task AssertServiceAsync<TService>(Type implementationType, ServiceLifetime? lifetime = null)
            where TService : class
        {
            await using var scope = ServiceProvider.CreateAsyncScope();
            AssertService<TService>(scope, implementationType, lifetime);
        }

        protected void AssertService<TService, TImplementation>(IServiceScope scope, ServiceLifetime? lifetime = null)
            where TService : class
            => AssertService<TService>(scope, typeof(TImplementation), lifetime);

        protected void AssertService<TService>(IServiceScope scope, Type implementationType, ServiceLifetime? lifetime = null)
            where TService : class
        {
            var implementation = scope.ServiceProvider.GetService<TService>();

            Assert.NotNull(implementation);
            Assert.IsType(implementationType, implementation);

            if (lifetime is not null)
            {
                var services = Services.Where(s =>
                    s.ServiceType == typeof(TService) &&
                    s.ImplementationType == implementationType);

                foreach (var service in services)
                    Assert.Equal(lifetime, service.Lifetime);
            }
        }

        protected async Task AssertServiceAsync<TService>(ServiceLifetime? lifetime = null)
            where TService : class
            => await AssertServiceAsync<TService, TService>(lifetime);

        protected void AssertService<TService>(IServiceScope scope, ServiceLifetime? lifetime = null)
            where TService : class
            => AssertService<TService, TService>(scope, lifetime);

        protected abstract void ConfigureServices(IServiceCollection services);

        [Fact]
        public void Builds_and_Validates()
        {
            _ = ServiceProvider; // Accessing the provider will build and validate it.
        }
    }
}
