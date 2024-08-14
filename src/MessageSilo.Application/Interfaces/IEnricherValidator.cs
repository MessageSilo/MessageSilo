using FluentValidation;
using MessageSilo.Application.DTOs;

namespace MessageSilo.Application.Interfaces
{
    public interface IEnricherValidator : IValidator<EnricherDTO>
    {
    }
}
