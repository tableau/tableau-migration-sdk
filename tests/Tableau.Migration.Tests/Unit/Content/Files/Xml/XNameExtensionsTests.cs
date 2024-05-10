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
using System.Xml.Linq;
using Tableau.Migration.Content.Files.Xml;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content.Files.Xml
{
    public class XNameExtensionsTests
    {
        public class MatchFeatureFlagName
        {
            [Theory]
            [InlineData("{ns1}el", "{NS1}el", true)] //namespace case insensitive match
            [InlineData("{ns1}_.fcp.FeatureFlag.true...el", "{NS1}el", true)] //namespace case insensitive FFS differences match
            [InlineData("{ns1}el", "{ns2}el", false)] //namespace not matched
            [InlineData("{ns1}_.fcp.FeatureFlag.true...el", "{ns2}el", false)] //namespace not matched FFS differences
            [InlineData("el", "EL", true)] //simple case insensitive match
            [InlineData("_.fcp.FeatureFlag.true...el", "EL", true)] //FFS prefix case insensitive match
            [InlineData("el", "fel", false)] //simple not matched
            [InlineData("_.fcp.FeatureFlag.true...el", "fel", false)] //FFS prefix local name not matched
            public void Matches(string a, string b, bool expected)
            {
                var xa = XName.Get(a);
                var xb = XName.Get(b);
                var result = xa.MatchFeatureFlagName(xb);

                Assert.Equal(expected, result);
            }

            [Fact]
            public void ValidateNonFfsMatchName()
            {
                XName xa = XName.Get("test");
                XName xb = XName.Get("_.fcp.FeatureFlag.true...test");

                Assert.Throws<ArgumentException>(() => xa.MatchFeatureFlagName(xb));
            }
        }
    }
}
