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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit.Sdk;

namespace Tableau.Migration.Tests
{
    /// <summary>
    /// Provides culture name test data to theory tests.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CultureNameTestDataAttribute : DataAttribute
    {
        private static readonly string[] _cultureNames = new string[2]
        {
            "en-US",
            "fi-FI" // has an alternate decimal number format
        };

        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            return _cultureNames
                .Select(x => new object[] { x });
        }
    }
}
