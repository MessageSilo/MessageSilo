using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBMonitor.Infrastructure.User
{
    public interface IUserGrain : IGrainWithStringKey
    {
        Task AddDeadLetterCorrector(string settingString);

        Task InitDeadLetterCorrectors();
    }
}
