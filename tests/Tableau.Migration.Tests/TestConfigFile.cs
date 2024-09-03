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
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Tableau.Migration.Tests
{
    public sealed class TestConfigFile : IAsyncDisposable
    {
        private static readonly string DefaultFolderPath = Path.Combine(Environment.CurrentDirectory, "test-config");
        private readonly string DefaultFileName = $"{Guid.NewGuid()}.json";

        public readonly string FilePath;

        public TestConfigFile(string? filePath = null)
        {
            var folderPath = Path.GetDirectoryName(filePath) ?? DefaultFolderPath;
            var fileName = Path.GetFileName(filePath) ?? DefaultFileName;

            FilePath = Path.GetFullPath(Path.Combine(folderPath, fileName));

            Directory.CreateDirectory(folderPath);
        }

        public static async Task<TestConfigFile> FromContentAsync(string content, CancellationToken cancel)
        {
            var file = new TestConfigFile();
            await file.WriteAsync(content, cancel).ConfigureAwait(false);
            return file;
        }

        public async Task<string> ReadAsync(CancellationToken cancel) 
            => await File.ReadAllTextAsync(FilePath, Constants.DefaultEncoding, cancel).ConfigureAwait(false);

        public async Task WriteAsync(string content, CancellationToken cancel) 
            => await File.WriteAllTextAsync(FilePath, content, Constants.DefaultEncoding, cancel).ConfigureAwait(false);

        public async Task WriteAsync(dynamic content, CancellationToken cancel) 
            => await WriteAsync(JsonSerializer.Serialize(content), cancel).ConfigureAwait(false);

        public async Task EditAsync(Action<JsonNode> edit, CancellationToken cancel)
        {
            var content = await ReadAsync(cancel).ConfigureAwait(false);
            var json = JsonNode.Parse(content, new JsonNodeOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(json);

            edit(json);

            await WriteAsync(json, cancel).ConfigureAwait(false);
        }

        public async Task EditAsync(string configKey, object? value, CancellationToken cancel)
            => await EditAsync(new Dictionary<string, object?> { [configKey] = value }, cancel).ConfigureAwait(false);

        public async Task EditAsync(IDictionary<string, object?> configElements, CancellationToken cancel)
        {
            var content = await ReadAsync(cancel);

            var json = JsonNode.Parse(content);

            Assert.NotNull(json);

            foreach (var configElement in configElements)
            {
                var jsonElement = json.GetByPath(configElement.Key, true);

                // Update the value of the property: 
                jsonElement.ReplaceWith(configElement.Value);
            }

            // Convert the json object back to a string:
            string newContent = json.ToString();

            await WriteAsync(newContent, cancel).ConfigureAwait(false);
        }

        #region - IAsyncDisposable -

        public ValueTask DisposeAsync()
        {
            File.Delete(FilePath);
            return ValueTask.CompletedTask;
        }

        #endregion
    }
}
