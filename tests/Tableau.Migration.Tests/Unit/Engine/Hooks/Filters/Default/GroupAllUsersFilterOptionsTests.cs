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

using Tableau.Migration.Engine.Hooks.Filters.Default;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Filters.Default
{
    public class GroupAllUsersFilterOptionsTests
    {
        public abstract class GroupAllUsersFilterOptionsTest : AutoFixtureTestBase
        { }

        public class Ctor : GroupAllUsersFilterOptionsTest
        {
            [Fact]
            public void Initializes()
            {
                var options = new GroupAllUsersFilterOptions();

                Assert.NotNull(options.AllUsersGroupNames);
                Assert.Empty(options.AllUsersGroupNames);
            }
        }

        public class AllUsersGroupNames : GroupAllUsersFilterOptionsTest
        {
            [Fact]
            public void Gets_values()
            {
                var options = new GroupAllUsersFilterOptions();

                var groupName = Create<string>();

                options.AllUsersGroupNames.Add(groupName);

                Assert.Equal(groupName, Assert.Single(options.AllUsersGroupNames));
            }
        }
    }
}
