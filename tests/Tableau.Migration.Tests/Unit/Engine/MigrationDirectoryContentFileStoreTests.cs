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
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tableau.Migration.Engine;
using Tableau.Migration.Tests.Unit.Content.Files;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine
{
    public class MigrationDirectoryContentFileStoreTests
    {
        public abstract class MigrationDirectoryContentFileStoreTest : DirectoryContentFileStoreTestBase<MigrationDirectoryContentFileStore>
        {
            protected readonly Mock<IMigrationInput> MockMigrationInput = new();

            protected readonly Guid MigrationId = Guid.NewGuid();

            public MigrationDirectoryContentFileStoreTest()
            { 
                MockMigrationInput.SetupGet(i => i.MigrationId).Returns(MigrationId);
            }

            protected override IServiceCollection ConfigureServices(IServiceCollection services)
            {
                return services
                    .Replace(MockMigrationInput);
            }

            protected override MigrationDirectoryContentFileStore CreateFileStore()
                => Services.GetRequiredService<MigrationDirectoryContentFileStore>();
        }

        public class Ctor : MigrationDirectoryContentFileStoreTest
        {
            [Fact]
            public void UsesMigrationSubDirectory()
            {
                Assert.Equal(Path.Combine(RootPath, $"migration-{MigrationId:N}"), BaseStorePath);
            }
        }
    }
}
