using FluentValidation;
using MessageSilo.Application.DTOs;
using MessageSilo.Application.Interfaces;
using MessageSilo.Domain.Entities;
using MessageSilo.Domain.Enums;

namespace MessageSilo.Application.Services
{
    public class ConnectionValidator : ValidatorBase<ConnectionSettingsDTO>, IConnectionValidator
    {
        public ConnectionValidator(IEnumerable<Entity> entities) : base()
        {
            RuleFor(p => p.UserId).NotEmpty().WithName("UserId");

            RuleFor(p => p.Name)
                .NotEmpty()
                .MaximumLength(20)
                .Matches("^[a-zA-Z0-9$_-]+$")
                .Must((e, x) => isUnique(entities, e)).WithMessage(p => $"Entity with name '{p.Name}' already exist")
                .WithName("Name");

            RuleFor(p => p.Type).NotEmpty();

            RuleFor(p => p.ReceiveMode).NotEmpty();

            RuleFor(p => p.Target)
                .Must(x => isTargetExist(entities, x)).WithMessage(p => $"No Target with name '{p.Target}' found")
                .When(p => !string.IsNullOrEmpty(p.Target));

            RuleForEach(p => p.Enrichers)
                .Must(x => isEnricherExist(entities, x)).WithMessage(p => "Enricher at index '{CollectionIndex}' not found")
                .When(p => p.Enrichers.Count() > 0);

            RuleFor(p => p.QueueName).NotEmpty()
                .When(p => p.Type == MessagePlatformType.Azure_Queue ||
                           p.Type == MessagePlatformType.RabbitMQ ||
                           p.Type == MessagePlatformType.AWS_SQS);

            RuleFor(p => p.ConnectionString).NotEmpty()
                .When(p => p.Type == MessagePlatformType.Azure_Queue ||
                           p.Type == MessagePlatformType.Azure_Topic ||
                           p.Type == MessagePlatformType.RabbitMQ);

            RuleFor(p => p.TopicName).NotEmpty()
                .When(p => p.Type == MessagePlatformType.Azure_Topic);

            RuleFor(p => p.SubscriptionName).NotEmpty()
                .When(p => p.Type == MessagePlatformType.Azure_Topic);

            RuleFor(p => p.Region).NotEmpty()
                .When(p => p.Type == MessagePlatformType.AWS_SQS);

            RuleFor(p => p.AccessKey).NotEmpty()
                .When(p => p.Type == MessagePlatformType.AWS_SQS);

            RuleFor(p => p.SecretAccessKey).NotEmpty()
                .When(p => p.Type == MessagePlatformType.AWS_SQS);
        }

        private bool isUnique(IEnumerable<Entity> entities, ConnectionSettingsDTO entity) => entities.Count(p => p.Id == entity.Id) == 1;

        private bool isTargetExist(IEnumerable<Entity> entities, string targetName) => entities.Any(p => (p.Kind == EntityKind.Target || p.Kind == EntityKind.Connection) && p.Name == targetName);

        private bool isEnricherExist(IEnumerable<Entity> entities, string enricherName) => entities.Any(p => p.Kind == EntityKind.Enricher && p.Name == enricherName);
    }
}
