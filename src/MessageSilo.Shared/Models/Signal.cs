using MessageSilo.Shared.Enums;
using Microsoft.Extensions.Logging;

namespace MessageSilo.Shared.Models
{
    public record Signal(string EntityId, SignalType Type, LogLevel LogLevel, string LogMessage);
}
