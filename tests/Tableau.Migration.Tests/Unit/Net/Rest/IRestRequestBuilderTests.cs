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
using Tableau.Migration.Net.Rest;
using Tableau.Migration.Net.Rest.Paging;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net.Rest
{
    public class IRestRequestBuilderTests
    {
        public class WithPage
        {
            [Fact]
            public void PageNumberSizeComponentOverride()
            {
                var mockBuilder = new Mock<IRestRequestBuilder> { CallBase = true };
                mockBuilder.Setup(x => x.WithPage(It.IsAny<Page>())).Returns(mockBuilder.Object);

                var result = mockBuilder.Object.WithPage(2, 4);

                Assert.Same(mockBuilder.Object, result);
                mockBuilder.Verify(x => x.WithPage(It.Is<Page>(p => p == new Page(2, 4))), Times.Once);
            }
        }
    }
}
