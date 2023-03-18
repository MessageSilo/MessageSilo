using MessageSilo.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageSilo.Shared.Models
{
    public class ConnectionState
    {
        public ConnectionSettingsDTO ConnectionSettings { get; set; }

        public ConnectionStatus Status { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Name:               {ConnectionSettings.Name}");
            sb.AppendLine($"Type:               {ConnectionSettings.Type}");
            sb.AppendLine($"Status:             {Status}");
            sb.AppendLine($"---");
            sb.AppendLine($"ConnectionString:   {ConnectionSettings.ConnectionString}");
            sb.AppendLine($"QueueName:          {ConnectionSettings.QueueName}");

            switch (ConnectionSettings.Type)
            {
                case MessagePlatformType.Azure_Queue:
                    sb.AppendLine($"SubQueue:           {ConnectionSettings.SubQueue}");
                    break;
                case MessagePlatformType.Azure_Topic:
                    sb.AppendLine($"TopicName:          {ConnectionSettings.TopicName}");
                    sb.AppendLine($"SubscriptionName:   {ConnectionSettings.SubscriptionName}");
                    sb.AppendLine($"SubQueue:           {ConnectionSettings.SubQueue}");
                    break;
                case MessagePlatformType.RabbitMQ:
                    sb.AppendLine($"ExchangeName:       {ConnectionSettings.ExchangeName}");
                    break;
            }

            sb.AppendLine($"CorrectorFuncBody:  {ConnectionSettings.CorrectorFuncBody}");
            sb.AppendLine($"AutoAck:            {ConnectionSettings.AutoAck}");

            return sb.ToString();
        }
    }
}
