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

using System;
using System.IO;
using System.IO.Abstractions;
using Moq;
using Tableau.Migration.Config;
using Tableau.Migration.Content.Files;
using Tableau.Migration.Engine;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine
{
    public class MigrationDirectoryContentFileStoreTests
    {
        public class TestFileStore : MigrationDirectoryContentFileStore
        {
            public TestFileStore(IFileSystem fileSystem, IContentFilePathResolver pathResolver, IConfigReader configReader, IMigrationInput migrationInput)
                : base(fileSystem, pathResolver, configReader, migrationInput)
            { }

            public string PublicBaseStorePath => BaseStorePath;

        }

        public class Ctor : AutoFixtureTestBase
        {
            [Fact]
            public void UsesMigrationSubDirectory()
            {
                var rootPath = Create<string>();

                var config = Freeze<MigrationSdkOptions>();
                config.Files.RootPath = rootPath;

                var migrationId = Guid.NewGuid();
                var mockInput = Freeze<Mock<IMigrationInput>>();
                mockInput.SetupGet(x => x.MigrationId).Returns(migrationId);

                var fs = Create<TestFileStore>();

                Assert.Equal(Path.Combine(rootPath, $"migration-{migrationId:N}"), fs.PublicBaseStorePath);
            }
        }
    }
}
