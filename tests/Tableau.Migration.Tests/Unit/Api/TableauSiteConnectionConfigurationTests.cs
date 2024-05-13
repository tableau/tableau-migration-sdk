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
using Tableau.Migration.Api;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api
{
    public class TableauSiteConnectionConfigurationTests
    {
        public class Empty
        {
            [Fact]
            public void HasEmptyValues()
            {
                var empty = TableauSiteConnectionConfiguration.Empty;

                Assert.Equal("https://localhost/", empty.ServerUrl.ToString());
                Assert.Empty(empty.SiteContentUrl);
                Assert.Empty(empty.AccessTokenName);
                Assert.Empty(empty.AccessToken);
            }
        }

        public class Validate
        {
            [Fact]
            public void Valid()
            {
                var c = new TableauSiteConnectionConfiguration(new Uri("https://localhost"), "site", "tokenName", "token");

                var vr = c.Validate();

                vr.AssertSuccess();
            }

            [Fact]
            public void InvalidAccessTokenName()
            {
                var c = new TableauSiteConnectionConfiguration(new Uri("https://localhost"), "site", "", "token");

                var vr = c.Validate();

                vr.AssertFailure();
                Assert.Single(vr.Errors);
            }

            [Fact]
            public void InvalidAccessToken()
            {
                var c = new TableauSiteConnectionConfiguration(new Uri("https://localhost"), "site", "tokenName", "");

                var vr = c.Validate();

                vr.AssertFailure();
                Assert.Single(vr.Errors);
            }
        }
    }
}
