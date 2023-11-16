using MessageSilo.Shared.Enums;
using YamlDotNet.Serialization;

namespace MessageSilo.Shared.Models
{
    [GenerateSerializer]
    public class Entity : IComparable<Entity>
    {
        [Id(0)]
        public string UserId { get; set; }

        [Id(1)]
        public string Name { get; set; }

        [Id(2)]
        public EntityKind Kind { get; set; }

        public string YamlDefinition { get; set; }

        [YamlIgnore]
        public string Id => $"{UserId}|{Name}";

        public int CompareTo(Entity? other)
        {
            var result = this.Id.CompareTo(other?.Id);

            if (result == 0)
                result = this.Kind.CompareTo(other?.Kind);

            return result;
        }
    }
}
