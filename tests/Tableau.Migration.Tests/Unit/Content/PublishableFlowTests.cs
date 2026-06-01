//
//  Copyright (c) 2026, Salesforce, Inc.
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

using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Files;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content
{
    public class PublishableFlowTests
    {
        public class Ctor : AutoFixtureTestBase
        {
            [Fact]
            public void InitializesWithConnections()
            {
                var innerFlow = Create<IFlow>();
                var connections = CreateMany<IConnection>(3).ToImmutableList();
                var mockFile = Create<Mock<IContentFileHandle>>();

                var result = new PublishableFlow(innerFlow, connections, mockFile.Object);

                Assert.Equal(innerFlow.Id, result.Id);
                Assert.Equal(innerFlow.Name, result.Name);
                Assert.Equal(3, result.Connections.Count);
                Assert.Same(connections, result.Connections);
                Assert.Same(mockFile.Object, result.File);
            }

            [Fact]
            public void InitializesWithEmptyConnections()
            {
                var innerFlow = Create<IFlow>();
                var connections = ImmutableList<IConnection>.Empty;
                var mockFile = Create<Mock<IContentFileHandle>>();

                var result = new PublishableFlow(innerFlow, connections, mockFile.Object);

                Assert.Equal(innerFlow.Id, result.Id);
                Assert.Equal(innerFlow.Name, result.Name);
                Assert.Empty(result.Connections);
                Assert.Same(connections, result.Connections);
                Assert.Same(mockFile.Object, result.File);
            }
        }

        public class DisposeAsync : AutoFixtureTestBase
        {
            [Fact]
            public async Task DisposesFileAsync()
            {
                var innerFlow = Create<IFlow>();
                var connections = ImmutableList<IConnection>.Empty;
                var mockFile = Create<Mock<IContentFileHandle>>();

                await using (var pf = new PublishableFlow(innerFlow, connections, mockFile.Object))
                { }

                mockFile.Verify(x => x.DisposeAsync(), Times.Once);
            }
        }
    }
}
