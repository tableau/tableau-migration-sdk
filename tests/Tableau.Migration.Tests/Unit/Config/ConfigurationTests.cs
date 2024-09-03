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
using System.ComponentModel;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Tableau.Migration.Config;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Permissions;
using Tableau.Migration.Content.Schedules.Server;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Config
{
    [Category("Configuration")]
    public class ConfigurationTests : AutoFixtureTestBase
    {
        protected class TestData
        {
            public const int USER_BATCH_SIZE = 201;
            public const int GROUP_BATCH_SIZE = 202;
            public const int PROJECT_BATCH_SIZE = 203;
            public const int WORKBOOK_BATCH_SIZE = 204;
            public const int DATASOURCE_BATCH_SIZE = 205;
            public const int EXTRACT_REFRESH_TASK_BATCHSIZE = 12;

            public const int FILE_CHUNK_SIZE_GB = 2034;
        }

        [Fact]
        public async Task LoadFromInitialConfiguration()
        {
            var content =
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
        }},
        {{
            ""type"":""ServerExtractRefreshTask"",
            ""batchSize"": {TestData.EXTRACT_REFRESH_TASK_BATCHSIZE}            
        }}
        ],
        ""Network"": {{
            ""{nameof(MigrationSdkOptions.Network.FileChunkSizeKB)}"": {TestData.FILE_CHUNK_SIZE_GB}
        }}
    }}
}}";

            await using var context = await ConfigurationTestContext.FromContentAsync(content, Cancel);

            var loaded = context.GetCurrentConfiguration();

            Assert.Equal(TestData.USER_BATCH_SIZE, context.GetCurrentConfiguration<IUser>().BatchSize);
            Assert.Equal(TestData.GROUP_BATCH_SIZE, context.GetCurrentConfiguration<IGroup>().BatchSize);
            Assert.Equal(TestData.PROJECT_BATCH_SIZE, context.GetCurrentConfiguration<IProject>().BatchSize);
            Assert.Equal(TestData.WORKBOOK_BATCH_SIZE, context.GetCurrentConfiguration<IWorkbook>().BatchSize);
            Assert.Equal(TestData.DATASOURCE_BATCH_SIZE, context.GetCurrentConfiguration<IDataSource>().BatchSize);
            Assert.Equal(TestData.EXTRACT_REFRESH_TASK_BATCHSIZE, context.GetCurrentConfiguration<IServerExtractRefreshTask>().BatchSize);

            Assert.Equal(TestData.FILE_CHUNK_SIZE_GB, loaded.Network.FileChunkSizeKB);
        }

        /// <summary>
        /// This test checks if changes to a config file can be hot-reloaded.
        /// </summary>
        [Fact]
        public async Task FileConfigChangeReload()
        {
            var content =
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

            await using var context = await ConfigurationTestContext.FromContentAsync(content, Cancel);

            var oldConfig = context.GetCurrentConfiguration();

            Assert.Equal(TestData.FILE_CHUNK_SIZE_GB, oldConfig.Network.FileChunkSizeKB);

            int expectedFileChunkSizeKB = TestData.FILE_CHUNK_SIZE_GB + 1;
            int expectedUserBatchSize = TestData.USER_BATCH_SIZE + 1;
            int expectedGroupBatchSize = TestData.GROUP_BATCH_SIZE + 1;

            var newConfig = await context.WaitForUpdateAsync(async () =>
            {
                await context.ConfigFile!.EditAsync(json =>
                {
                    var sdkNode = json.GetByPath(nameof(MigrationSdkOptions), true);

                    sdkNode.ReplaceWith(NameOf.Build<MigrationSdkOptions>(o => o.Network.FileChunkSizeKB), expectedFileChunkSizeKB, true);

                    UpdateContentTypeBatchSize(sdkNode, 0, expectedUserBatchSize);
                    UpdateContentTypeBatchSize(sdkNode, 1, expectedGroupBatchSize);
                },
                Cancel)
                .ConfigureAwait(false);
            },
            TestCancellationTimeout);

            Assert.Equal(expectedFileChunkSizeKB, newConfig.Network.FileChunkSizeKB);
            Assert.Equal(expectedUserBatchSize, newConfig.ContentTypes[0].BatchSize);
            Assert.Equal(expectedGroupBatchSize, newConfig.ContentTypes[1].BatchSize);

            // Unchanged values
            Assert.Equal(oldConfig.ContentTypes[2].BatchSize, newConfig.ContentTypes[2].BatchSize);
            Assert.Equal(oldConfig.ContentTypes[3].BatchSize, newConfig.ContentTypes[3].BatchSize);
            Assert.Equal(oldConfig.ContentTypes[4].BatchSize, newConfig.ContentTypes[4].BatchSize);

            static void UpdateContentTypeBatchSize(JsonNode json, int index, int value)
                => json
                    .GetArrayItemByPath(NameOf.Build<MigrationSdkOptions>(o => o.ContentTypes), index, true)
                    .GetByPath(NameOf.Build<ContentTypesOptions>(o => o.BatchSize), true)
                    .ReplaceWith(value);
        }

        [Fact]
        public async Task DefaultsApplied()
        {
            await using var context = ConfigurationTestContext.WithoutConfigFile();

            var freshConfig = context.GetCurrentConfiguration();

            Assert.Equal(NetworkOptions.Defaults.FILE_CHUNK_SIZE_KB, freshConfig.Network.FileChunkSizeKB);            
            
            Assert.Equal(ContentTypesOptions.Defaults.BATCH_SIZE, context.GetCurrentConfiguration<IUser>().BatchSize);
            Assert.Equal(ContentTypesOptions.Defaults.BATCH_SIZE, context.GetCurrentConfiguration<IDataSource>().BatchSize);
            Assert.Equal(ContentTypesOptions.Defaults.BATCH_SIZE, context.GetCurrentConfiguration<IServerExtractRefreshTask>().BatchSize);

            Assert.NotNull(freshConfig?.DefaultPermissionsContentTypes);
        }

        [Fact]
        public async Task Reads_DefaultPermissionsContentTypes_config()
        {
            var content =
$@"{{
  ""MigrationSdkOptions"": {{
    ""DefaultPermissionsContentTypes"": {{
      ""{nameof(MigrationSdkOptions.DefaultPermissionsContentTypes.UrlSegments)}"": [""test1"", ""test2""]
    }}
  }}
}}";

            await using var context = await ConfigurationTestContext.FromContentAsync(content, Cancel);

            var config = context.GetCurrentConfiguration();

            var defaultUrlSegments = DefaultPermissionsContentTypeUrlSegments.GetAll();
            var configSegments = config.DefaultPermissionsContentTypes.UrlSegments;

            Assert.True(configSegments.SequenceEqual(defaultUrlSegments.Concat(["test1", "test2"])));
        }
    }
}
