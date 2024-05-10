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
using Tableau.Migration.Content.Files;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content.Files
{
    public class EncryptedFileHandleTests
    {
        public class Ctor : AutoFixtureTestBase
        {
            [Fact]
            public void CopiesInnerValues()
            {
                var mockStore = Freeze<Mock<IContentFileStore>>();
                var mockInner = Freeze<Mock<ContentFileHandle>>();

                var handle = new EncryptedFileHandle(mockStore.Object, mockInner.Object);
                Assert.Equal(mockInner.Object.Path, handle.Path);
                Assert.Equal(mockInner.Object.OriginalFileName, handle.OriginalFileName);
            }
        }

        public class DisposeAsync : AutoFixtureTestBase
        {
            [Fact]
            public async Task DisposesInnerStreamAsync()
            {
                var mockStore = Freeze<Mock<IContentFileStore>>();
                var mockInner = Freeze<Mock<ContentFileHandle>>();

                var handle = new EncryptedFileHandle(mockStore.Object, mockInner.Object);

                await handle.DisposeAsync();

                mockInner.Verify(x => x.DisposeAsync(), Times.Once);
            }
        }
    }
}
