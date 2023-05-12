using MessageSilo.Shared.Enums;
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

        public string Url { get; set; }

        public TargetDTO()
        {
            Kind = EntityKind.Target;
        }
    }
}
