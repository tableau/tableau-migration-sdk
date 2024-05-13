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

using System.Net.Http;
using Xunit;

namespace Tableau.Migration.Tests
{
    internal static class HttpContentExtensions
    {
        public static void AssertSingleHeaderValue(this HttpContent content, string headerKey, string headerValue)
        {
            content.AssertHeaderExists(headerKey);

            var actualHeaderValue = Assert.Single(content.Headers.GetValues(headerKey));

            Assert.Equal(headerValue, actualHeaderValue);
        }

        public static void AssertHeaderExists(this HttpContent content, string headerKey)
        {
            Assert.True(content.Headers.Contains(headerKey));
        }

        public static void AssertHeaderDoesNotExist(this HttpContent content, string headerKey)
        {
            Assert.False(content.Headers.Contains(headerKey));
        }
    }
}
