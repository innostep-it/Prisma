using System.Text.Json;
using System.Text.Json.Serialization;

namespace Prisma.EventManagement.Configuration;

internal static class JsonConfig
{
    internal static JsonSerializerOptions DefaultOptions => new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        }
    };
}