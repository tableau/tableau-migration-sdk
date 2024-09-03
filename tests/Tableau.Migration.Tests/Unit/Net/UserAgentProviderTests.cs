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
using Tableau.Migration.Config;
using Tableau.Migration.Net;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net
{
    public sealed class UserAgentProviderTests
    {
        public sealed class UserAgent : AutoFixtureTestBase
        {
            private readonly IMigrationSdk _metadata;

            private MigrationSdkOptions _options;

            public UserAgent()
            {
                _metadata = Freeze<IMigrationSdk>();

                _options = new();
                var mockConfig = Freeze<Mock<IConfigReader>>();
                mockConfig.Setup(x => x.Get()).Returns(() => _options);                
            }

            [Fact]
            public void NoComments()
            {
                var p = Create<UserAgentProvider>();

                Assert.Equal($"{Constants.USER_AGENT_PREFIX}/{_metadata.Version}", p.UserAgent);
            }

            [Fact]
            public void WithComments()
            {
                _options.Network.UserAgentComment = "no comment";

                var p = Create<UserAgentProvider>();

                Assert.Equal($"{Constants.USER_AGENT_PREFIX}/{_metadata.Version} ({_options.Network.UserAgentComment})", p.UserAgent);
            }
        }
    }
}
