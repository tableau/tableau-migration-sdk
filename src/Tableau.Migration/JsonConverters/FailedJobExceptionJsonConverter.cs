using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tableau.Migration.Api.Models;

namespace Tableau.Migration.JsonConverters
{
    /// <summary>
    /// Provides a custom JSON converter for <see cref="FailedJobException"/> objects, allowing for custom serialization and deserialization logic.
    /// </summary>
    public class FailedJobExceptionJsonConverter : JsonConverter<FailedJobException>
    {
        /// <summary>
        /// Reads and converts the JSON to type <see cref="FailedJobException"/>.
        /// </summary>
        /// <param name="reader">The reader to deserialize objects or value types.</param>
        /// <param name="typeToConvert">The type of object to convert.</param>
        /// <param name="options">Options to control the behavior during reading.</param>
        /// <returns>A <see cref="FailedJobException"/> object deserialized from JSON.</returns>
        public override FailedJobException Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            IJob? failedJob = null;
            string? exceptionMessage = null;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var propertyName = reader.GetString();
                    reader.Read(); // Move to the property value.

                    switch (propertyName)
                    {
                        case nameof(FailedJobException.FailedJob):
                            failedJob = JsonSerializer.Deserialize<IJob>(ref reader, options);
                            break;

                        case nameof(FailedJobException.Message):
                            exceptionMessage = reader.GetString();
                            break;
                    }
                }
                else if (reader.TokenType == JsonTokenType.EndObject)
                {
                    break; // End of the object.
                }
            }

            Guard.AgainstNull(exceptionMessage, nameof(exceptionMessage));
            Guard.AgainstNull(failedJob, nameof(failedJob));

            return new FailedJobException(failedJob, exceptionMessage);
        }

        /// <summary>
        /// Writes a specified <see cref="FailedJobException"/> object to JSON.
        /// </summary>
        /// <param name="writer">The writer to serialize objects or value types.</param>
        /// <param name="value">The <see cref="FailedJobException"/> value to serialize.</param>
        /// <param name="options">Options to control the behavior during writing.</param>
        public override void Write(Utf8JsonWriter writer, FailedJobException value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WritePropertyName(nameof(FailedJobException.FailedJob));
            JsonSerializer.Serialize(writer, value.FailedJob, options);

            writer.WriteString(nameof(FailedJobException.Message), value.Message);

            writer.WriteEndObject();
        }
    }
}
