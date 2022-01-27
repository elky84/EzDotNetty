using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Runtime.Caching;

namespace EzDotNetty.Util
{
    public static class JsonUtil
    {
        private static JsonSerializer JsonSerializer = new JsonSerializer();

        static public T Populate<T>(this JObject extensionData) where T : new()
        {
            var value = new T();
            if (extensionData != null)
            {
                JsonSerializer.Populate(extensionData.CreateReader(), value);
            }
            return value;
        }

    }
}
