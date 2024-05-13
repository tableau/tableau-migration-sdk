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
using Tableau.Migration.Content;
using Tableau.Migration.Content.Files;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content
{
    public class PublishableFlowTests
    {
        public class DisposeAsync : AutoFixtureTestBase
        {
            [Fact]
            public async Task DisposesFileAsync()
            {
                var innerFlow = Create<IFlow>();
                var mockFile = Create<Mock<IContentFileHandle>>();

                await using (var pf = new PublishableFlow(innerFlow, mockFile.Object))
                { }

                mockFile.Verify(x => x.DisposeAsync(), Times.Once);
            }
        }
    }
}
