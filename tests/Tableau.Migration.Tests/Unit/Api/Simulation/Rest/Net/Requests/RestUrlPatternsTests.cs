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

using Tableau.Migration.Api.Simulation.Rest.Net.Requests;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Simulation.Rest.Net.Requests
{
    public class RestUrlPatternsTests
    {
        [Theory]
        [InlineData("sites", "/api/9.9/sites")]
        [InlineData("sites", "/api/9.9/sites/")]
        [InlineData("sites", "/api/9.99/sites")]
        [InlineData("sites", "/api/9.99/sites/")]
        [InlineData("sites", "/api/exp/sites/", true)]
        public void RestApiUrl(string suffix, string input, bool experimental = false)
        {
            Assert.Matches(RestUrlPatterns.RestApiUrl(suffix, useExperimental: experimental), input);
            return;
        }

        [Theory]
        [InlineData("sites", "/api/9.9/sites/14904aef-a798-4c7f-b178-2ad224a09b0e")]
        [InlineData("sites", "/api/9.9/sites/14904aef-a798-4c7f-b178-2ad224a09b0e/")]
        [InlineData("sites", "/api/9.99/sites/14904aef-a798-4c7f-b178-2ad224a09b0e")]
        [InlineData("sites", "/api/9.99/sites/14904aef-a798-4c7f-b178-2ad224a09b0e/")]
        public void EntityUrl(string suffix, string input)
        {
            Assert.Matches(RestUrlPatterns.EntityUrl(suffix), input);
        }

        [Theory]
        [InlineData("workbooks", "/api/9.9/sites/14904aef-a798-4c7f-b178-2ad224a09b0e/workbooks")]
        [InlineData("workbooks", "/api/9.9/sites/14904aef-a798-4c7f-b178-2ad224a09b0e/workbooks/")]
        [InlineData("workbooks", "/api/9.99/sites/14904aef-a798-4c7f-b178-2ad224a09b0e/workbooks")]
        [InlineData("workbooks", "/api/9.99/sites/14904aef-a798-4c7f-b178-2ad224a09b0e/workbooks/")]
        [InlineData("customviews", "/api/exp/sites/14904aef-a798-4c7f-b178-2ad224a09b0e/customviews", true)]
        [InlineData("customviews", "/api/exp/sites/14904aef-a798-4c7f-b178-2ad224a09b0e/customviews/",true)]
        public void SiteUrl(string suffix, string input, bool experimental = false)
        {
            Assert.Matches(RestUrlPatterns.SiteUrl(suffix, useExperimental: experimental), input);
        }

        [Theory]
        [InlineData("workbooks", "/api/9.9/sites/14904aef-a798-4c7f-b178-2ad224a09b0e/workbooks/63c3dca3-6a85-4437-815a-5392b239d5b3")]
        [InlineData("workbooks", "/api/9.9/sites/14904aef-a798-4c7f-b178-2ad224a09b0e/workbooks/63c3dca3-6a85-4437-815a-5392b239d5b3/")]
        [InlineData("workbooks", "/api/9.99/sites/14904aef-a798-4c7f-b178-2ad224a09b0e/workbooks/63c3dca3-6a85-4437-815a-5392b239d5b3")]
        [InlineData("workbooks", "/api/9.99/sites/14904aef-a798-4c7f-b178-2ad224a09b0e/workbooks/63c3dca3-6a85-4437-815a-5392b239d5b3/")]
        [InlineData("customviews", "/api/exp/sites/14904aef-a798-4c7f-b178-2ad224a09b0e/customviews/63c3dca3-6a85-4437-815a-5392b239d5b3/", true)]
        [InlineData("customviews", "/api/exp/sites/14904aef-a798-4c7f-b178-2ad224a09b0e/customviews/63c3dca3-6a85-4437-815a-5392b239d5b3", true)]
        public void SiteEntityUrl(string suffix, string input, bool experimental = false)
        {
            Assert.Matches(RestUrlPatterns.SiteEntityUrl(suffix, useExperimental: experimental), input);
        }
    }
}
