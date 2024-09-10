using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;
using MessageSilo.Application.DTOs;

namespace MessageSilo.Client.Models
{
    public class ConnectionNode : NodeModel
    {
        public ConnectionSettingsDTO DTO { get; set; }

        public Func<ConnectionSettingsDTO, Task> OnSendClick { get; set; }

        public ConnectionNode(ConnectionSettingsDTO dto, string name, Func<ConnectionSettingsDTO, Task> onSendClick, Point? position = null) : base(name, position)
        {
            DTO = dto;
            OnSendClick = onSendClick;
        }
    }
}
