using FluentValidation;
using MessageSilo.Application.DTOs;
using MessageSilo.Domain.Entities;

namespace MessageSilo.Application.Interfaces
{
    public interface IConnectionValidator : IValidator<ConnectionSettingsDTO>
    {
        Task<IEnumerable<ValidationFailure>> ValidateAsync(ConnectionSettingsDTO instance);
    }
}
