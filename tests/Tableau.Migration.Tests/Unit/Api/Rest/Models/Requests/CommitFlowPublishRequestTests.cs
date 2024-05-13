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

using Tableau.Migration.Api.Models;
using Tableau.Migration.Api.Rest.Models.Requests;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models.Requests
{
    public class CommitFlowPublishRequestTests
    {
        public abstract class CommitFlowPublishRequestTest : AutoFixtureTestBase
        { }

        public class Ctor : CommitFlowPublishRequestTest
        {
            [Fact]
            public void Initializes()
            {
                var options = Create<IPublishFlowOptions>();

                var request = new CommitFlowPublishRequest(options);

                Assert.NotNull(request.Flow);

                Assert.Equal(options.Name, request.Flow.Name);
                Assert.Equal(options.Description, request.Flow.Description);

                Assert.NotNull(request.Flow.Project);

                Assert.Equal(options.ProjectId, request.Flow.Project.Id);
            }
        }
    }
}
