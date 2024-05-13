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

using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
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

        public abstract class EncryptedFileStoreTest : ContentFileStoreTestBase<EncryptedFileStore>
        {
            protected readonly Mock<ISymmetricEncryptionFactory> MockEncryptionFactory = new();
            protected readonly Mock<MemoryContentFileStore> MockInnerFileStore;
            protected readonly Mock<ILogger<EncryptedFileStore>> MockLogger = new();
            protected readonly MockSharedResourcesLocalizer MockSharedResourcesLocalizer = new();

            protected bool DisableFileEncryption { get; set; }

            public EncryptedFileStoreTest()
            {
                MockInnerFileStore = new(MemoryStreamManager.Instance)
                {
                    CallBase = true
                };

                MockEncryptionFactory.Setup(f => f.Create()).Returns(Aes.Create());

                MockConfigReader.Setup(x => x.Get())
                    .Returns(() => new MigrationSdkOptions
                    {
                        Files = new()
                        {
                            DisableFileEncryption = DisableFileEncryption
                        }
                    });
            }

            protected override IServiceCollection ConfigureServices(IServiceCollection services)
            {
                var mockLoggerFactory = new Mock<ILoggerFactory>();
                mockLoggerFactory.Setup(f => f.CreateLogger(It.Is<string>(s => s.Contains(nameof(EncryptedFileStore)))))
                    .Returns(MockLogger.Object);

                return services
                    .Replace(mockLoggerFactory)
                    .Replace(MockSharedResourcesLocalizer)
                    .Replace(MockEncryptionFactory)
                    .AddScoped(p => new EncryptedFileStore(p, MockInnerFileStore.Object));
            }

            protected override EncryptedFileStore CreateFileStore() => Services.GetRequiredService<EncryptedFileStore>();
        }

        #endregion

        #region - Ctor -

        public class Ctor : EncryptedFileStoreTest
        {
            [Fact]
            public void WarningOnEncryptionDisabled()
            {
                DisableFileEncryption = true;

                var store = new EncryptedFileStore(Services, MockInnerFileStore.Object);

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
                {
                    await using (var writeStream = await file.OpenWriteAsync(Cancel))
                    await using (var writer = new StreamWriter(writeStream.Content, Constants.DefaultEncoding))
                    {
                        await writer.WriteAsync(content);
                    }

                    var encryptedValue = Constants.DefaultEncoding.GetString(MockInnerFileStore.Object.GetFileData(path));
                    Assert.NotEqual(content, encryptedValue);
                    Assert.NotEmpty(encryptedValue);

                    string roundtrip;
                    await using var readStream = await file.OpenReadAsync(Cancel);

                    using (var reader = new StreamReader(readStream.Content))
                    {
                        roundtrip = await reader.ReadToEndAsync();
                    }

                    Assert.Equal(content, roundtrip);
                }
            }
        }

        #endregion
    }
}
