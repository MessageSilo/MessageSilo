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
    public class ConnectionValidator : AbstractValidator<ConnectionSettingsDTO>
    {
        public ConnectionValidator()
        {
            RuleFor(p => p.PartitionKey).NotEmpty().WithName("UserId");
            RuleFor(p => p.RowKey)
                .NotEmpty()
                .MaximumLength(20)
                .Matches("^[a-zA-Z0-9$_-]+$")
                .WithName("Name");
            RuleFor(p => p.Type).NotEmpty();

            RuleFor(p => p.QueueName).NotEmpty()
                .When(p => p.Type == MessagePlatformType.Azure_Queue ||
                           p.Type == MessagePlatformType.RabbitMQ);

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
