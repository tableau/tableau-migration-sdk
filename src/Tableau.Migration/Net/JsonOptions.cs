using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tableau.Migration.Net
{
    internal static class JsonOptions
    {
        public static JsonSerializerOptions Default = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters =
            {
                new JsonStringEnumConverter()
            }
        };
    }
}
