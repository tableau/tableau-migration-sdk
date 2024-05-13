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

using System.Diagnostics.CodeAnalysis;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models;
using Xunit;

namespace Tableau.Migration.Tests
{
    public static class RestExceptionExtensions
    {
        [return: NotNullIfNotNull(nameof(error))]
        public static void AssertErrorEquals(
            this RestException exception,
            [NotNullIfNotNull(nameof(error))] Error? error)
        {
            Assert.NotNull(error);
            Assert.Equal(error, exception.ToError());
        }

        public static Error ToError(this RestException exception)
            => new()
            {
                Code = exception.Code,
                Detail = exception.Detail,
                Summary = exception.Summary,
            };
    }
}
