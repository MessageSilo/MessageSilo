using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;
using MessageSilo.Application.DTOs;

namespace MessageSilo.Client.Models
{
    public class EnricherNode : NodeModel
    {
        public EnricherDTO DTO { get; set; }

        public EnricherNode(EnricherDTO dto, string name, Point? position = null) : base(name, position)
        {
            DTO = dto;
        }
    }
}
