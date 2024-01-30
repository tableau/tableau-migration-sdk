// Copyright (c) 2023, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the ""License"") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an ""AS IS"" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.Config;
using Tableau.Migration.Content.Files;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content.Files
{
    public class EncryptedFileStoreTests
    {
        #region - Test Classes -

        public class EncryptedFileStoreTest : AutoFixtureTestBase
        {
            protected readonly Mock<ISymmetricEncryptionFactory> MockEncryptionFactory;
            protected readonly Mock<MemoryContentFileStore> MockInnerFileStore;
            protected readonly Mock<ILogger<EncryptedFileStore>> MockLogger;

            protected bool DisableFileEncryption { get; set; }

            protected readonly EncryptedFileStore FileStore;

            public EncryptedFileStoreTest()
            {
                MockEncryptionFactory = Freeze<Mock<ISymmetricEncryptionFactory>>();
                MockEncryptionFactory.Setup(x => x.Create())
                    .Returns(() => Aes.Create());

                MockInnerFileStore = new Mock<MemoryContentFileStore>()
                {
                    CallBase = true
                };
                AutoFixture.Register<IContentFileStore>(() => MockInnerFileStore.Object);

                MockLogger = Freeze<Mock<ILogger<EncryptedFileStore>>>();

                var mockConfig = Freeze<Mock<IConfigReader>>();
                mockConfig.Setup(x => x.Get())
                    .Returns(() => new MigrationSdkOptions
                    {
                        Files = new()
                        {
                            DisableFileEncryption = DisableFileEncryption
                        }
                    });

                FileStore = Create<EncryptedFileStore>();
            }
        }

        #endregion

        #region - Ctor -

        public class Ctor : EncryptedFileStoreTest
        {
            [Fact]
            public void WarningOnEncryptionDisabled()
            {
                DisableFileEncryption = true;

                var store = Create<EncryptedFileStore>();

                MockLogger.VerifyWarnings(Times.Once);
            }
        }

        #endregion

        #region - Create -

        public class Create : EncryptedFileStoreTest
        {
            [Fact]
            public void UsesInnerStore()
            {
                var path = Create<string>();
                var originalFileName = Create<string>();

                var handle = FileStore.Create(path, originalFileName);

                var encryptedHandle = Assert.IsType<EncryptedFileHandle>(handle);

                Assert.Equal(path, encryptedHandle.Path);
                Assert.Equal(originalFileName, encryptedHandle.OriginalFileName);

                MockInnerFileStore.Verify(x => x.Create(path, originalFileName), Times.Once);
            }

            [Fact]
            public void UsesInnerStoreWithContentItem()
            {
                var contentItem = Create<TestContentType>();
                var originalFileName = Create<string>();

                var handle = FileStore.Create(contentItem, originalFileName);

                var encryptedHandle = Assert.IsType<EncryptedFileHandle>(handle);

                Assert.Equal(originalFileName, encryptedHandle.OriginalFileName);

                MockInnerFileStore.Verify(x => x.Create(contentItem, originalFileName), Times.Once);
            }
        }

        #endregion

        #region - DeleteAsync -

        public class DeleteAsync : EncryptedFileStoreTest
        {
            [Fact]
            public async Task CallsInnerStoreAsync()
            {
                var innerHandle = new ContentFileHandle(MockInnerFileStore.Object, Create<string>(), Create<string>());

                await FileStore.DeleteAsync(innerHandle, Cancel);

                MockInnerFileStore.Verify(x => x.DeleteAsync(innerHandle, Cancel), Times.Once);
            }
        }

        #endregion

        #region - GetTableauFileEditorAsync -

        public class GetTableauFileEditorAsync : EncryptedFileStoreTest
        {
            [Fact]
            public async Task CallsInnerStoreAsync()
            {
                var path = Create<string>();
                await using var file = FileStore.Create(path, Create<string>());

                var innerHandle = new ContentFileHandle(MockInnerFileStore.Object, path, Create<string>());

                await FileStore.GetTableauFileEditorAsync(innerHandle, Cancel);

                MockInnerFileStore.Verify(x => x.GetTableauFileEditorAsync(innerHandle, Cancel, null), Times.Once);
            }
        }

        #endregion

        #region - CloseTableauFileEditorAsync -

        public class CloseTableauFileEditorAsync : EncryptedFileStoreTest
        {
            [Fact]
            public async Task CallsInnerStoreAsync()
            {
                var innerHandle = new ContentFileHandle(MockInnerFileStore.Object, Create<string>(), Create<string>());

                await FileStore.CloseTableauFileEditorAsync(innerHandle, Cancel);

                MockInnerFileStore.Verify(x => x.CloseTableauFileEditorAsync(innerHandle, Cancel), Times.Once);
            }
        }

        #endregion

        #region - OpenReadAsync/OpenWriteAsync -

        public class Roundtrip : EncryptedFileStoreTest
        {
            [Fact]
            public async Task RoundtripEncryptionAsync()
            {
                const string content = "hi2u";
                const string path = "test.txt";

                await using var file = FileStore.Create(path, Create<string>());

                await using (var writeStream = await file.OpenWriteAsync(Cancel))
                await using (var writer = new StreamWriter(writeStream.Content, Encoding.UTF8))
                {
                    await writer.WriteAsync(content);
                }

                var encryptedValue = Encoding.UTF8.GetString(MockInnerFileStore.Object.GetFileData(path));
                Assert.NotEqual(content, encryptedValue);
                Assert.NotEmpty(encryptedValue);

                string roundtrip;
                await using (var readStream = await file.OpenReadAsync(Cancel))
                using (var reader = new StreamReader(readStream.Content))
                {
                    roundtrip = await reader.ReadToEndAsync();
                }

                Assert.Equal(content, roundtrip);
            }
        }

        #endregion
    }
}
