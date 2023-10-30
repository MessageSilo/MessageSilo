namespace MessageSilo.Shared.Models
{
    [GenerateSerializer]
    public class EntityManagerState
    {
        [Id(0)]
        public Dictionary<int, double> Throughput { get; set; } = new Dictionary<int, double>();
        [Id(1)]
        public List<Entity> Entities { get; set; } = new();
    }
}
