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

using System;
using Tableau.Migration.Api;
using Tableau.Migration.Engine.Endpoints;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Endpoints
{
    public class TableauApiEndpointConfigurationTests
    {
        public class Empty
        {
            [Fact]
            public void HasEmptySiteConnectionConfig()
            {
                var empty = TableauApiEndpointConfiguration.Empty;
                Assert.Equal(TableauSiteConnectionConfiguration.Empty, empty.SiteConnectionConfiguration);
            }
        }

        public class Validate
        {
            [Fact]
            public void ValidSiteConnectionConfig()
            {
                var c = new TableauApiEndpointConfiguration(new(new Uri("https://localhost"), "site", "tokenName", "token"));

                var vr = c.Validate();

                vr.AssertSuccess();
            }

            [Fact]
            public void InvalidSiteConnectionConfig()
            {
                var c = new TableauApiEndpointConfiguration(new(new Uri("https://localhost"), "", "tokenName", ""));

                var vr = c.Validate();

                vr.AssertFailure();
                Assert.Single(vr.Errors);
            }
        }
    }
}
