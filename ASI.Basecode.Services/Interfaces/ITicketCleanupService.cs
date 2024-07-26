using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Interfaces
{
    public interface ITicketCleanupService
    {
        Task CleanupTicketsAsync();
        Task NotifyAgentsonIdleTicketsAsync();
    }
}
