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

using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.Engine.Options;

namespace Tableau.Migration.Tests.Unit
{
    public abstract class OptionsHookTestBase<TOptions> : AutoFixtureTestBase
        where TOptions : class, new()
    {
        protected readonly Mock<IMigrationPlanOptionsProvider<TOptions>> MockOptionsProvider;
        
        protected TOptions Options { get; set; }

        public OptionsHookTestBase()
        {
            Options = new();

            MockOptionsProvider = new();
            MockOptionsProvider.Setup(x => x.Get()).Returns(() => Options);
        }
    }
}
