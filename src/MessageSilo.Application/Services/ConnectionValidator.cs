using FluentValidation;
using MessageSilo.Application.DTOs;
using MessageSilo.Application.Interfaces;
using MessageSilo.Domain.Enums;

namespace MessageSilo.Application.Services
{
    public class ConnectionValidator : ValidatorBase<ConnectionSettingsDTO>, IConnectionValidator
    {
        public ConnectionValidator() : base()
        {
            RuleFor(p => p.UserId).NotEmpty().WithName("UserId");

            RuleFor(p => p.Name)
                .NotEmpty()
                .MaximumLength(20)
                .Matches("^[a-zA-Z0-9$_-]+$")
                .WithName("Name");

            RuleFor(p => p.Type).NotEmpty();

            RuleFor(p => p.ReceiveMode).NotEmpty();

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
    }
}
