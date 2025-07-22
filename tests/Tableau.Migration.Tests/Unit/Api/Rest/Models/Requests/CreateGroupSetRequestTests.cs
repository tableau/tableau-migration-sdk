//
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

using Tableau.Migration.Api.Rest.Models.Requests;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models.Requests
{
    public sealed class CreateGroupSetRequestTests
    {
        public sealed class Ctor : AutoFixtureTestBase
        {
            [Fact]
            public void DefaultCtor()
            {
                var r = new CreateGroupSetRequest();

                Assert.Null(r.GroupSet);
            }

            [Fact]
            public void FullCtor()
            {
                var name = Create<string>();
                var r = new CreateGroupSetRequest(name);

                Assert.NotNull(r.GroupSet);
                Assert.Equal(name, r.GroupSet.Name);
            }
        }
    }
}
