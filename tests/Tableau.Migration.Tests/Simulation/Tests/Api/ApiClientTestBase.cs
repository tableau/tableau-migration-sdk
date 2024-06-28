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

using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Net;

namespace Tableau.Migration.Tests.Simulation.Tests.Api
{
    public abstract class ApiClientTestBase : SingleServerSimulationTestBase
    {
        internal readonly ApiClient ApiClient;

        public ApiClientTestBase(bool isCloud = false)
            : base(isCloud)
        {
            var apiInputInitializer = ServiceProvider.GetRequiredService<IApiClientInputInitializer>();
            apiInputInitializer.Initialize(SiteConfig);

            var requestFactoryInputInitializer = ServiceProvider.GetRequiredService<IRequestBuilderFactoryInputInitializer>();
            requestFactoryInputInitializer.Initialize(Api.ServerUrl);

            ApiClient = (ServiceProvider.GetRequiredService<IApiClient>() as ApiClient)!;
        }

        protected override IServiceCollection ConfigureServices(IServiceCollection services)
        {
            return base.ConfigureServices(services)
                .AddTableauMigrationSdk();
        }

        internal async Task<SitesApiClient> GetSitesClientAsync(CancellationToken cancel)
        {
            var result = await ApiClient.SignInAsync(cancel);

            result.AssertSuccess();

            return (result.Value as SitesApiClient)!;
        }
    }

    public abstract class ApiClientTestBase<TApiClient, TContent> : ApiClientTestBase
        where TApiClient : IContentApiClient
        where TContent : IRestIdentifiable, new()
    {
        protected string UrlPrefix => RestUrlPrefixes.GetUrlPrefix(typeof(TApiClient));

        protected TApiClient GetApiClient() => ServiceProvider.GetRequiredService<TApiClient>();

        protected virtual TContent CreateContentItem() => Create<TContent>();

        protected IImmutableList<TContent> CreateContentItems(int count)
           => Enumerable.Range(0, count).Select(_ => CreateContentItem()).ToImmutableList();
    }
}
