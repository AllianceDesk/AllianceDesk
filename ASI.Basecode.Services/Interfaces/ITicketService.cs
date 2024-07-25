﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.ServiceModels;

namespace ASI.Basecode.Services.Interfaces
{
        public interface ITicketService
        {
                IQueryable<TicketViewModel> GetAllTickets();
                IQueryable<TicketViewModel> GetUserTickets(Guid userId, byte? status, string? searchTerm, string? sortOrder, int? page);
                IQueryable<TicketViewModel> GetAgentTickets(Guid agentId, string? status, string? searchTerm, string? sortOrder, int? page);
                IQueryable<TicketViewModel> GetWeeklyTickets(DateTime startOfWeek, DateTime endOfWeek);
                Task AddAsync(TicketViewModel ticket);
                void Update(TicketViewModel ticket);
                void Delete(Guid id);
                TicketViewModel GetById(Guid ticketId);
                IEnumerable<Category> GetCategories();
                IEnumerable<TicketPriority> GetPriorities();
                IEnumerable<TicketStatus> GetStatuses();
                Category GetCategoryById(byte categoryId);
                TicketPriority GetPriorityById(byte priorityId);
                TicketStatus GetStatusById(byte statusId);
                void SendMessage(TicketMessageViewModel message);
                IEnumerable<TicketActivityViewModel> GetHistory(Guid ticketId);
                IEnumerable<TicketMessageViewModel> GetMessages(Guid ticketId);
                void AssignAgent(Guid ticketId, Guid userId);
                Dictionary<string, int> GetTicketVolume();
                List<UserViewModel> GetWeeklyTopResolvers();
                void UpdateStatus(Guid ticketId, byte statusId);
                void AddFeedback(FeedbackViewModel model);
        }
}
