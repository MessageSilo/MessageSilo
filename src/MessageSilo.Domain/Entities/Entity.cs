using MessageSilo.Domain.Enums;

namespace MessageSilo.Domain.Entities
{
    [Serializable]
    public class Entity : IComparable<Entity>
    {
        public required string UserId { get; set; }

        public required string Name { get; set; }

        public required EntityKind Kind { get; set; }

        public required string YamlDefinition { get; set; }

        public string Id => $"{UserId}|{Name}";

        public int CompareTo(Entity? other)
        {
            var result = Id.CompareTo(other?.Id);

            if (result == 0)
                result = Kind.CompareTo(other?.Kind);

            return result;
        }
    }
}
