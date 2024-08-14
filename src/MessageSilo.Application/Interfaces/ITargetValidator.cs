using FluentValidation;
using MessageSilo.Application.DTOs;

namespace MessageSilo.Application.Interfaces
{
    public interface ITargetValidator : IValidator<TargetDTO>
    {
    }
}
