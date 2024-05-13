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
using Tableau.Migration.Net.Rest;
using Xunit;


namespace Tableau.Migration.Tests.Unit.Net.Rest
{
    public class GuidExtensionsTests
    {
        public class GuidExtensionTest : AutoFixtureTestBase
        { }

        public class ToUrlSegment : GuidExtensionTest
        {
            [Fact]
            public void Uses_expected_format()
            {
                var guidValue = "e70e0c66-bd02-40e2-9f98-47511047b4a6";
                var guid = Guid.Parse(guidValue);

                Assert.Equal(guidValue, guid.ToUrlSegment());
            }
        }
    }
}
