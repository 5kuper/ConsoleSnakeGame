using System.Reflection;

namespace ConsoleSnakeGame.Core.Gridwork.ObstaclePlacements
{
    using AttrInfos = IEnumerable<(OPAttribute Instance, Type Target)>;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    internal class OPAttribute : Attribute
    {
        public static readonly Lazy<AttrInfos> Items = new(() =>
        {
            return from assy in AppDomain.CurrentDomain.GetAssemblies()
                   from type in assy.GetTypes().Where(t => t.IsSubclassOf(typeof(ObstaclePlacement)))
                   from attr in type.GetCustomAttributes<OPAttribute>()
                   select (attr, type);
        });

        public OPAttribute(string name, params object[] ctorArgs)
        {
            Name = !string.IsNullOrWhiteSpace(name) ? name : throw
                new ArgumentException("Name cannot be null or whitespace.", nameof(name));

            CtorArgs = ctorArgs ?? throw new ArgumentNullException(nameof(ctorArgs));
        }

        public string Name { get; }
        public object[] CtorArgs { get; }

        public static ObstaclePlacement Create(string name)
        {
            var attr = Items.Value.FirstOrDefault(i => i.Instance.Name == name);

            if (attr == default)
                throw new ArgumentException("ObstaclePlacement with that name was not found.", nameof(name));

            return (ObstaclePlacement)Activator.CreateInstance(attr.Target, attr.Instance.CtorArgs)!;
        }

        public static ObstaclePlacement Create(IEnumerable<string> names)
        {
            if (names?.Any() == false)
                throw new ArgumentException("Names cannot be null or empty.", nameof(names));

            ObstaclePlacement first = null!, last = null!;

            foreach (var name in names!)
            {
                var op = Create(name);

                if (first is null) first = op;
                else last.TryInit(op);

                last = op;
            }

            return first;
        }
    }
}
