using System;
using System.Collections.Generic;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.ServiceModels;


namespace ASI.Basecode.Services.Interfaces
{
    public interface ITicketService
    {
        IEnumerable<TicketViewModel> RetrieveAll();
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

        void AddMessage(TicketMessageViewModel message);

        void AddHistory(TicketActivityViewModel activity);
        IEnumerable<TicketActivityViewModel> GetHistory(string id);
        IEnumerable<TicketMessageViewModel> GetMessages(string id);
    }
}
