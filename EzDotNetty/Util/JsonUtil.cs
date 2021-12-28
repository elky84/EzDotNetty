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

        static public List<T>? DeserializeToList<T>(this string path) where T : class
        {
            ObjectCache cache = MemoryCache.Default;
            if (cache[path] is not List<T> list)
            {
                var deserializedList = JsonConvert.DeserializeObject<List<T>>(File.ReadAllText(path));
                if (deserializedList != null)
                {
                    var filePaths = new List<string> { path };
                    var policy = new CacheItemPolicy();
                    policy.ChangeMonitors.Add(new HostFileChangeMonitor(filePaths));
                    cache.Set(path, deserializedList, policy);
                }
                return deserializedList;
            }
            else
                return list;
        }
    }
}
