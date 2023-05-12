using FluentValidation;
using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageSilo.Shared.Validators
{
    public class EnricherValidator : AbstractValidator<EnricherDTO>
    {
        public EnricherValidator()
        {
            RuleFor(p => p.PartitionKey).NotEmpty().WithName("UserId");
            RuleFor(p => p.RowKey)
                .NotEmpty()
                .MaximumLength(20)
                .Matches("^[a-zA-Z0-9$_-]+$")
                .WithName("Name");
            RuleFor(p => p.Type).NotEmpty();

            RuleFor(p => p.Function).NotEmpty()
                .When(p => p.Type == EnricherType.Inline);

            RuleFor(p => p.Url).NotEmpty()
                .When(p => p.Type == EnricherType.API);
        }
    }
}
