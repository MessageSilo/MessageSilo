using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageSilo.Shared.Models
{
    public class TargetDTO : Entity
    {
        //Common
        public TargetType? Type { get; set; }

        //API
        public string Url { get; set; }

        //Azure_EventGrid
        public string Endpoint { get; set; }

        public string TopicName { get; set; }

        public string AccessKey { get; set; }

        public TargetDTO()
        {
            Kind = EntityKind.Target;
        }

        public async Task Encrypt(string password)
        {
            if (AccessKey is not null)
                AccessKey = await encryptAsync(AccessKey, password);
        }

        public async Task Decrypt(string password)
        {
            if (AccessKey is not null)
                AccessKey = await decryptAsync(AccessKey, password);
        }

        public override string ToString()
        {
            return YamlConverter.Serialize(this);
        }
    }
}
