using MessageSilo.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageSilo.Shared.Models
{
    public class TargetDTO
    {
        //Common
        public string Token { get; set; }

        public string Name { get; set; }

        public string Id => $"{Token}|{Name}";

        public TargetType Type { get; set; }

        public string Url { get; set; }
    }
}
