//
//  Copyright (c) 2025, Salesforce, Inc.
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
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using Tableau.Migration.Engine.Manifest;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Manifest
{
    public class MigrationManifestSerializerTests : AutoFixtureTestBase
    {
        // If you need to debug these tests and you need access to the file that is saved and loaded, 
        // you need to make some temporary changes to this file.
        //
        // The TestMigrationManifestSerializer ctor creates a MockFileSystem, so files are not actually
        // saved to disk. If you want to see the manifest that is created, change MockFileSystem line to
        //      AutoFixture.Register<IFileSystem>(() => new FileSystem());
        // That will mean the test will use the real file system.
        //
        // The actual tests also a the temp file to save the manifest to. You can change that to a real filepath 
        // so it's easier to find during the actual debugging. 

        public MigrationManifestSerializerTests()
        {
            AutoFixture.Register<IFileSystem>(() => new MockFileSystem());
        }

        [Fact]
        public async Task ManifestSaveLoadAsync()
        {
            // Arrange
            var manifest = Create<IMigrationManifest>();

            using var tempFile = new TempFile();

            // Verify that the test infra is working
            Assert.True(manifest.Entries.Any());
            Assert.True(manifest.Errors.Any());

            var serializer = Create<MigrationManifestSerializer>();
            var cancel = new CancellationToken();

            // Act
            await serializer.SaveAsync(manifest, tempFile.FilePath);
            var loadedManifest = await serializer.LoadAsync(tempFile.FilePath, cancel);

            // Assert
            Assert.NotNull(loadedManifest);
            Assert.Equal(manifest as MigrationManifest, loadedManifest);
        }

        [Fact]
        public async Task ManifestSaveLoad_DifferentVersionAsync()
        {
            var mockManifest = new Mock<MigrationManifest>(Guid.NewGuid(), Guid.NewGuid(), PipelineProfile.ServerToCloud, null) { CallBase = true };
            mockManifest.Setup(m => m.ManifestVersion).Returns(1);

            var serializer = Create<MigrationManifestSerializer>();
            var cancel = new CancellationToken();

            // Save manifest V2, then try to load with the MigrationManifestSerializer that only supports V1
            using var tempFile = new TempFile();
            await serializer.SaveAsync(mockManifest.Object, tempFile.FilePath);

            await Assert.ThrowsAsync<NotSupportedException>(() => serializer.LoadAsync(tempFile.FilePath, cancel));
        }

        [Fact]
        public async Task ManifestSaveLoad_WithoutPipeline()
        {
            // Manifest without a pipeline profile
            string emptyManifest = @"
{
  ""PlanId"": ""f036f386-3987-41ad-a5a8-d4e73a0e068f"",
  ""MigrationId"": ""5823024a-c391-4294-8c47-eec4b729a4ee"",
  ""Errors"": [ ],
  ""Entries"": {},
  ""ManifestVersion"": 4
}";

            using var tempFile = new TempFile();

            // Save the JSON string to the temp file
            File.WriteAllText(tempFile.FilePath, emptyManifest);

            // Use a real file system for this specific test
            var realFileSystem = new FileSystem();
            var serializer = new MigrationManifestSerializer(realFileSystem);
            var cancel = new CancellationToken();

            // Act
            var loadedManifest = await serializer.LoadAsync(tempFile.FilePath, cancel);

            // Assert
            Assert.NotNull(loadedManifest);
            Assert.Equal(PipelineProfile.ServerToCloud, loadedManifest.PipelineProfile);
        }
    }

    public class TempFile : IDisposable
    {
        public string FilePath { get; }

        public TempFile()
        {
            FilePath = Path.GetTempFileName();
        }

        public void Dispose()
        {
            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
            }
        }
    }
}
