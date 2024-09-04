using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;
using MessageSilo.Application.DTOs;

namespace MessageSilo.Client.Models
{
    public class TargetNode : NodeModel
    {
        public TargetDTO DTO { get; set; }

        public TargetNode(TargetDTO dto, string name, Point? position = null) : base(name, position)
        {
            DTO = dto;
        }
    }
}
