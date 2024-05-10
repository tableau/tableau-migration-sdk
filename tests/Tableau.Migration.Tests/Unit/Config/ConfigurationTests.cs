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
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tableau.Migration.Config;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Permissions;
using Tableau.Migration.Engine.Pipelines;
using Xunit;
using Xunit.Abstractions;

namespace Tableau.Migration.Tests.Unit.Config
{
    [Category("Configuration")]
    public class ConfigurationTests
        : IDisposable
    {
        private static readonly string ASSEMBLY_PATH = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        private static readonly string TEST_DATA_PATH = Path.Combine(ASSEMBLY_PATH, TEST_DATA_DIR);
        private static readonly string TEST_DATA_FILE1_PATH = Path.Combine(TEST_DATA_PATH, TEST_DATA_FILE1);
        private static readonly string TEST_DATA_FILE2_PATH = Path.Combine(TEST_DATA_PATH, TEST_DATA_FILE2);
        private static readonly string TEST_DATA_FILE3_PATH = Path.Combine(TEST_DATA_PATH, TEST_DATA_FILE3);
        private const string TEST_DATA_DIR = "TestData";
        private const string TEST_DATA_FILE1 = "configuration_testdata1.json";
        private static readonly string TEST_DATA_FILE1_CONTENT =
$@"
{{
    ""MigrationSdkOptions"": {{
        ""contentTypes"": [
        {{
            ""type"":""User"",
            ""batchSize"": {TestData.USER_BATCH_SIZE}            
        }},
        {{
            ""type"":""Group"",
            ""batchSize"": {TestData.GROUP_BATCH_SIZE}            
        }},
        {{
            ""type"":""Project"",
            ""batchSize"": {TestData.PROJECT_BATCH_SIZE}            
        }},
        {{
            ""type"":""Workbook"",
            ""batchSize"": {TestData.WORKBOOK_BATCH_SIZE}            
        }},
        {{
            ""type"":""DataSource"",
            ""batchSize"": {TestData.DATASOURCE_BATCH_SIZE}            
        }}
        ],
        ""Network"": {{
            ""{nameof(MigrationSdkOptions.Network.FileChunkSizeKB)}"": {TestData.FILE_CHUNK_SIZE_GB}
        }}
    }}
}}
";
        private const string TEST_DATA_FILE2 = "configuration_testdata2.json";
        private static readonly string TEST_DATA_FILE2_CONTENT =
$@"
{{
    ""MigrationSdkOptions"": {{
        ""contentTypes"": [
        {{
            ""type"":""User"",
            ""batchSize"": {TestData.USER_BATCH_SIZE}            
        }},
        {{
            ""type"":""Group"",
            ""batchSize"": {TestData.GROUP_BATCH_SIZE}            
        }},
        {{
            ""type"":""Project"",
            ""batchSize"": {TestData.PROJECT_BATCH_SIZE}            
        }},
        {{
            ""type"":""Workbook"",
            ""batchSize"": {TestData.WORKBOOK_BATCH_SIZE}            
        }},
        {{
            ""type"":""DataSource"",
            ""batchSize"": {TestData.DATASOURCE_BATCH_SIZE}            
        }}
        ],
        ""Network"": {{
            ""{nameof(MigrationSdkOptions.Network.FileChunkSizeKB)}"": {TestData.FILE_CHUNK_SIZE_GB}
        }}
    }}
}}
";

        private const string TEST_DATA_FILE3 = "configuration_testdata3.json";
        private static readonly string TEST_DATA_FILE3_CONTENT =
$@"{{
  ""MigrationSdkOptions"": {{
    ""DefaultPermissionsContentTypes"": {{
      ""{nameof(MigrationSdkOptions.DefaultPermissionsContentTypes.UrlSegments)}"": [""test1"", ""test2""]
    }}
  }}
}}";
        protected class TestData
        {
            public const int USER_BATCH_SIZE = 201;
            public const int GROUP_BATCH_SIZE = 202;
            public const int PROJECT_BATCH_SIZE = 203;
            public const int WORKBOOK_BATCH_SIZE = 204;
            public const int DATASOURCE_BATCH_SIZE = 205;

            public const int FILE_CHUNK_SIZE_GB = 2034;
        }
        private readonly ITestOutputHelper _output;

        private readonly bool skipGithubRunnerTests;

        public ConfigurationTests(ITestOutputHelper output)
        {
            _output = output;

            var config = Environment.GetEnvironmentVariable("MIGRATIONSDK_SKIP_FLAKY_TESTS");

            skipGithubRunnerTests = config?.Equals("yes", StringComparison.OrdinalIgnoreCase) ?? false;

            Directory.CreateDirectory(
                TEST_DATA_PATH);
            File.WriteAllText(
                TEST_DATA_FILE1_PATH,
                TEST_DATA_FILE1_CONTENT);
            File.WriteAllText(
                TEST_DATA_FILE2_PATH,
                TEST_DATA_FILE2_CONTENT);
            File.WriteAllText(
                TEST_DATA_FILE3_PATH,
                TEST_DATA_FILE3_CONTENT);
        }

        public void Dispose()
        {
            Directory.Delete(TEST_DATA_PATH, recursive: true);
            GC.SuppressFinalize(this);
        }

        #region - Helper Methods -

        private static ServiceProvider GetServiceProvider(string? testDataFile = null)
        {
            var serviceCollection = new ServiceCollection().AddLogging()
                                                           .AddSingleton(Mock.Of<ILoggerProvider>());

            if (string.IsNullOrEmpty(testDataFile))
                return serviceCollection.AddTableauMigrationSdk()
                                        .BuildServiceProvider();

            var assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Assert.NotNull(assemblyLocation);
            var configuration = new ConfigurationBuilder()
                                .SetBasePath(assemblyLocation)
                                .AddJsonFile(path: Path.Combine(assemblyLocation, TEST_DATA_DIR, testDataFile), optional: false, reloadOnChange: true)
                                .AddEnvironmentVariables()
                                .Build();

            return serviceCollection.AddTableauMigrationSdk(configuration.GetSection(nameof(MigrationSdkOptions)))
                                    .BuildServiceProvider();
        }

        private static async Task EditConfigFile(string path, Dictionary<string, int> configKeys, int waitAfterSaveMilliseconds)
        {
            JObject jObject = GetJsonObjectFromFile(path);

            foreach (var configKey in configKeys)
                ReplaceValue(configKey.Key, configKey.Value, jObject);

            await SaveJsonFile(path, waitAfterSaveMilliseconds, jObject).ConfigureAwait(false);
        }

        private static async Task SaveJsonFile(string path, int waitAfterSaveMilliseconds, JObject jObject)
        {
            // Convert the JObject back to a string and save the file.
            await File.WriteAllTextAsync(path, jObject.ToString()).ConfigureAwait(false);

            /// Artificially induced wait so the IOptionsMonitor.OnChange() event handler can pick up file changes
            await Task.Delay(waitAfterSaveMilliseconds).ConfigureAwait(false);
        }

        private static JObject GetJsonObjectFromFile(string path)
        {
            var jsonString = File.ReadAllText(path);
            Assert.NotNull(jsonString);

            return (JObject)JsonConvert.DeserializeObject(jsonString)!;
        }

        static void ReplaceValue(string configKey, int value, JObject jObject)
        {
            var jToken = jObject.SelectToken(configKey);

            Assert.NotNull(jToken);
            // Update the value of the property: 
            jToken.Replace(value);
        }

        #endregion

        [Fact]
        public void LoadFromInitialConfiguration()
        {
            var serviceProvider = GetServiceProvider(TEST_DATA_FILE1);
            var configReader = serviceProvider.GetRequiredService<IConfigReader>();


            Assert.Equal(TestData.USER_BATCH_SIZE, configReader.Get<IUser>().BatchSize);
            Assert.Equal(TestData.GROUP_BATCH_SIZE, configReader.Get<IGroup>().BatchSize);
            Assert.Equal(TestData.PROJECT_BATCH_SIZE, configReader.Get<IProject>().BatchSize);
            Assert.Equal(TestData.WORKBOOK_BATCH_SIZE, configReader.Get<IWorkbook>().BatchSize);
            Assert.Equal(TestData.DATASOURCE_BATCH_SIZE, configReader.Get<IDataSource>().BatchSize);


            var freshConfig = configReader.Get();
            Assert.NotNull(freshConfig?.Network);
            Assert.Equal(TestData.FILE_CHUNK_SIZE_GB, freshConfig.Network.FileChunkSizeKB);
        }

        /// <summary>
        /// This test checks if changes to a config file can be hot-reloaded.
        /// </summary>
        [Theory]
        [InlineData(400, 10)]
        public async Task FileConfigChangeReload(int readDelay, int maxRetries)
        {
            var retries = 0;

            do
            {
                try
                {
                    var serviceProvider = GetServiceProvider(TEST_DATA_FILE2);
                    var configReader = serviceProvider.GetRequiredService<IConfigReader>();

                    var oldConfig = configReader.Get();
                    Assert.NotNull(oldConfig?.Network);
                    Assert.Equal(TestData.FILE_CHUNK_SIZE_GB, oldConfig.Network.FileChunkSizeKB);

                    await EditConfigFile(
                        Path.Combine(Directory.GetCurrentDirectory(), TEST_DATA_DIR, TEST_DATA_FILE2),
                        new Dictionary<string, int> {
                            {$"{nameof(MigrationSdkOptions)}.Network.{nameof(MigrationSdkOptions.Network.FileChunkSizeKB)}",55 },
                            {$"{nameof(MigrationSdkOptions)}.contentTypes[0].batchSize",102 }
                        },
                        readDelay);

                    var newConfig = configReader.Get();
                    var newNetworkConfig = newConfig?.Network;

                    Assert.NotNull(newNetworkConfig);
                    Assert.Equal(55, newNetworkConfig.FileChunkSizeKB);


                    var newUserConfig = newConfig?.ContentTypes.FirstOrDefault(i => i.Type == "User");
                    Assert.NotNull(newUserConfig);
                    Assert.Equal(102, newUserConfig.BatchSize);

                    break;
                }
                catch
                {
                    retries++;
                }
            }
            while (retries < maxRetries);

            if (retries < maxRetries)
            {
                _output.WriteLine($"[INFO FileConfigChangeReload Tests]: Retries count: {retries}.");
                return;
            }

            if (skipGithubRunnerTests)
            {
                _output.WriteLine($"[WARN FileConfigChangeReload Tests]: Max retries ({maxRetries}) reached.");
            }
            else
            {
                Assert.Fail($"[WARN FileConfigChangeReload Tests]: Max retries ({maxRetries}) reached.");
            }
        }

        [Fact]
        public void DefaultsApplied()
        {
            var serviceProvider = GetServiceProvider();
            var configReader = serviceProvider.GetRequiredService<IConfigReader>();

            var freshConfig = configReader.Get();
            Assert.NotNull(freshConfig?.Network);
            Assert.Equal(NetworkOptions.Defaults.FILE_CHUNK_SIZE_KB, freshConfig.Network.FileChunkSizeKB);            
            
            Assert.Equal(ContentTypesOptions.Defaults.BATCH_SIZE, configReader.Get<IUser>().BatchSize);
            Assert.Equal(ContentTypesOptions.Defaults.BATCH_SIZE, configReader.Get<IDataSource>().BatchSize);

            Assert.NotNull(freshConfig?.DefaultPermissionsContentTypes);
        }

        [Fact]
        public void Reads_DefaultPermissionsContentTypes_config()
        {
            var serviceProvider = GetServiceProvider(TEST_DATA_FILE3);
            var configReader = serviceProvider.GetRequiredService<IConfigReader>();
            var config = configReader.Get();
            Assert.NotNull(config?.DefaultPermissionsContentTypes);

            var defaultUrlSegments = DefaultPermissionsContentTypeUrlSegments.GetAll();
            var configSegments = config.DefaultPermissionsContentTypes.UrlSegments;

            Assert.True(configSegments.SequenceEqual(defaultUrlSegments.Concat(new[] { "test1", "test2" })));
        }
    }
}
