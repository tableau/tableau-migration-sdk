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
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tableau.Migration.Config;
using Tableau.Migration.Content.Files;

namespace Tableau.Migration.Tests.Unit.Content.Files
{
    public abstract class ContentFileStoreTestBase<TFileStore> : AutoFixtureTestBase, IAsyncDisposable
        where TFileStore : IContentFileStore
    {
        private readonly Lazy<TFileStore> _fileStore;
        private readonly Lazy<ServiceProvider> _services;

        protected IServiceProvider Services => _services.Value;

        protected readonly MockFileSystem MockFileSystem = new();
        protected readonly Mock<IContentFilePathResolver> MockPathResolver = new();
        protected readonly Mock<IConfigReader> MockConfigReader = new();

        protected readonly MigrationSdkOptions SdkOptions;

        protected readonly string RootPath;
        protected readonly string BaseRelativePath;

        protected TFileStore FileStore => _fileStore.Value;

        public ContentFileStoreTestBase()
        {
            RootPath = Create<string>();
            BaseRelativePath = Create<string>();

            SdkOptions = Freeze<MigrationSdkOptions>();
            SdkOptions.Files.RootPath = RootPath;

            MockConfigReader.Setup(r => r.Get()).Returns(SdkOptions);

            _services = new(() => ConfigureServices().BuildServiceProvider());

            _fileStore = new(CreateFileStore);
        }

        protected virtual TFileStore CreateFileStore() => ActivatorUtilities.GetServiceOrCreateInstance<TFileStore>(Services);

        protected virtual IServiceCollection ConfigureServices(IServiceCollection services) => services;

        private IServiceCollection ConfigureServices()
        {
            var services = new ServiceCollection()
                .AddTableauMigrationSdk()
                .Replace<IFileSystem>(MockFileSystem)
                .Replace(MockPathResolver)
                .Replace(MockConfigReader);

            return ConfigureServices(services);
        }

        public async virtual ValueTask DisposeAsync()
        {
            if (_services.IsValueCreated)
                await _services.Value.DisposeAsync();

            GC.SuppressFinalize(this);
        }
    }
}
