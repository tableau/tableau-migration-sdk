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
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.Resources;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Manifest
{
    public class TestMigrationManifestSerializer : AutoFixtureTestBase
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

        public TestMigrationManifestSerializer()
        {
            AutoFixture.Register<IFileSystem>(() => new MockFileSystem());
        }

        [Fact]
        public async Task ManifestSaveLoadAsync()
        {
            // Arrange
            var manifest = Create<IMigrationManifest>();

            var tempFile = Path.GetTempFileName();

            // Verify that the test infra is working
            Assert.True(manifest.Entries.Any());
            Assert.True(manifest.Errors.Any());

            var serializer = Create<MigrationManifestSerializer>();
            var cancel = new CancellationToken();

            // Act
            await serializer.SaveAsync(manifest, tempFile);
            var loadedManifest = await serializer.LoadAsync(tempFile, cancel);

            // Assert
            Assert.NotNull(loadedManifest);
            Assert.Equal(manifest as MigrationManifest, loadedManifest);
        }

        [Fact]
        public async Task ManifestSaveLoad_DifferentVersionAsync()
        {
            var localizer = Create<ISharedResourcesLocalizer>();
            var logFactory = Create<ILoggerFactory>();

            var mockManifest = new Mock<MigrationManifest>(localizer, logFactory, Guid.NewGuid(), Guid.NewGuid(), null) { CallBase = true };
            mockManifest.Setup(m => m.ManifestVersion).Returns(1);

            var serializer = Create<MigrationManifestSerializer>();
            var cancel = new CancellationToken();

            // Save manifest V2, then try to load with the MigrationManifestSerializer that only supports V1
            var tempFile = Path.GetTempFileName();
            await serializer.SaveAsync(mockManifest.Object, tempFile);

            await Assert.ThrowsAsync<NotSupportedException>(() => serializer.LoadAsync(tempFile, cancel));
        }
    }
}
