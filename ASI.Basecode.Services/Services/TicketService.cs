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
using System.Net.Sockets;

namespace ASI.Basecode.Services.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ITicketPriorityRepository _priorityRepository;
        private readonly ITicketStatusRepository _statusRepository;
        private readonly IMapper _mapper;

        public TicketService(ITicketRepository ticketRepository, ICategoryRepository categoryRepository, ITicketPriorityRepository ticketPriorityRepository, ITicketStatusRepository ticketStatusRepository, IMapper mapper)
        {
            _ticketRepository = ticketRepository;
            _categoryRepository = categoryRepository;
            _priorityRepository = ticketPriorityRepository;
            _statusRepository = ticketStatusRepository;
            _mapper = mapper;
        }


        #region User Methods

        #endregion

        #region Admin Methods

        #endregion 

        public IEnumerable<TicketViewModel> RetrieveAll()
        {

            

            var data = _ticketRepository.RetrieveAll().Select(s => new TicketViewModel
            {
                TicketId = s.TicketId.ToString(),
                Title = s.Title,
                Description = s.Description,
                Category = _categoryRepository.RetrieveAll().Where(c => c.CategoryId == s.CategoryId).FirstOrDefault().CategoryName,
                Priority = _priorityRepository.RetrieveAll().Where(p => p.PriorityId == s.PriorityId).FirstOrDefault().PriorityName,
                Status = _statusRepository.RetrieveAll().Where(st => st.StatusId == s.StatusId).FirstOrDefault().StatusName,
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
            newTicket.TicketId = Guid.NewGuid();
            newTicket.Title = ticket.Title;
            newTicket.Description = ticket.Description;
            newTicket.DateCreated = DateTime.Now;

            // This is a temporary value for CreatedBy, replace when user authentication is implemented
            newTicket.CreatedBy = Guid.Parse("c9876543-b21d-43e5-a345-556642441234");

            newTicket.StatusId = 1;
            newTicket.PriorityId = Convert.ToByte(ticket.PriorityId);
            newTicket.CategoryId = Convert.ToByte(ticket.CategoryId);
            _ticketRepository.Add(newTicket);
        }

        public void Update(TicketViewModel ticket)
        {
            var existingTicket = _ticketRepository.RetrieveAll().Where(s => s.TicketId.ToString() == ticket.TicketId).FirstOrDefault();

            _mapper.Map(ticket, existingTicket);
            
            _ticketRepository.Update(existingTicket);
        }

        public void Delete(String id)
        {
            _ticketRepository.Delete(id);
        }

        public IEnumerable<Category> GetCategories()
        {
            return _categoryRepository.RetrieveAll();
        }

        public IEnumerable<TicketPriority> GetPriorities()
        {
            return _priorityRepository.RetrieveAll();
        }
        
        public IEnumerable<TicketStatus> GetStatuses()
        {
            return _statusRepository.RetrieveAll();
        }

        public Category GetCategoryById(byte id)
        {
            return _categoryRepository.RetrieveAll().Where(s => s.CategoryId == id).FirstOrDefault();
        }

        public TicketPriority GetPriorityById(byte id)
        {
            return _priorityRepository.RetrieveAll().Where(s => s.PriorityId == id).FirstOrDefault();
        }

        public TicketStatus GetStatusById(byte id)
        {
            return _statusRepository.RetrieveAll().Where(s => s.StatusId == id).FirstOrDefault();
        }
    }
}
