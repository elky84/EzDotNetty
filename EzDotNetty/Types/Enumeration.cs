using System.Reflection;

namespace EzDotNetty.Types
{
    // [열거형 형식 대신 열거형 클래스 사용 | Microsoft Docs] (https://docs.microsoft.com/ko-kr/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/enumeration-classes-over-enum-types)
    public abstract class Enumeration : IComparable
    {
        // ReSharper disable once MemberCanBePrivate.Global
        public string Name { get; }

        // ReSharper disable once MemberCanBePrivate.Global
        public int Id { get; set; }

        protected Enumeration(int id, string name) => (Id, Name) = (id, name);

        public override string ToString() => Name;

        // ReSharper disable once MemberCanBePrivate.Global
        public static IEnumerable<T> GetAll<T>() where T : Enumeration =>
            typeof(T).GetFields(BindingFlags.Public |
                                BindingFlags.Static |
                                BindingFlags.DeclaredOnly)
                     .Select(f => f.GetValue(null))
                     .Cast<T>();

        public override bool Equals(object? obj)
        {
            if (obj is not Enumeration otherValue)
            {
                return false;
            }

            var typeMatches = GetType() == obj.GetType();
            var valueMatches = Id.Equals(otherValue.Id);

            return typeMatches && valueMatches;
        }

        public int CompareTo(object? other) => Id.CompareTo(((Enumeration)other!).Id);

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public static T? Get<T>(string name) where T : Enumeration => 
            GetAll<T>().FirstOrDefault(x => x.Name == name);

        // Other utility methods ...
    }
}
