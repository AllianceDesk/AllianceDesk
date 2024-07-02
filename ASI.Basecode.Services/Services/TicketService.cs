using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Data.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASI.Basecode.Services.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ITicketPriorityRepository _priorityRepository;
        private readonly ITicketStatusRepository _statusRepository;
        private readonly IMapper _mapper;

        public TicketService(ITicketRepository ticketRepository, IUserRepository userRepository, ICategoryRepository categoryRepository, ITicketPriorityRepository ticketPriorityRepository, ITicketStatusRepository ticketStatusRepository, IMapper mapper)
        {
            _ticketRepository = ticketRepository;
            _userRepository = userRepository;
            _categoryRepository = categoryRepository;
            _priorityRepository = ticketPriorityRepository;
            _statusRepository = ticketStatusRepository;
            _mapper = mapper;
        }

        public IEnumerable<TicketViewModel> RetrieveAll()
        {

            var data = _ticketRepository.RetrieveAll().Select(s => new TicketViewModel
            {
                TicketId = s.TicketId.ToString(),
                Title = s.Title,
                Description = s.Description,
                DateCreated = s.DateCreated,
                CreatorId = s.CreatedBy.ToString(),
                AgentId = s.AssignedAgent.ToString(),
                StatusId = s.StatusId.ToString(),
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

            Ticket newTicket = new Ticket();
            newTicket.TicketId = Guid.NewGuid();
            newTicket.Title = ticket.Title;
            newTicket.Description = ticket.Description;
            newTicket.DateCreated = DateTime.Now;
            newTicket.CreatedBy = Guid.Parse(ticket.CreatorId); 
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
        
        public TicketViewModel GetById(string id)
        {
            var ticket = _ticketRepository.RetrieveAll().Where(s => s.TicketId.ToString() == id).FirstOrDefault();

            if (ticket == null)
            {
                return null;
            }

            var ticketViewModel = new TicketViewModel
            {
                TicketId = ticket.TicketId.ToString(),
                Title = ticket.Title,
                Description = ticket.Description,
                CategoryId = ticket.CategoryId.ToString(),
                PriorityId = ticket.PriorityId.ToString(),
                StatusId = ticket.StatusId.ToString()
            };

            return ticketViewModel;
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
