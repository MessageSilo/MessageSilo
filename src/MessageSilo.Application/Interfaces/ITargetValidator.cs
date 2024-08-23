using FluentValidation;
using MessageSilo.Application.DTOs;
using MessageSilo.Domain.Entities;

namespace MessageSilo.Application.Interfaces
{
    public interface ITargetValidator : IValidator<TargetDTO>
    {
        Task<IEnumerable<ValidationFailure>> ValidateAsync(TargetDTO instance);
    }
}
