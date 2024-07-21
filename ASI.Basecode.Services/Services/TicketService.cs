using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Data.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Services.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ITicketPriorityRepository _priorityRepository;
        private readonly ITicketStatusRepository _statusRepository;
        private readonly ITicketActivityRepository _ticketActivityRepository;
        private readonly ITicketActivityOperationRepository _ticketActivityOperationRepository;
        private readonly ITicketMessageRepository _ticketMessageRepository;
        private readonly ISessionHelper _sessionHelper;
        private readonly IMapper _mapper;
        private readonly ITeamRepository _teamRepository;

        public TicketService(
            ITicketRepository ticketRepository,
            IUserRepository userRepository,
            ICategoryRepository categoryRepository,
            ITicketPriorityRepository ticketPriorityRepository,
            ITicketStatusRepository ticketStatusRepository,
            ITicketActivityRepository ticketActivityRepository,
            ITicketActivityOperationRepository ticketActivityOperationRepository,
            ITicketMessageRepository ticketMessageRepository,
            IMapper mapper,
            ISessionHelper sessionHelper,
            ITeamRepository teamRepository)
        {
            _ticketRepository = ticketRepository;
            _userRepository = userRepository;
            _categoryRepository = categoryRepository;
            _priorityRepository = ticketPriorityRepository;
            _statusRepository = ticketStatusRepository;
            _ticketActivityRepository = ticketActivityRepository;
            _ticketActivityOperationRepository = ticketActivityOperationRepository;
            _ticketMessageRepository = ticketMessageRepository;
            _mapper = mapper;
            _sessionHelper = sessionHelper;
            _teamRepository = teamRepository;
        }

        public IEnumerable<TicketViewModel> RetrieveAll()
        {
            var tickets = _ticketRepository.RetrieveAll().ToList();
            var categories = _categoryRepository.RetrieveAll().ToDictionary(c => c.CategoryId, c => c.CategoryName);
            var priorities = _priorityRepository.RetrieveAll().ToDictionary(p => p.PriorityId, p => p.PriorityName);
            var statuses = _statusRepository.RetrieveAll().ToDictionary(st => st.StatusId, st => st.StatusName);
            var users = _userRepository.GetUsers().ToDictionary(u => u.UserId, u => u.Name);
            var ticketIds = tickets.Select(t => t.TicketId).ToList();
            var ticketActivities = _ticketActivityRepository.GetActivitiesByTicketIds(ticketIds).ToList();

            var activitiesByTicketId = ticketActivities
                .GroupBy(a => a.TicketId)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(a => a.ModifiedAt).ToList());

            var data = tickets.Select(s => new TicketViewModel
            {
                TicketId = s.TicketId.ToString(),
                Title = s.Title,
                Description = s.Description,
                DateCreated = s.DateCreated,
                CreatorId = s.CreatedBy.ToString(),
                AgentId = s.AssignedAgent.ToString(),
                StatusId = s.StatusId.ToString(),
                Category = categories.TryGetValue(s.CategoryId, out var categoryName) ? categoryName : "Unknown",
                Priority = priorities.TryGetValue(s.PriorityId, out var priorityName) ? priorityName : "Unknown",
                Status = statuses.TryGetValue(s.StatusId, out var statusName) ? statusName : "Unknown",
                AgentName = s.AssignedAgent.HasValue && users.TryGetValue(s.AssignedAgent.Value, out var agentName) ? agentName : "Unknown",
                CreatorName = users.TryGetValue(s.CreatedBy, out var creatorName) ? creatorName : "Unknown",
                TicketHistory = activitiesByTicketId.TryGetValue(s.TicketId, out var activities) ?
                    _mapper.Map<IEnumerable<TicketActivityViewModel>>(activities) : Enumerable.Empty<TicketActivityViewModel>()
            });

            return data;
        }

        public IEnumerable<TicketViewModel> GetUserTickets(Guid id)
        {
            var tickets = _ticketRepository.GetUserTicketsById(id);
            var categories = _categoryRepository.RetrieveAll().ToDictionary(c => c.CategoryId, c => c.CategoryName);
            var priorities = _priorityRepository.RetrieveAll().ToDictionary(p => p.PriorityId, p => p.PriorityName);
            var statuses = _statusRepository.RetrieveAll().ToDictionary(st => st.StatusId, st => st.StatusName);
            var users = _userRepository.GetUsers().ToDictionary(u => u.UserId, u => u.Name);
            var ticketIds = tickets.Select(t => t.TicketId).ToList();
            var ticketActivities = _ticketActivityRepository.GetActivitiesByTicketIds(ticketIds).ToList();

            var activitiesByTicketId = ticketActivities
                .GroupBy(a => a.TicketId)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(a => a.ModifiedAt).ToList());

            var model = tickets.Select(s => new TicketViewModel
            {
                TicketId = s.TicketId.ToString(),
                Title = s.Title,
                Description = s.Description,
                DateCreated = s.DateCreated,
                CreatorId = s.CreatedBy.ToString(),
                AgentId = s.AssignedAgent.ToString(),
                StatusId = s.StatusId.ToString(),
                Category = categories.TryGetValue(s.CategoryId, out var categoryName) ? categoryName : "Unknown",
                Priority = priorities.TryGetValue(s.PriorityId, out var priorityName) ? priorityName : "Unknown",
                Status = statuses.TryGetValue(s.StatusId, out var statusName) ? statusName : "Unknown",
                AgentName = s.AssignedAgent.HasValue && users.TryGetValue(s.AssignedAgent.Value, out var agentName) ? agentName : "Unknown",
                CreatorName = users.TryGetValue(s.CreatedBy, out var creatorName) ? creatorName : "Unknown",
                LatestUpdate = activitiesByTicketId.TryGetValue(s.TicketId, out var activities) ?
                    activities.OrderByDescending(a => a.ModifiedAt).FirstOrDefault() : null
            });

            return model;
        }
        public IEnumerable<TicketViewModel> GetAgentTickets(Guid id)
        {
            var tickets = _ticketRepository.GetAgentTicketsById(id);

            return _mapper.Map<IEnumerable<TicketViewModel>>(tickets);
        }

        public void Add(TicketViewModel ticket)
        {
            Ticket newTicket = new Ticket();
            newTicket.TicketId = Guid.NewGuid();
            newTicket.Title = ticket.Title;
            newTicket.Description = ticket.Description;
            newTicket.DateCreated = DateTime.Now;
            newTicket.CreatedBy = _sessionHelper.GetUserIdFromSession();
            newTicket.StatusId = 1;
            newTicket.PriorityId = Convert.ToByte(ticket.PriorityId);
            newTicket.CategoryId = Convert.ToByte(ticket.CategoryId);
            _ticketRepository.Add(newTicket);

            // Add ticket activity
            TicketActivity newActivity = new TicketActivity();
            newActivity.HistoryId = Guid.NewGuid();
            newActivity.TicketId = newTicket.TicketId;
            newActivity.OperationId = 1;
            newActivity.ModifiedBy = newTicket.CreatedBy;
            newActivity.ModifiedAt = DateTime.Now;
            newActivity.Message = "Ticket created";
            _ticketActivityRepository.Add(newActivity);
        }

        public void Update(TicketViewModel ticket)
        {


            var existingTicket = _ticketRepository.GetTicketById(Guid.Parse(ticket.TicketId));

            existingTicket.Title = ticket.Title;
            existingTicket.Description = ticket.Description;
            existingTicket.CategoryId = Convert.ToByte(ticket.CategoryId);
            existingTicket.PriorityId = Convert.ToByte(ticket.PriorityId);

            if (ticket.AgentId != null)
            {
                existingTicket.AssignedAgent = Guid.Parse(ticket.AgentId);
            }
            _ticketRepository.Update(existingTicket);

            // Add ticket activity
            TicketActivity newActivity = new TicketActivity();
            newActivity.HistoryId = Guid.NewGuid();
            newActivity.TicketId = existingTicket.TicketId;
            newActivity.OperationId = 2;
            newActivity.ModifiedBy = _sessionHelper.GetUserIdFromSession();
            newActivity.ModifiedAt = DateTime.Now;
            newActivity.Message = "Ticket updated";
            _ticketActivityRepository.Add(newActivity);
        }

        public void CloseTicket(string ticketId)
        {
            var existingTicket = _ticketRepository.GetTicketById(Guid.Parse(ticketId));

            existingTicket.StatusId = 5;

            _ticketRepository.Update(existingTicket);

            // Add ticket activity
            TicketActivity newActivity = new TicketActivity();
            newActivity.HistoryId = Guid.NewGuid();
            newActivity.TicketId = existingTicket.TicketId;
            newActivity.OperationId = 5;
            newActivity.ModifiedBy = _sessionHelper.GetUserIdFromSession();
            newActivity.ModifiedAt = DateTime.Now;
            newActivity.Message = "Ticket was closed";
            _ticketActivityRepository.Add(newActivity);
        }

        public void Delete(String id)
        {
            Guid guid = Guid.Parse(id);
            // Delete the ticket activities associated with the ticket
            var activitites = _ticketActivityRepository.GetActivitiesByTicketId(guid);
            var activities = _ticketActivityRepository.RetrieveAll().Where(s => s.TicketId.ToString() == id).ToList();

            foreach (var activity in activities)
            {
                _ticketActivityRepository.Delete(activity.HistoryId);
            }

            // Delete the ticket messages associated with the ticket

            // Delete the ticket itself
            _ticketRepository.Delete(id);
        }

        public TicketViewModel GetById(string id)
        {
            Guid guid = Guid.Parse(id);
            var ticket = _ticketRepository.GetTicketById(guid);

            if (ticket == null)
            {
                return null;
            }

            var userIds = new[] { ticket.CreatedBy, ticket.AssignedAgent }
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .Distinct();

            var categories = _categoryRepository.RetrieveAll().ToDictionary(c => c.CategoryId, c => c.CategoryName);
            var priorities = _priorityRepository.RetrieveAll().ToDictionary(p => p.PriorityId, p => p.PriorityName);
            var statuses = _statusRepository.RetrieveAll().ToDictionary(st => st.StatusId, st => st.StatusName);
            var users = _userRepository.GetUsersByIds(userIds).ToDictionary(u => u.UserId, u => u.Name);

            var ticketViewModel = new TicketViewModel
            {
                TicketId = ticket.TicketId.ToString(),
                Title = ticket.Title,
                Description = ticket.Description,
                CategoryId = ticket.CategoryId.ToString(),
                PriorityId = ticket.PriorityId.ToString(),
                Category = categories.TryGetValue(ticket.CategoryId, out var categoryName) ? categoryName : "Unknown",
                Priority = priorities.TryGetValue(ticket.PriorityId, out var priorityName) ? priorityName : "Unknown",
                Status = statuses.TryGetValue(ticket.StatusId, out var statusName) ? statusName : "Unknown",
                CreatorName = users.TryGetValue(ticket.CreatedBy, out var creatorName) ? creatorName : "Unknown",
                AgentName = ticket.AssignedAgent.HasValue && users.TryGetValue(ticket.AssignedAgent.Value, out var agentName) ? agentName : "Unknown",
                TeamName = null

                // Attachments = ticket.Attachments.ToString(),
                // Feedback = ticket.Feedback // Uncomment and implement if needed
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
            return _categoryRepository.GetCategoryById(id);
        }

        public TicketPriority GetPriorityById(byte id)
        {
            return _priorityRepository.GetPriorityById(id);
        }

        public TicketStatus GetStatusById(byte id)
        {
            return _statusRepository.GetStatusById(id);
        }

        public void SendMessage(TicketMessageViewModel message)
        {
            TicketMessage newMessage = new TicketMessage();
            newMessage.MessageId = Guid.NewGuid();
            newMessage.TicketId = Guid.Parse(message.TicketId);
            newMessage.MessageBody = message.Message;
            newMessage.UserId = Guid.Parse(message.SentById);
            newMessage.PostedAt = DateTime.Now;

            _ticketMessageRepository.Add(newMessage);
        }

        public IEnumerable<TicketMessageViewModel> GetMessages(string id)
        {

            var messages = _ticketMessageRepository.GetMessagesByTicketId(id);

            // Fetch the users involved in the conversation so we don't have to look up the name for every message
            var userIds = messages.Select(m => m.UserId).Distinct();
            var users = _userRepository.GetUsersByIds(userIds);
            var userDictionary = users.ToDictionary(u => u.UserId, u => u.Name);

            var model = messages.Select(message => new TicketMessageViewModel
            {
                MessageId = message.MessageId.ToString(),
                TicketId = message.TicketId.ToString(),
                Message = message.MessageBody,
                SentById = message.UserId.ToString(),
                SentByName = userDictionary[message.UserId],
                PostedAt = message.PostedAt,
            });

            return model;
        }

        public IEnumerable<TicketActivityViewModel> GetHistory(string id)
        {
            Guid guid = Guid.Parse(id);
            var activities = _ticketActivityRepository.GetActivitiesByTicketId(guid);

            // Fetch all users involved in the activities to avoid multiple database calls
            var userIds = activities.Select(a => a.ModifiedBy).Distinct();
            var users = _userRepository.GetUsersByIds(userIds);
            var userDictionary = users.ToDictionary(u => u.UserId);

            // Fetch all operations involved in the activities to avoid multiple database calls
            var operationIds = activities.Select(a => a.OperationId).Distinct();
            var operations = _ticketActivityOperationRepository.GetOperationsByIds(operationIds);
            var operationDictionary = operations.ToDictionary(o => o.OperationId);

            var activityViewModels = activities.Select(activity => new TicketActivityViewModel
            {
                HistoryId = activity.HistoryId.ToString(),
                TicketId = activity.TicketId.ToString(),
                ModifiedBy = activity.ModifiedBy.ToString(),
                ModifiedByName = userDictionary[activity.ModifiedBy].Name,
                ModifiedAt = activity.ModifiedAt,
                OperationName = operationDictionary[activity.OperationId].Name,
                OperationId = activity.OperationId,
            }).ToList();

            return activityViewModels;
        }

        public void AssignAgent(string ticketId, string userId)
        {
            Guid ticketGuid = Guid.Parse(ticketId);
            Guid userGuid = Guid.Parse(userId);

            var existingTicket = _ticketRepository.GetTicketById(ticketGuid);

            existingTicket.AssignedAgent = userGuid;
            existingTicket.StatusId = 2;
            _ticketRepository.Update(existingTicket);
        }

        public Dictionary<string, int> GetTicketVolume()
        {
            var startDate = DateTime.Today.AddDays(-6);
            var endDate = DateTime.Today.AddDays(1);

            var dailyCounts = _ticketRepository.RetrieveAll()
                            .Where(t => t.DateCreated >= startDate && t.DateCreated <= endDate)
                            .GroupBy(t => t.DateCreated.Date)
                            .Select(g => new { Date = g.Key, Count = g.Count() })
                            .ToList();

            var result = Enumerable.Range(0, 7)
                        .Select(i => DateTime.Today.AddDays(-i))
                        .ToDictionary(date => date.ToString("dddd"), date => 0);

            foreach (var dailyCount in dailyCounts)
            {
                result[dailyCount.Date.ToString("dddd")] = dailyCount.Count;
            }
            return result;
        }

        public List<UserViewModel> GetWeeklyTopResolvers()
        {
            var startDate = DateTime.Today.AddDays(-6);
            var endDate = DateTime.Today.AddDays(1);

            var topResolvers = _ticketRepository.RetrieveAll()
                                .Where(t => t.StatusId == 4 && t.DateClosed >= startDate && t.DateClosed <= endDate)
                                .GroupBy(t => t.AssignedAgent)
                                .Select(g => new { UserId = g.Key, ResolvedCount = g.Count() })
                                .OrderByDescending(g => g.ResolvedCount)
                                .Take(5)
                                .ToList();
            var result = topResolvers.Select(tr => new UserViewModel
            {
                UserId = tr.UserId.ToString(),
                TeamName = _teamRepository.RetrieveAll().Where(t => t.TeamId ==
                            _userRepository.GetUsers().Where(u => u.UserId == tr.UserId).FirstOrDefault().TeamId)
                            .FirstOrDefault().TeamName,
                Name = _userRepository.GetUsers().Where(u => u.UserId == tr.UserId).FirstOrDefault().Name,
                TicketResolved = tr.ResolvedCount,
            }).ToList();

            return result;
        }
    }
}