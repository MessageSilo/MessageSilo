using FluentValidation;
using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Models;

namespace MessageSilo.Shared.Validators
{
    public class EnricherValidator : AbstractValidator<EnricherDTO>
    {
        public EnricherValidator(IEnumerable<Entity> entities)
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

        private bool isUnique(IEnumerable<Entity> entities, EnricherDTO entity) => !entities.Any(p => p.Id == entity.Id && p.Kind != entity.Kind);
    }
}
