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

using System.Text.Json;
using System.Text.Json.Serialization;
using Tableau.Migration.Engine.Manifest;

namespace Tableau.Migration.TestComponents.JsonConverters
{
    public class MigrationManifestEntryCollectionWriter : JsonConverter<IMigrationManifestEntryCollection>
    {
        public override void Write(Utf8JsonWriter writer, IMigrationManifestEntryCollection value, JsonSerializerOptions options)
        {
            writer.WriteStartArray(); // Start an array of all entries

            foreach (var partitionType in value.GetPartitionTypes())
            {
                writer.WriteStartObject(); // Each partion is an object in the array of entries
                writer.WriteString(Constants.PARTITION, partitionType.FullName); // Write the type of the partion

                writer.WritePropertyName(Constants.ENTRIES); // Entries per partion (yes, entries is overloaded here but naming is hard)
                writer.WriteStartArray(); // Start the array of entries per partition

                var entries = value.ForContentType(partitionType);
                foreach (var entry in entries)
                {
                    JsonSerializer.Serialize(writer, entry, options); // Each individual entry can be reflected, so we can let the JsonSerializer handle it
                }

                writer.WriteEndArray(); // End of the array of entries per partion
                writer.WriteEndObject(); // End of partion object
            }

            writer.WriteEndArray(); // End of array of all entries
        }

        #region - Not Implemented -

        public override MigrationManifestEntryCollection? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException($"{nameof(MigrationManifestEntryCollectionWriter)} only supports serialization of {nameof(MigrationManifest)}s. Use {nameof(MigrationManifestEntryCollectionReader)} to deserialize");
        }

        #endregion

    }
}
