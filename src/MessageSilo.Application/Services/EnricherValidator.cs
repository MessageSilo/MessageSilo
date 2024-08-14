using FluentValidation;
using MessageSilo.Application.DTOs;
using MessageSilo.Application.Interfaces;
using MessageSilo.Domain.Entities;
using MessageSilo.Domain.Enums;

namespace MessageSilo.Application.Services
{
    public class EnricherValidator : AbstractValidator<EnricherDTO>, IEnricherValidator
    {
        public EnricherValidator(IEnumerable<Entity> entities) : base()
        {
            RuleFor(p => p.UserId).NotEmpty().WithName("UserId");

            RuleFor(p => p.Name)
                .NotEmpty()
                .MaximumLength(20)
                .Matches("^[a-zA-Z0-9$_-]+$")
                .Must((e, x) => isUnique(entities, e)).WithMessage(p => $"Entity with name '{p.Name}' already exist")
                .WithName("Name");

            RuleFor(p => p.Type).NotEmpty();

            RuleFor(p => p.Function).NotEmpty()
                .When(p => p.Type == EnricherType.Inline);

            RuleFor(p => p.Url).NotEmpty()
                .When(p => p.Type == EnricherType.API);

            RuleFor(p => p.Command).NotEmpty()
                .When(p => p.Type == EnricherType.AI);
        }

        private bool isUnique(IEnumerable<Entity> entities, EnricherDTO entity) => entities.Count(p => p.Id == entity.Id) == 1;
    }

}
