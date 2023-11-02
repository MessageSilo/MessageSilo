using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Serialization;
using YamlDotNet.Serialization;

namespace MessageSilo.Shared.Models
{
    [GenerateSerializer]
    public class ConnectionSettingsDTO : Entity
    {
        [Id(0)]
        public string ConnectionString { get; set; }

        [Id(1)]
        public MessagePlatformType? Type { get; set; }

        [Id(2)]
        public string QueueName { get; set; }

        [Id(3)]
        public IEnumerable<string> Enrichers { get; set; } = new List<string>();

        [Id(4)]
        public string Target { get; set; }

        [YamlIgnore]
        public string TargetId => string.IsNullOrEmpty(Target) ? null! : $"{UserId}|{Target}";

        [YamlIgnore]
        [Id(5)]
        public EntityKind TargetKind { get; set; }

        [Id(6)]
        public ReceiveMode? ReceiveMode { get; set; }

        //Azure

        [Id(7)]
        public string TopicName { get; set; }

        [Id(8)]
        public string SubscriptionName { get; set; }

        [Id(9)]
        public string SubQueue { get; set; }

        //RabbitMQ

        [Id(10)]
        public string ExchangeName { get; set; }

        //AWS

        [Id(11)]
        public string Region { get; set; }

        [Id(12)]
        public string AccessKey { get; set; }

        [Id(13)]
        public string SecretAccessKey { get; set; }

        public ConnectionSettingsDTO()
        {
            Kind = EntityKind.Connection;
        }

        public override string ToString()
        {
            return YamlConverter.Serialize(this);
        }

        public async Task Encrypt(string password)
        {
            if (ConnectionString is not null)
                ConnectionString = await encryptAsync(ConnectionString, password);

            if (AccessKey is not null)
                AccessKey = await encryptAsync(AccessKey, password);

            if (SecretAccessKey is not null)
                SecretAccessKey = await encryptAsync(SecretAccessKey, password);
        }

        public async Task Decrypt(string password)
        {
            if (ConnectionString is not null)
                ConnectionString = await decryptAsync(ConnectionString, password);

            if (AccessKey is not null)
                AccessKey = await decryptAsync(AccessKey, password);

            if (SecretAccessKey is not null)
                SecretAccessKey = await decryptAsync(SecretAccessKey, password);
        }

        public ConnectionSettingsDTO GetCopy()
        {
            var yaml = ToString();
            return YamlConverter.Deserialize<ConnectionSettingsDTO>(yaml);
        }
    }
}
