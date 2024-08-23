using FluentValidation;
using MessageSilo.Application.DTOs;
using MessageSilo.Domain.Entities;

namespace MessageSilo.Application.Interfaces
{
    public interface IEnricherValidator : IValidator<EnricherDTO>
    {
        Task<IEnumerable<ValidationFailure>> ValidateAsync(EnricherDTO instance);
    }
}
