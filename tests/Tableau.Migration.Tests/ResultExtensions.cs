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
using System.Collections.Generic;
using Xunit;

namespace Tableau.Migration.Tests
{
    internal static class ResultExtensions
    {
        public static void AssertSuccess(this IResult result)
        {
            Assert.Empty(result.Errors);
            Assert.True(result.Success);
        }

        public static void AssertFailure(this IResult result, IEnumerable<Exception>? errorsToMatch = null)
        {
            Assert.False(result.Success);
            Assert.NotEmpty(result.Errors);

            if(errorsToMatch is not null)
            {
                Assert.Equal(errorsToMatch, result.Errors);
            }
        }
    }
}
