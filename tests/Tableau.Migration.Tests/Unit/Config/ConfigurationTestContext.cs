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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Tableau.Migration.Config;

namespace Tableau.Migration.Tests.Unit.Config
{
    public class ConfigurationTestContext : IAsyncDisposable
    {
        private readonly bool _disposeConfigFile;

        public readonly TestConfigFile? ConfigFile;
        public readonly IServiceProvider ServiceProvider;

        public IConfigReader ConfigReader => ServiceProvider.GetRequiredService<IConfigReader>();
        public IOptionsMonitor<MigrationSdkOptions> Monitor => ServiceProvider.GetRequiredService<IOptionsMonitor<MigrationSdkOptions>>();

        public ConfigurationTestContext(TestConfigFile? configFile, bool disposeConfigFile)
        {
            ConfigFile = configFile;
            ServiceProvider = ConfigureServices();
            _disposeConfigFile = disposeConfigFile;
        }

        public static async Task<ConfigurationTestContext> FromContentAsync(string content, CancellationToken cancel)
            => new(await TestConfigFile.FromContentAsync(content, cancel).ConfigureAwait(false), true);

        public static ConfigurationTestContext WithoutConfigFile() => new(null, false);

        public async Task<MigrationSdkOptions> WaitForUpdateAsync(Func<Task> updateAsync, TimeSpan timeout)
        {
            var tcs = new TaskCompletionSource<MigrationSdkOptions>();

            using var watcher = Monitor.OnChange(o =>
            {
                tcs.TrySetResult(o);
            });

            await updateAsync().ConfigureAwait(false);

            try
            {
                return await tcs.Task.WaitAsync(timeout).ConfigureAwait(false);
            }
            catch (TimeoutException ex)
            {
                throw new TimeoutException($"The configuration was not updated in {timeout}.", ex);
            }
        }

        public MigrationSdkOptions GetCurrentConfiguration() => ConfigReader.Get();

        public ContentTypesOptions GetCurrentConfiguration<TOptions>()
            where TOptions : IContentReference
            => ConfigReader.Get<TOptions>();

        private ServiceProvider ConfigureServices()
        {
            var configBuilder = new ConfigurationBuilder();

            if (ConfigFile is not null)
                configBuilder.AddJsonFile(path: ConfigFile.FilePath, optional: false, reloadOnChange: true);

            configBuilder.AddEnvironmentVariables();

            var configuration = configBuilder.Build();

            return new ServiceCollection()
                .AddLogging()
                .AddSingleton(Mock.Of<ILoggerProvider>())
                .AddTableauMigrationSdk(configuration.GetSection(nameof(MigrationSdkOptions)))
                .BuildServiceProvider();
        }

        #region - IAsyncDisposable -

        public async ValueTask DisposeAsync()
        {
            await ServiceProvider.DisposeIfNeededAsync();

            if (_disposeConfigFile && ConfigFile is not null)
                await ConfigFile.DisposeAsync();

            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
