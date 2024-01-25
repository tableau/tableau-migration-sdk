using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.Resources;
using Tableau.Migration.TestComponents.JsonConverters.JsonObjects;

namespace Tableau.Migration.TestComponents.JsonConverters
{
    public class MigrationManifestEntryCollectionReader : JsonConverter<IMigrationManifestEntryCollectionEditor>
    {
        private readonly ISharedResourcesLocalizer _localizer;
        private readonly ILoggerFactory _loggerFactory;

        public MigrationManifestEntryCollectionReader(ISharedResourcesLocalizer localizer, ILoggerFactory loggerFactory)
        {
            _localizer = localizer;
            _loggerFactory = loggerFactory;
        }

        public override MigrationManifestEntryCollection? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Get the assembly name of a well known Tableau.Migration Type. This is required later to get the type from a string
            var assemblyName = typeof(MigrationManifestEntryCollection).Assembly.FullName;
            MigrationManifestEntryCollection collection = new(_localizer, _loggerFactory);

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    // Once we reach a property name, we can dive in and deserialize based on the property type
                    case JsonTokenType.PropertyName:
                        var propName = reader.GetString();
                        if (propName == Constants.PARTITION)
                        {
                            // We're in a known property, read the next token which should be string representing the property type
                            reader.Read();

                            // Translate the partion type string into a type object
                            string propTypeStr = reader.GetString() ?? throw new JsonException("Property Type should not be null");
                            Type contentType = Type.GetType($"{propTypeStr}, {assemblyName}") ?? throw new TypeLoadException($"Unable to convert {propTypeStr} to type"); ;

                            MigrationManifestEntryCollectionReader.ReadAndAssertPropertyName(ref reader, Constants.ENTRIES);

                            // Get all the entries and deserialize them
                            var deserializeEntries = JsonSerializer.Deserialize<List<JsonManifestEntry>>(ref reader) ?? throw new JsonException("Unable to deserialize entries");

                            // Get the partition for this Content Type
                            var partition = collection.GetOrCreatePartition(contentType);
                            var partitionAsBuilder = partition as IMigrationManifestEntryBuilder ?? throw new Exception($"Unable to convert partition to {nameof(IMigrationManifestEntryBuilder)}");

                            // Convert all the entries
                            var migrationEntries = deserializeEntries.Select(x => x.AsMigrationManifestEntry(partitionAsBuilder)).ToList();

                            // Add converted entries to partition
                            partition.CreateEntries(migrationEntries);
                        }
                        else
                        {
                            throw new JsonException($"Unknown property {propName}");
                        }
                        break;

                    // We're done with the array, so we're returning out collection
                    case JsonTokenType.EndArray:
                        return collection;

                    // No ops
                    case JsonTokenType.StartArray:
                    case JsonTokenType.StartObject:
                    case JsonTokenType.EndObject:
                    case JsonTokenType.True:
                    case JsonTokenType.False:
                    case JsonTokenType.Null:
                        break;

                    // We should never be hitting a json string or number as the property reader should be doing that
                    // That means we should skip the property 
                    case JsonTokenType.String:
                    case JsonTokenType.Number:
                    default:
                        reader.Skip();
                        break;
                }

            }

            return collection;
        }

        #region - Asserters -

        /// <summary>
        /// Verify that last json token was a <see cref="JsonTokenType.PropertyName"/>
        /// </summary>
        internal static void AssertPropertyName(ref Utf8JsonReader reader)
        {
            if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException("Expected property name");
        }

        /// <summary>
        /// Verify that last json token was a <see cref="JsonTokenType.StartArray"/>
        /// </summary>
        internal static void AssertStartArray(ref Utf8JsonReader reader)
        {
            if (reader.TokenType != JsonTokenType.StartArray) throw new JsonException("Expected start of array");
        }

        /// <summary>
        /// Verify that last json token was a <see cref="JsonTokenType.StartObject"/>
        /// </summary>
        internal static void AssertStartObject(ref Utf8JsonReader reader)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected start of object");
        }

        /// <summary>
        /// Read the next json token and verify that it's a <see cref="JsonTokenType.PropertyName"/>
        /// </summary>
        /// <param name="expected">The expected string value. If null, then it's not checked</param>
        internal static void ReadAndAssertPropertyName(ref Utf8JsonReader reader, string? expected = null)
        {
            reader.Read();
            MigrationManifestEntryCollectionReader.AssertPropertyName(ref reader);
            if (expected != null)
            {
                var actual = reader.GetString();
                string.Equals(actual, expected, StringComparison.Ordinal);
            }
        }

        /// <summary>
        /// Read the next json token and verify that it's a <see cref="JsonTokenType.StartArray"/>
        /// </summary>
        internal static void ReadAndAssertStartArray(ref Utf8JsonReader reader)
        {
            reader.Read();
            MigrationManifestEntryCollectionReader.AssertStartArray(ref reader);
        }

        /// <summary>
        /// Read the next json token and verify that it's a <see cref="JsonTokenType.StartObject"/>
        /// </summary>
        internal static void ReadAndAssertStartObject(ref Utf8JsonReader reader)
        {
            reader.Read();
            MigrationManifestEntryCollectionReader.AssertStartObject(ref reader);
        }

        #endregion

        #region - Not Implemented -
        public override void Write(Utf8JsonWriter writer, IMigrationManifestEntryCollectionEditor value, JsonSerializerOptions options)
        {
            throw new NotImplementedException($"{nameof(MigrationManifestEntryCollectionReader)} only supports deserialization of {nameof(MigrationManifest)}s. Use {nameof(MigrationManifestEntryCollectionWriter)} to serialize");
        }
        #endregion

    }
}
