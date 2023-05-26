using System.Text.Json;

namespace Example.Application.Helpers
{
    public static class UtilHelper
    {

        public static string Serialize<TValue>(TValue value)
        {
            var jsonOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            return JsonSerializer.Serialize(value, jsonOptions);
        }
    }
}
