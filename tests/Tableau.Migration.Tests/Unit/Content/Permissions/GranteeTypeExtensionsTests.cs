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

using System;
using Tableau.Migration.Content.Permissions;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content.Permissions
{
    public sealed class GranteeTypeExtensionsTests
    {
        public sealed class ToUrlSegment : AutoFixtureTestBase
        {
            [Theory]
            [EnumData<GranteeType>]
            public void SupportsAllGranteeTypes(GranteeType t)
            {
                var u = t.ToUrlSegment();
                Assert.NotEmpty(u);
            }

            [Fact]
            public void ThrowsOnUnsupportedGranteeType()
            {
                Assert.Throws<NotSupportedException>(() => ((GranteeType)int.MaxValue).ToUrlSegment());
            }
        }
    }
}
