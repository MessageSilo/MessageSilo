namespace MessageSilo.Shared.Models
{
    public class EntityManagerState
    {
        public Dictionary<int, double> Throughput { get; set; } = new Dictionary<int, double>();
    }
}
