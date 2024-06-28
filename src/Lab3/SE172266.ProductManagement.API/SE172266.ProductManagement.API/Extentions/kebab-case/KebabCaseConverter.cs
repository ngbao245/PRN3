using System.Text.Json;
using System.Text.Json.Serialization;

namespace SE172266.ProductManagement.API.Extentions.kebab_case
{
    public class KebabCaseConverter<T> : JsonConverter<T>
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
            {
                var json = doc.RootElement.GetRawText();
                return JsonSerializer.Deserialize<T>(json, options);
            }
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, typeof(T), options);
        }
    }
}
