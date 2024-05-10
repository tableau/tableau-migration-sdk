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
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Engine.Endpoints;

namespace Tableau.Migration.Tests.Unit.Engine.Endpoints
{
    public class TableauApiEndpointTestBase<TEndpoint> : AutoFixtureTestBase, IDisposable
        where TEndpoint : TableauApiEndpointBase
    {
        protected readonly Mock<IApiClient> MockServerApi;
        protected readonly Mock<ISitesApiClient> MockSiteApi;

        protected readonly ServiceProvider MigrationServices;

        public TableauApiEndpointTestBase()
        {
            MockSiteApi = Create<Mock<ISitesApiClient>>();
            MockServerApi = Freeze<Mock<IApiClient>>();
            MockServerApi.Setup(x => x.SignInAsync(Cancel))
                .ReturnsAsync(() => AsyncDisposableResult<ISitesApiClient>.Succeeded(MockSiteApi.Object));

            var migrationServiceCollection = new ServiceCollection()
                .AddMigrationApiClient();
            migrationServiceCollection.AddTransient(p => MockServerApi.Object);

            MigrationServices = migrationServiceCollection.BuildServiceProvider();
        }

        public void Dispose()
        {
            MigrationServices.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
