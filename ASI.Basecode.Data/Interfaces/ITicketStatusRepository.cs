using ASI.Basecode.Data.Models;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Interfaces
{
    public interface ITicketStatusRepository
    {
        IEnumerable<TicketStatus> RetrieveAll();
    }
}
