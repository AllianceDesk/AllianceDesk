using System;
using System.Collections.Generic;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.Services.Services;


namespace ASI.Basecode.Services.Interfaces
{
        public interface ITicketService
        {
                IEnumerable<TicketViewModel> RetrieveAll();
                IEnumerable<TicketViewModel> GetUserTickets(Guid id);
                IEnumerable<TicketViewModel> GetAgentTickets(Guid id);

                void Add(TicketViewModel ticket);
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
