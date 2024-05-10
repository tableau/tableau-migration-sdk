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

using Moq;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Tests
{
    /// <summary>
    /// A mocked <see cref="ISharedResourcesLocalizer"/> instance that is setup with with default behavior.
    /// </summary>
    public sealed class MockSharedResourcesLocalizer : Mock<ISharedResourcesLocalizer>
    {
        private readonly ISharedResourcesLocalizer _localizer = TestSharedResourcesLocalizer.Instance;

        public MockSharedResourcesLocalizer()
        {
            Setup(l => l.GetAllStrings(It.IsAny<bool>()))
                .Returns(_localizer.GetAllStrings);

            Setup(l => l[It.IsAny<string>()])
                .Returns<string>(value => _localizer[value]);
        }
    }
}
