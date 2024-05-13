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
    public class ContentFileHandleTests
    {
        public class ContentFileHandleTest : AutoFixtureTestBase
        {
            protected readonly Mock<IContentFileStore> MockFileStore;
            protected readonly ContentFileHandle Handle;

            public ContentFileHandleTest()
            {
                MockFileStore = Freeze<Mock<IContentFileStore>>();
                Handle = Create<ContentFileHandle>();
            }
        }

        public class OpenReadAsync : ContentFileHandleTest
        {
            [Fact]
            public async Task OpensFromStoreAsync()
            {
                var storeStream = Freeze<IContentFileStream>();

                var stream = await Handle.OpenReadAsync(Cancel);

                Assert.Same(storeStream, stream);
                MockFileStore.Verify(x => x.OpenReadAsync(Handle, Cancel), Times.Once);
            }
        }

        public class OpenWriteAsync : ContentFileHandleTest
        {
            [Fact]
            public async Task OpensFromStoreAsync()
            {
                var storeStream = Freeze<IContentFileStream>();

                var stream = await Handle.OpenWriteAsync(Cancel);

                Assert.Same(storeStream, stream);
                MockFileStore.Verify(x => x.OpenWriteAsync(Handle, Cancel), Times.Once);
            }
        }

        public class GetXmlStreamAsync : ContentFileHandleTest
        {
            [Fact]
            public async Task OpensFromStoreAsync()
            {
                var editorStream = Freeze<ITableauFileXmlStream>();
                //var storeEditor = Freeze<ITableauFileEditor>();

                var stream = await Handle.GetXmlStreamAsync(Cancel);

                Assert.Same(editorStream, stream);
                MockFileStore.Verify(x => x.GetTableauFileEditorAsync(Handle, Cancel, null), Times.Once);
            }
        }

        public class DisposAsync : ContentFileHandleTest
        {
            [Fact]
            public async Task DeletesFromStoreAsync()
            {
                await Handle.DisposeAsync();

                MockFileStore.Verify(x => x.DeleteAsync(Handle, default), Times.Once);
            }

            [Fact]
            public async Task DisposeTwiceCallDeleteOnce()
            {
                await Handle.DisposeAsync();
                await Handle.DisposeAsync();

                MockFileStore.Verify(x => x.DeleteAsync(Handle, default), Times.Once);
            }
        }
    }
}
