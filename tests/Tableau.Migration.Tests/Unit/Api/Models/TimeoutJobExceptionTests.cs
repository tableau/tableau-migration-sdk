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
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Models
{
    public class TimeoutJobExceptionTests
    {
        public class Ctor : AutoFixtureTestBase
        {
            [Fact]
            public void Initializes_with_job()
            {
                var job = Create<IJob>();
                var mockLocalizer = new MockSharedResourcesLocalizer();

                var ex = new TimeoutJobException(job, mockLocalizer.Object);

                Assert.Same(job, ex.Job);
            }

            [Fact]
            public void Initializes_without_job()
            {
                var mockLocalizer = new MockSharedResourcesLocalizer();

                var ex = new TimeoutJobException(null, mockLocalizer.Object);

                Assert.Null(ex.Job);
            }
        }
    }
}
