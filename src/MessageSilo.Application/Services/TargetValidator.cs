using FluentValidation;
using MessageSilo.Application.DTOs;
using MessageSilo.Application.Interfaces;
using MessageSilo.Domain.Enums;

namespace MessageSilo.Application.Services
{
    public class TargetValidator : ValidatorBase<TargetDTO>, ITargetValidator
    {
        public TargetValidator()
        {
            RuleFor(p => p.UserId).NotEmpty().WithName("UserId");

            RuleFor(p => p.Name)
                .NotEmpty()
                .MaximumLength(20)
                .Matches("^[a-zA-Z0-9$_-]+$")
                .WithName("Name");

            RuleFor(p => p.Type).NotEmpty();

            RuleFor(p => p.Url).NotEmpty()
                .When(p => p.Type == TargetType.API);

            RuleFor(p => p.Endpoint).NotEmpty()
                .When(p => p.Type == TargetType.Azure_EventGrid);

            RuleFor(p => p.AccessKey).NotEmpty()
                .When(p => p.Type == TargetType.Azure_EventGrid);
        }
    }
}
