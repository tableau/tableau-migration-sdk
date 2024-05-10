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

using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tableau.Migration.Content.Files;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content.Files
{
    public class DirectoryContentFileStoreTests
    {
        #region - Test Classes -

        public abstract class DirectoryContentFileStoreTest : DirectoryContentFileStoreTestBase<DirectoryContentFileStore>
        {
            protected override DirectoryContentFileStore CreateFileStore()
                => ActivatorUtilities.CreateInstance<DirectoryContentFileStore>(Services, BaseRelativePath);
        }

        #endregion

        #region - Create -

        public class Create : DirectoryContentFileStoreTest
        {
            [Fact]
            public async Task InitializesAndTracksFileAsync()
            {
                var relPath = Create<string>();
                var originalFileName = Create<string>();

                await using var file = FileStore.Create(relPath, originalFileName);

                Assert.Same(FileStore, file.Store);
                Assert.Equal(Path.Combine(ExpectedBasePath, relPath), file.Path);
                Assert.Equal(originalFileName, file.OriginalFileName);

                Assert.Contains(file.Path, TrackedFilePaths);
            }

            [Fact]
            public async Task InitializesAndTracksFileWithContentItemAsync()
            {
                var contentItem = Create<TestContentType>();
                var originalFileName = Create<string>();
                var generatedPath = Create<string>();

                MockPathResolver.Setup(x => x.ResolveRelativePath(contentItem, originalFileName))
                    .Returns(generatedPath);

                await using var file = FileStore.Create(contentItem, originalFileName);

                Assert.Same(FileStore, file.Store);
                Assert.Equal(Path.Combine(ExpectedBasePath, generatedPath), file.Path);
                Assert.Equal(originalFileName, file.OriginalFileName);

                Assert.Contains(file.Path, TrackedFilePaths);

                MockPathResolver.Verify(x => x.ResolveRelativePath(contentItem, originalFileName), Times.Once());
            }
        }

        #endregion

        #region - OpenReadAsync -

        public class OpenReadAsync : DirectoryContentFileStoreTest
        {
            [Fact]
            public async Task OpensStreamAsync()
            {
                var expectedContent = Create<string>();
                var relPath = Create<string>();

                await using var file = await CreateTestFileAsync(relPath, "test", expectedContent);

                await using var readStream = await FileStore.OpenReadAsync(file, Cancel);

                var readContent = new StreamReader(readStream.Content).ReadToEnd();

                Assert.Equal(expectedContent, readContent);
            }
        }

        #endregion

        #region - OpenWriteAsync -

        public class OpenWriteAsync : DirectoryContentFileStoreTest
        {
            [Fact]
            public async Task OverwritesAsync()
            {
                var expectedContent = Create<string>();
                var relPath = Create<string>();

                await using var file = await CreateTestFileAsync(relPath, "test", expectedContent + "initial content");

                await using (var writeStream = await FileStore.OpenWriteAsync(file, Cancel))
                {
                    writeStream.Content.Write(Constants.DefaultEncoding.GetBytes(expectedContent));
                }

                await using var readStream = await FileStore.OpenReadAsync(file, Cancel);
                var readContent = new StreamReader(readStream.Content).ReadToEnd();

                Assert.Equal(expectedContent, readContent);
            }
        }

        #endregion

        #region - DeleteAsync -

        public class DeleteAsync : DirectoryContentFileStoreTest
        {
            [Fact]
            public async Task DeletesAndUntracksFileAsync()
            {
                var relPath = Create<string>();

                await using var file = await CreateTestFileAsync(relPath, "test");

                await FileStore.DeleteAsync(file, Cancel);

                Assert.False(MockFileSystem.FileExists(file.Path));
                Assert.DoesNotContain(file.Path, TrackedFilePaths);
            }
        }

        #endregion

        #region -  GetTableauFileEditorAsync -

        public class GetTableauFileEditorAsync : DirectoryContentFileStoreTest
        {
            [Fact]
            public async Task CreatesAndReusesEditorAsync()
            {
                var relPath = Create<string>();

                await using var file = await CreateTestFileAsync(relPath, "test");

                await using var editor1 = await FileStore.GetTableauFileEditorAsync(file, Cancel);
                var editor2 = await FileStore.GetTableauFileEditorAsync(file, Cancel);

                Assert.Same(editor1, editor2);
            }
        }

        #endregion

        #region - CloseTableauFileEditorAsync -

        public class CloseTableauFileEditorAsync : DirectoryContentFileStoreTest
        {
            [Fact]
            public async Task DisposesEditorAsync()
            {
                var relPath = Create<string>();

                await using var file = await CreateTestFileAsync(relPath, "test");

                var editor = await FileStore.GetTableauFileEditorAsync(file, Cancel);

                await FileStore.CloseTableauFileEditorAsync(file, Cancel);

                Assert.Throws<ObjectDisposedException>(() => editor.Content.ReadByte());
            }
        }

        #endregion

        #region - DisposeAsync -

        public class DisposeAsync : DirectoryContentFileStoreTest
        {
            [Fact]
            public async Task CleansUpDirectoryAndAllTrackedFilesAsync()
            {
                var relPaths = CreateMany<string>();

                var files = (await Task.WhenAll(relPaths
                    .Select(async p =>
                    {
                        var file = await CreateTestFileAsync(p, "test");
                        // leave the editor open
                        _ = await FileStore.GetTableauFileEditorAsync(file, Cancel);
                        return file;
                    })
                )).ToImmutableArray();

                Assert.True(FileStore.HasOpenTableauFileEditor);

                await FileStore.DisposeAsync();

                Assert.All(files, file =>
                {
                    Assert.False(MockFileSystem.FileExists(file.Path));
                });

                Assert.Empty(TrackedFilePaths);
                Assert.False(MockFileSystem.Directory.Exists(ExpectedBasePath));
                Assert.False(FileStore.HasOpenTableauFileEditor);
            }
        }

        #endregion
    }
}
