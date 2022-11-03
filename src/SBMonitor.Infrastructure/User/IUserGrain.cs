using Orleans;
using SBMonitor.Core.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBMonitor.Infrastructure.User
{
    public interface IUserGrain : IGrainWithStringKey
    {
        Task AddDeadLetterCorrector(ConnectionSettingsDTO setting);

        Task InitDeadLetterCorrectors();
    }
}
