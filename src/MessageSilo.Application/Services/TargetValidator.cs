using FluentValidation;
using MessageSilo.Application.DTOs;
using MessageSilo.Application.Interfaces;
using MessageSilo.Domain.Entities;
using MessageSilo.Domain.Enums;

namespace MessageSilo.Application.Services
{
    public class TargetValidator : ValidatorBase<TargetDTO>, ITargetValidator
    {
        public TargetValidator(IEnumerable<Entity> entities)
        {
            RuleFor(p => p.UserId).NotEmpty().WithName("UserId");

            RuleFor(p => p.Name)
                .NotEmpty()
                .MaximumLength(20)
                .Matches("^[a-zA-Z0-9$_-]+$")
                .Must((e, x) => isUnique(entities, e)).WithMessage(p => $"Entity with name '{p.Name}' already exist")
                .WithName("Name");

            RuleFor(p => p.Type).NotEmpty();

            RuleFor(p => p.Url).NotEmpty()
                .When(p => p.Type == TargetType.API);

            RuleFor(p => p.Endpoint).NotEmpty()
                .When(p => p.Type == TargetType.Azure_EventGrid);

            RuleFor(p => p.AccessKey).NotEmpty()
                .When(p => p.Type == TargetType.Azure_EventGrid);
        }

        private bool isUnique(IEnumerable<Entity> entities, TargetDTO entity) => entities.Count(p => p.Id == entity.Id) == 1;
    }
}
