using Newtonsoft.Json;
using System.Collections;
using System.Reflection;

namespace EzDotNetty.Table
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : Attribute
    {
        public string Path { get; private set; }

        public TableAttribute(string path)
        {
            Path = path;
        }
    }

    public abstract class BaseTable
    {
        protected string Path => GetType().GetCustomAttribute<TableAttribute>()?.Path ??
            throw new Exception("cannot find TableAttribute.");

        protected BaseTable()
        {
            Load();
        }

        protected abstract void Load();
    }

    public class BaseList<T> : BaseTable, IReadOnlyList<T> where T : class
    {
        private List<T?>? _values = new();

        public T? this[int index]
        {
#pragma warning disable CS8766 // 반환 형식에서 참조 형식의 null 허용 여부가 암시적으로 구현된 멤버와 일치하지 않음(null 허용 여부 특성 때문일 수 있음)
            get
#pragma warning restore CS8766 // 반환 형식에서 참조 형식의 null 허용 여부가 암시적으로 구현된 멤버와 일치하지 않음(null 허용 여부 특성 때문일 수 있음)
            {
                if (index > _values!.Count - 1)
                    return null;

                return _values[index];
            }
        }

        public int Count => _values!.Count;

        public IEnumerator<T> GetEnumerator() => _values!.GetEnumerator();

        protected override void Load()
        {
            _values = JsonConvert.DeserializeObject<List<T?>>(File.ReadAllText(Path));
        }

        IEnumerator IEnumerable.GetEnumerator() => _values!.GetEnumerator();
    }

    public class BaseDict<K, T> : BaseTable, IReadOnlyDictionary<K, T> where T : class where K : notnull
    {
        private Dictionary<K, T?>? _dictionary = new();

        public T? this[K key]
        {
#pragma warning disable CS8766 // 반환 형식에서 참조 형식의 null 허용 여부가 암시적으로 구현된 멤버와 일치하지 않음(null 허용 여부 특성 때문일 수 있음)
            get
#pragma warning restore CS8766 // 반환 형식에서 참조 형식의 null 허용 여부가 암시적으로 구현된 멤버와 일치하지 않음(null 허용 여부 특성 때문일 수 있음)
            {
                return _dictionary!.TryGetValue(key, out var value) ? value : null;
            }
        }

        public IEnumerable<K> Keys => _dictionary!.Keys;

#pragma warning disable CS8613 // 반환 형식에 있는 참조 형식 Null 허용 여부가 암시적으로 구현된 멤버와 일치하지 않습니다.
        public IEnumerable<T?> Values => _dictionary!.Values;
#pragma warning restore CS8613 // 반환 형식에 있는 참조 형식 Null 허용 여부가 암시적으로 구현된 멤버와 일치하지 않습니다.

        public int Count => _dictionary!.Count;

        public bool ContainsKey(K key) => _dictionary!.ContainsKey(key);

        public IEnumerator<KeyValuePair<K, T>> GetEnumerator() => _dictionary!.GetEnumerator();

#pragma warning disable CS8767 // 반환 형식에서 참조 형식의 null 허용 여부가 암시적으로 구현된 멤버와 일치하지 않음(null 허용 여부 특성 때문일 수 있음)
        public bool TryGetValue(K key, out T? value) => _dictionary!.TryGetValue(key, out value);
#pragma warning restore CS8767 // 반환 형식에서 참조 형식의 null 허용 여부가 암시적으로 구현된 멤버와 일치하지 않음(null 허용 여부 특성 때문일 수 있음)

        protected override void Load()
        {
            var property = typeof(T).GetProperties().FirstOrDefault(x => x.Name == "Id");

            var deserialized = JsonConvert.DeserializeObject<List<T?>>(File.ReadAllText(Path));
            if(deserialized != null)
            {
                _dictionary = deserialized.ToDictionary(x => ((K?)property!.GetValue(x))!, x => x);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => _dictionary!.GetEnumerator();
    }

    public static class MasterTable
    {
        private static Dictionary<Type, BaseTable?>? _loadedTableDict = new();

        static MasterTable()
        {
        }

        public static void Load(string assemblyName)
        {
            var assembly = string.IsNullOrEmpty(assemblyName) ?
                Assembly.GetEntryAssembly() :
                AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => {
                    var name = x.GetName().Name;
                    return name != null && assemblyName.StartsWith(name);
                });

            _loadedTableDict = assembly!.GetTypes()
                .Where(x => x.IsSubclassOf(typeof(BaseTable)) && x.Namespace == assemblyName)
                .ToDictionary(x => x, x =>
                {
                    var instance = Activator.CreateInstance(x);
                    return instance as BaseTable;
                });
        }

        public static T? From<T>() where T : BaseTable
        {
            if (_loadedTableDict!.TryGetValue(typeof(T), out var value) == false)
                return null;

            return value as T;
        }
    }
}
