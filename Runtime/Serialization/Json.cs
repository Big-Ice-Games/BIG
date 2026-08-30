using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BIG
{
    /// <summary>
    /// JSON serialization for configs, saves and other non-realtime data.
    /// For realtime networking use the deterministic binary <see cref="Serializer"/> instead.
    ///
    /// Converter pattern (works across the whole BIG ecosystem, including downstream libraries like BIG.Deterministic):
    /// - Types WE OWN carry their converter themselves: a nested class in the Serialization/Json region of the struct
    ///   plus [Newtonsoft.Json.JsonConverter(typeof(JsonConverter))] on the type. Newtonsoft discovers it automatically —
    ///   no registration anywhere, any library can add its own types this way.
    /// - Types we DO NOT own (third-party, e.g. FixedMathSharp) cannot be decorated — register their converter once
    ///   at bootstrap via <see cref="RegisterConverter"/>.
    /// </summary>
    public static class Json
    {
        private static readonly object LOCK = new object();

        private static readonly JsonSerializerSettings SETTINGS = new JsonSerializerSettings
        {
            Converters = { new StringEnumConverter() }
        };

        /// <summary>
        /// Register converter for a type that cannot carry the [JsonConverter] attribute itself (third-party types).
        /// Call once during bootstrap, before the first serialization. Registering the same converter type again is ignored.
        /// </summary>
        public static void RegisterConverter(JsonConverter converter)
        {
            lock (LOCK)
            {
                var type = converter.GetType();
                foreach (var registered in SETTINGS.Converters)
                {
                    if (registered.GetType() == type)
                        return;
                }

                SETTINGS.Converters.Add(converter);
            }
        }

        public static string SerializeJson<T>(this T obj)
        {
            if (obj == null) return string.Empty;
            return JsonConvert.SerializeObject(obj, SETTINGS);
        }

        public static T DeserializeJson<T>(this string json)
        {
            if (string.IsNullOrEmpty(json)) return default!;
            return JsonConvert.DeserializeObject<T>(json, SETTINGS)!;
        }
    }
}
