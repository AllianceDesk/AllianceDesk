using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.ServiceModels;

namespace ASI.Basecode.Services.Interfaces
{
        public interface ITicketService
        {
                IQueryable<TicketViewModel> RetrieveAll();
                IQueryable<TicketViewModel> GetUserTickets(Guid id, byte? status, string? searchTerm, string? sortOrder, int? page);
                IEnumerable<TicketViewModel> GetAgentTickets(Guid id, string? status, string? searchTerm, string? sortOrder, int? page);
                IQueryable<TicketViewModel> GetWeeklyTickets(DateTime startOfWeek, DateTime endOfWeek);
                Task AddAsync(TicketViewModel ticket);
                void Update(TicketViewModel ticket);
                void Delete(string id);
                TicketViewModel GetById(string id);
                IEnumerable<Category> GetCategories();
                IEnumerable<TicketPriority> GetPriorities();
                IEnumerable<TicketStatus> GetStatuses();
                Category GetCategoryById(byte id);
                TicketPriority GetPriorityById(byte id);
                TicketStatus GetStatusById(byte id);
                void SendMessage(TicketMessageViewModel message);
                IEnumerable<TicketActivityViewModel> GetHistory(string id);
                IEnumerable<TicketMessageViewModel> GetMessages(string id);
                void AssignAgent(string ticketId, string userId);
                Dictionary<string, int> GetTicketVolume();
                List<UserViewModel> GetWeeklyTopResolvers();
                void UpdateStatus(string ticketId, byte statusId);
                void AddFeedback(FeedbackViewModel model);
        }
}
