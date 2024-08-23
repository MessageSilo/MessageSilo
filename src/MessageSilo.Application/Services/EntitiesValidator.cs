using FluentValidation;
using MessageSilo.Domain.Entities;

namespace MessageSilo.Application.Services
{
    public class EntitiesValidator : ValidatorBase<IEnumerable<Entity>>
    {
        public EntitiesValidator(IEnumerable<Entity> entities) : base()
        {
            RuleForEach(p => p)
                .Must(p => entities.Count(e => e.Name == p.Name) == 1)
                .WithMessage(p => "Entities must have unique names.");
        }
    }
}
