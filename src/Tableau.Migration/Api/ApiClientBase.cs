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
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Net.Rest;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Api
{
    internal abstract class ApiClientBase
    {
        protected readonly IRestRequestBuilderFactory RestRequestBuilderFactory;
        protected readonly ILogger Logger;
        protected readonly ISharedResourcesLocalizer SharedResourcesLocalizer;

        public ApiClientBase(
            IRestRequestBuilderFactory restRequestBuilderFactory,
            ILoggerFactory loggerFactory,
            ISharedResourcesLocalizer sharedResourcesLocalizer)
        {
            RestRequestBuilderFactory = restRequestBuilderFactory;
            Logger = loggerFactory.CreateLogger(GetType());
            SharedResourcesLocalizer = sharedResourcesLocalizer;
        }

        public T ExecuteForInstanceType<T>(TableauInstanceType expected, TableauInstanceType actual, Func<T> executeIfSupported)
            where T : notnull
        {
            if (AssertInstanceType(expected, actual, true))
                return executeIfSupported();

            return default;
        }

        public T ReturnForInstanceType<T>(TableauInstanceType expected, TableauInstanceType actual, T returnValueWhenSupported)
            where T : notnull
        {
            if (AssertInstanceType(expected, actual, true))
                return returnValueWhenSupported;

            return default;
        }

        public bool AssertInstanceType(TableauInstanceType expected, TableauInstanceType actual, [DoesNotReturnIf(true)] bool throwOnFailure)
            => actual == expected
                ? true
                : throwOnFailure
                    ? throw new TableauInstanceTypeNotSupportedException(actual, SharedResourcesLocalizer)
                    : false;
    }
}
