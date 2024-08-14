namespace MessageSilo.Application.DTOs
{
    public class ApplyDTO
    {
        public List<TargetDTO> Targets { get; set; } = [];

        public List<EnricherDTO> Enrichers { get; set; } = [];

        public List<ConnectionSettingsDTO> Connections { get; set; } = [];

        public int Scale { get; set; } = 1;
    }
}
