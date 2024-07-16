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
using System.IO.Abstractions;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.JsonConverters;
using Tableau.Migration.JsonConverters.SerializableObjects;
using Tableau.Migration.Resources;


namespace Tableau.Migration.Engine.Manifest
{
    /// <summary>
    /// Provides functionality to serialize and deserialize migration manifests in JSON format.
    /// </summary>
    public class MigrationManifestSerializer
    {
        private readonly IFileSystem _fileSystem;
        private readonly ISharedResourcesLocalizer _localizer;
        private readonly ILoggerFactory _loggerFactory;

        private readonly ImmutableArray<JsonConverter> _converters;

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationManifestSerializer"/> class.
        /// </summary>
        public MigrationManifestSerializer(IFileSystem fileSystem, ISharedResourcesLocalizer localizer, ILoggerFactory loggerFactory)
        {
            _fileSystem = fileSystem;
            _localizer = localizer;
            _loggerFactory = loggerFactory;

            _converters = CreateConverters();
        }

        /// <summary>
        /// This is the current MigrationManifest.ManifestVersion that this serializer supports.
        /// </summary>
        public const uint SupportedManifestVersion = MigrationManifest.LatestManifestVersion;


        /// <summary>
        /// Creates the list of JSON converters used by the MigrationManifestSerializer.
        /// </summary>
        /// <remarks>This is a static method so the tests can use the same list converters.</remarks>
        /// <returns>An immutable array of JSON converters.</returns>
        internal static ImmutableArray<JsonConverter> CreateConverters()
        {
            return new JsonConverter[]
            {
                new PythonExceptionConverter(),
                new SerializedExceptionJsonConverter(),
                new BuildResponseExceptionJsonConverter(),
                new JobJsonConverter(),
                new TimeoutJobExceptionJsonConverter(),
                new RestExceptionJsonConverter(),
                new FailedJobExceptionJsonConverter(),
                new ExceptionJsonConverterFactory(),    // This needs to be at the end. This list is ordered.
            }.ToImmutableArray();
        }

        private JsonSerializerOptions MergeJsonOptions(JsonSerializerOptions? jsonOptions)
        {
            jsonOptions ??= new() { WriteIndented = true };

            foreach (var converter in _converters)
            {
                jsonOptions.Converters.Add(converter);
            }

            return jsonOptions;
        }

        /// <summary>
        /// Saves a manifest in JSON format.
        /// </summary>
        /// <remarks>This async function does not take a cancellation token. This is because the saving should happen, 
        /// no matter what the status of the cancellation token is. Otherwise the manifest is not saved if the migration is cancelled.</remarks>
        /// <param name="manifest">The manifest to save.</param>
        /// <param name="path">The file path to save the manifest to.</param>
        /// <param name="jsonOptions">Optional JSON options to use.</param>
        public async Task SaveAsync(IMigrationManifest manifest, string path, JsonSerializerOptions? jsonOptions = null)
        {
            jsonOptions = MergeJsonOptions(jsonOptions);

            var dir = Path.GetDirectoryName(path);
            if (dir is not null && !_fileSystem.Directory.Exists(dir))
            {
                _fileSystem.Directory.CreateDirectory(dir);
            }

            var serializableManifest = new SerializableMigrationManifest(manifest);

            var file = _fileSystem.File.Create(path);
            await using (file.ConfigureAwait(false))
            {
                // If cancellation was requested, we still need to save the file, so use the default token.
                await JsonSerializer.SerializeAsync(file, serializableManifest, jsonOptions, default)
                    .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Loads a manifest from JSON format.
        /// </summary>
        /// <param name="path">The file path to load the manifest from.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <param name="jsonOptions">Optional JSON options to use.</param>
        /// <returns>The loaded <see cref="MigrationManifest"/>, or null if the manifest could not be loaded.</returns>
        public async Task<MigrationManifest?> LoadAsync(string path, CancellationToken cancel, JsonSerializerOptions? jsonOptions = null)
        {
            if (!_fileSystem.File.Exists(path))
            {
                return null;
            }

            jsonOptions = MergeJsonOptions(jsonOptions);

            var file = _fileSystem.File.OpenRead(path);
            await using (file.ConfigureAwait(false))
            {
                var manifest = await JsonSerializer.DeserializeAsync<SerializableMigrationManifest>(file, jsonOptions, cancel)
                    .ConfigureAwait(false);

                if (manifest is not null)
                {
                    if (manifest.ManifestVersion is not SupportedManifestVersion)
                        throw new NotSupportedException($"This {nameof(MigrationManifestSerializer)} only supports Manifest version {SupportedManifestVersion}. The manifest being loaded is version {manifest.ManifestVersion}");

                    return manifest.ToMigrationManifest(_localizer, _loggerFactory) as MigrationManifest;
                }

                return null;
            }
        }
    }
}
