using System;
using System.Collections.Generic;
using ASI.Basecode.Services.ServiceModels;


namespace ASI.Basecode.Services.Interfaces
{
    public interface ITicketService
    {
        IEnumerable<TicketViewModel> RetrieveAll();

        void Add(TicketViewModel ticket);

        void Update(TicketViewModel ticket);

        void Delete(string id);
    }
}
