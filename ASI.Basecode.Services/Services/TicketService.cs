using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Data.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IMapper _mapper;

        public TicketService(ITicketRepository ticketRepository, IMapper mapper)
        {
            _ticketRepository = ticketRepository;
            _mapper = mapper;
        }

        public IEnumerable<TicketViewModel> RetrieveAll()
        {
            var data = _ticketRepository.RetrieveAll().Select(s => new TicketViewModel
            {
                Id = s.Id,
                Title = s.Title,
                Description = s.Description,
                Category = s.Category,
                PriorityId = s.PriorityId,
                StatusId = s.StatusId,
                Attachment = s.Attachment,
                CreatedBy = s.CreatedBy,
                FeedbackId = s.FeedbackId
            });

            return data;
        }

        public void Add(TicketViewModel ticket)
        {
            if (ticket == null)
            {
                throw new ArgumentNullException(nameof(ticket), "TicketViewModel cannot be null.");
            }

            var newTicket = new Ticket();
            _mapper.Map(ticket, newTicket);
            newTicket.Id = Guid.NewGuid().ToString();
            // If "CreatedBy" is not part of the input view model and needs to be set manually
            newTicket.CreatedBy = "Ivan";

            _ticketRepository.Add(newTicket);
        }

        public void Update(TicketViewModel ticket)
        {
            var existingTicket = _ticketRepository.RetrieveAll().Where(s => s.Id == ticket.Id).FirstOrDefault();

            _mapper.Map(ticket, existingTicket);
            
            _ticketRepository.Update(existingTicket);
        }

        public void Delete(String id)
        {
            _ticketRepository.Delete(id);
        }
    }
}
