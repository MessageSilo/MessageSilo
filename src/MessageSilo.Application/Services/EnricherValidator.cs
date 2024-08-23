using FluentValidation;
using MessageSilo.Application.DTOs;
using MessageSilo.Application.Interfaces;
using MessageSilo.Domain.Enums;

namespace MessageSilo.Application.Services
{
    public class EnricherValidator : ValidatorBase<EnricherDTO>, IEnricherValidator
    {
        public EnricherValidator() : base()
        {
            RuleFor(p => p.UserId).NotEmpty().WithName("UserId");

            RuleFor(p => p.Name)
                .NotEmpty()
                .MaximumLength(20)
                .Matches("^[a-zA-Z0-9$_-]+$")
                .WithName("Name");

            RuleFor(p => p.Type).NotEmpty();

            RuleFor(p => p.Function).NotEmpty()
                .When(p => p.Type == EnricherType.Inline);

            RuleFor(p => p.Url).NotEmpty()
                .When(p => p.Type == EnricherType.API);

            RuleFor(p => p.Command).NotEmpty()
                .When(p => p.Type == EnricherType.AI);
        }
    }

}
