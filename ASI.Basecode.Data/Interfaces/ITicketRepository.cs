using System.Collections.Generic;
using ASI.Basecode.Data.Models;

namespace ASI.Basecode.Data.Interfaces
{
    public interface ITicketRepository
    {
        IEnumerable<Ticket> RetrieveAll();

        void Add (Ticket ticket);

        void Update (Ticket ticket);

        void Delete (string id);
    }
}