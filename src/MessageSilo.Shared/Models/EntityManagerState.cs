namespace MessageSilo.Shared.Models
{
    [GenerateSerializer]
    public class EntityManagerState
    {
        [Id(0)]
        public List<Entity> Entities { get; set; } = new();
    }
}
