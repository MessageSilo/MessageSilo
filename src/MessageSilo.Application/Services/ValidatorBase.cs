using FluentValidation;
using MessageSilo.Domain.Entities;

namespace MessageSilo.Application.Services
{
    public abstract class ValidatorBase<T> : AbstractValidator<T>
    {
        public async Task<IEnumerable<ValidationFailure>> ValidateAsync(T instance)
        {
            var result = await base.ValidateAsync(instance);

            return result.Errors.Select(p => new ValidationFailure(p.ErrorMessage));
        }
    }
}
