using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Data.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

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
                AgentName = _userRepository.GetUsers().Where(u => u.UserId == s.AssignedAgent).FirstOrDefault()?.Name,
                CreatorName = _userRepository.GetUsers().Where(u => u.UserId == s.CreatedBy).FirstOrDefault()?.Name,
                LatestUpdate = _ticketActivityRepository.RetrieveAll().Where(a => a.TicketId == s.TicketId).OrderByDescending(a => a.ModifiedAt).FirstOrDefault()
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
            Console.WriteLine(ticket.TicketId);
            var existingTicket = _ticketRepository.RetrieveAll().Where(s => s.TicketId.ToString() == ticket.TicketId).FirstOrDefault();

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

        public void Delete(String id)
        {
           // Delete the ticket activities associated with the ticket

            var activities = _ticketActivityRepository.RetrieveAll().Where(s => s.TicketId.ToString() == id).ToList();
            
            foreach (var activity in activities)
            {
                _ticketActivityRepository.Delete(activity.HistoryId.ToString());
            }

            // Delete the ticket messages associated with the ticket

            // Delete the ticket itself
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
                Category = _categoryRepository.RetrieveAll().FirstOrDefault(c => c.CategoryId == ticket.CategoryId)?.CategoryName,
                Priority = _priorityRepository.RetrieveAll().FirstOrDefault(p => p.PriorityId == ticket.PriorityId)?.PriorityName,
                Status = _statusRepository.RetrieveAll().FirstOrDefault(st => st.StatusId == ticket.StatusId)?.StatusName,
                CreatorName = _userRepository.GetUsers().FirstOrDefault(u => u.UserId == ticket.CreatedBy)?.Name,
                AgentName = _userRepository.GetUsers().FirstOrDefault(u => u.UserId == ticket.AssignedAgent)?.Name,
                TeamName = null

                // Attachments = ticket.Attachments.ToString(),
                // Feedback
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

        public void SendMessage(TicketMessageViewModel message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message), "TicketMessageViewModel cannot be null.");
            }

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

            var messages = _ticketMessageRepository.RetrieveAll()
                                     .Where(s => s.TicketId.ToString() == id)
                                     .Select(message => new TicketMessageViewModel
                                     {
                                         MessageId = message.MessageId.ToString(),
                                         TicketId = message.TicketId.ToString(),
                                         Message = message.MessageBody,
                                         SentById = message.UserId.ToString(),
                                         SentByName = _userRepository.GetUsers().FirstOrDefault(u => u.UserId == message.UserId).Name,
                                         PostedAt = message.PostedAt,
                                     });

            return messages;
        }

        public IEnumerable<TicketActivityViewModel> GetHistory(string id)
        {
            var activities = _ticketActivityRepository.RetrieveAll()
                                    .Where(s => s.TicketId.ToString() == id)
                                    .Select(activity => new TicketActivityViewModel
                                    {
                                        HistoryId = activity.HistoryId.ToString(),
                                        TicketId = activity.TicketId.ToString(),
                                        ModifiedBy = activity.ModifiedBy.ToString(),
                                        ModifiedByName = _userRepository.GetUsers().FirstOrDefault(u => u.UserId == activity.ModifiedBy).Name,
                                        ModifiedAt = activity.ModifiedAt,
                                        OperationName = _ticketActivityOperationRepository.RetrieveAll().FirstOrDefault(o => o.OperationId == activity.OperationId).Name,
                                        OperationId = activity.OperationId,
                                    });

            return activities;
        }

        public void AssignAgent(string ticketId, string userId)
        {
            var existingTicket = _ticketRepository.RetrieveAll().Where(s => s.TicketId.ToString() == ticketId).FirstOrDefault();

            existingTicket.AssignedAgent = Guid.Parse(userId);
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
                            .Select (g => new {Date = g.Key, Count = g.Count()})
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

        public List <UserViewModel> GetWeeklyTopResolvers()
        {
            var startDate = DateTime.Today.AddDays(-6);
            var endDate = DateTime.Today.AddDays(1);

            var topResolvers = _ticketRepository.RetrieveAll()
                                .Where (t => t.StatusId == 4 && t.DateClosed >= startDate && t.DateClosed <= endDate)
                                .GroupBy(t => t.AssignedAgent)
                                .Select(g => new {UserId = g.Key, ResolvedCount = g.Count()})
                                .OrderByDescending (g=> g.ResolvedCount)
                                .Take (5)
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