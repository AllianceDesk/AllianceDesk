using System;
using System.Collections.Generic;
using System.Linq;
using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Basecode.Data.Repositories;


namespace ASI.Basecode.Data.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly List<Ticket> _data = new List<Ticket>();

        public IEnumerable<Ticket> RetrieveAll()
        {
            return _data;
        }

        public void Add(Ticket ticket)
        {
            _data.Add(ticket);
        }

        public void Update(Ticket ticket)
        {
            var existingData = _data.Where(x => x.Id == ticket.Id).FirstOrDefault();
            if (existingData != null)
            {
                existingData = ticket;
            }
        }

        public void Delete(String id)
        {
            var existingData = _data.Where(x => x.Id == id).FirstOrDefault();
            if (existingData != null)
            {
                _data.Remove(existingData);
            }
        }
    }
}
