//
//  Copyright (c) 2026, Salesforce, Inc.
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

using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Engine.Services;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Services
{
    public sealed class MigrationServicesTests
    {
        public sealed class SupportedPlanServices
        {
            [Fact]
            public void HasExpectedServices()
            {
                Assert.Contains(typeof(IDestinationContentReferenceFinder<>), MigrationServices.SupportedPlanServices);
                Assert.Contains(typeof(IMigrationContentLoader<>), MigrationServices.SupportedPlanServices);
                Assert.Contains(typeof(ISourceContentReferenceFinder<>), MigrationServices.SupportedPlanServices);
            }
        }
    }
}
