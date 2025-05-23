﻿//
//  Copyright (c) 2025, Salesforce, Inc.
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

using Tableau.Migration.Api.Models;
using Tableau.Migration.Api.Rest.Models.Requests;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models
{
    public class CommitDataSourcePublishRequestTests
    {
        public abstract class CommitDataSourcePublishRequestTest : AutoFixtureTestBase
        { }

        public class Ctor : CommitDataSourcePublishRequestTest
        {
            [Fact]
            public void Initializes()
            {
                var options = Create<IPublishDataSourceOptions>();

                var request = new CommitDataSourcePublishRequest(options);

                Assert.NotNull(request.DataSource);

                Assert.Equal(options.Name, request.DataSource.Name);
                Assert.Equal(options.Description, request.DataSource.Description);
                Assert.Equal(options.UseRemoteQueryAgent, request.DataSource.UseRemoteQueryAgent);
                Assert.Equal(options.EncryptExtracts, request.DataSource.EncryptExtracts);

                Assert.NotNull(request.DataSource.Project);

                Assert.Equal(options.ProjectId, request.DataSource.Project.Id);
            }
        }
    }
}
