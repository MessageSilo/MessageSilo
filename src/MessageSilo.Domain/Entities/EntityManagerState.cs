namespace MessageSilo.Domain.Entities
{
    public class EntityManagerState
    {
        public List<Entity> Entities { get; set; } = [];

        public int Scale { get; set; } = 1;
    }
}
