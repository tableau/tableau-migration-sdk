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

using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Preparation;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Preparation
{
    public class EndpointContentItemPreparerTests
    {
        public class PullAsync : ContentItemPreparerTestBase<TestPublishType>
        {
            private readonly Mock<ISourceEndpoint> _mockSourceEndpoint;
            private readonly EndpointContentItemPreparer<TestContentType, TestPublishType> _preparer;

            public PullAsync()
            {
                _mockSourceEndpoint = Freeze<Mock<ISourceEndpoint>>();
                _preparer = Create<EndpointContentItemPreparer<TestContentType, TestPublishType>>();
            }

            [Fact]
            public async Task PullsFromSourceEndpointAsync()
            {
                var pullResult = Result<TestPublishType>.Succeeded(new());
                _mockSourceEndpoint.Setup(x => x.PullAsync<TestContentType, TestPublishType>(Item.SourceItem, Cancel))
                    .ReturnsAsync(pullResult);

                var result = await _preparer.PrepareAsync(Item, Cancel);

                result.AssertSuccess();
                Assert.Same(pullResult.Value, result.Value);
                _mockSourceEndpoint.Verify(x => x.PullAsync<TestContentType, TestPublishType>(Item.SourceItem, Cancel), Times.Once);
            }
        }
    }
}
