namespace MessageSilo.Shared.Models
{
    [GenerateSerializer]
    public class ApplyDTO
    {
        [Id(0)]
        public List<TargetDTO> Targets { get; set; } = new();

        [Id(1)]
        public List<EnricherDTO> Enrichers { get; set; } = new();

        [Id(2)]
        public List<ConnectionSettingsDTO> Connections { get; set; } = new();

        [Id(3)]
        public int Scale { get; set; } = 1;

        public ApplyDTO() { }
    }
}
