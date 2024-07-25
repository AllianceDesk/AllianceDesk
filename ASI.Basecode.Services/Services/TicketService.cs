using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Data.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

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
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly ISessionHelper _sessionHelper;
        private readonly IMapper _mapper;
        private readonly INotificationRepository _notificationRepository;
        private readonly IAttachmentRepository _attachmentRepository;

        public TicketService(
            ITicketRepository ticketRepository,
            IUserRepository userRepository,
            ICategoryRepository categoryRepository,
            ITicketPriorityRepository ticketPriorityRepository,
            ITicketStatusRepository ticketStatusRepository,
            ITicketActivityRepository ticketActivityRepository,
            ITicketActivityOperationRepository ticketActivityOperationRepository,
            ITicketMessageRepository ticketMessageRepository,
            IFeedbackRepository feedbackRepository,
            IMapper mapper,
            ISessionHelper sessionHelper,
            INotificationRepository notificationRepository,
            ITeamRepository teamRepository,
            IAttachmentRepository attachmentRepository)
        {
            _ticketRepository = ticketRepository;
            _userRepository = userRepository;
            _categoryRepository = categoryRepository;
            _priorityRepository = ticketPriorityRepository;
            _statusRepository = ticketStatusRepository;
            _ticketActivityRepository = ticketActivityRepository;
            _ticketActivityOperationRepository = ticketActivityOperationRepository;
            _ticketMessageRepository = ticketMessageRepository;
            _feedbackRepository = feedbackRepository;
            _teamRepository = teamRepository;
            _mapper = mapper;
            _sessionHelper = sessionHelper;
            _teamRepository = teamRepository;
            _notificationRepository = notificationRepository;
            _attachmentRepository = attachmentRepository;
        }

        public IQueryable<TicketViewModel> RetrieveAll()
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
                TicketNumber = s.TicketNumber,
                Title = s.Title,
                Description = s.Description,
                DateCreated = s.DateCreated,
                CreatorId = s.CreatedBy.ToString(),
                AgentId = s.AssignedAgent.ToString(),
                StatusId = s.StatusId,
                Category = categories.TryGetValue(s.CategoryId, out var categoryName) ? categoryName : "Unknown",
                Priority = priorities.TryGetValue(s.PriorityId, out var priorityName) ? priorityName : "Unknown",
                Status = statuses.TryGetValue(s.StatusId, out var statusName) ? statusName : "Unknown",
                AgentName = s.AssignedAgent.HasValue && users.TryGetValue(s.AssignedAgent.Value, out var agentName) ? agentName : "Not yet Assigned",
                CreatorName = users.TryGetValue(s.CreatedBy, out var creatorName) ? creatorName : "Unknown",
                TicketHistory = activitiesByTicketId.TryGetValue(s.TicketId, out var activities) ?
                    _mapper.Map<IEnumerable<TicketActivityViewModel>>(activities) : Enumerable.Empty<TicketActivityViewModel>()
            });

            return data.AsQueryable();
        }

        public IQueryable<TicketViewModel> GetUserTickets(Guid userId, byte? status, string? searchTerm, string? sortOrder, int? page)
        {
            // Retrieve IQueryable from repository
            var ticketsQuery = _ticketRepository.GetUserTicketsById(userId);

            // Retrieve additional data
            var categories = _categoryRepository.RetrieveAll().ToDictionary(c => c.CategoryId, c => c.CategoryName);
            var priorities = _priorityRepository.RetrieveAll().ToDictionary(p => p.PriorityId, p => p.PriorityName);
            var statuses = _statusRepository.RetrieveAll().ToDictionary(st => st.StatusId, st => st.StatusName);
            var users = _userRepository.GetUsers().ToDictionary(u => u.UserId, u => u.Name);
            
            
            var ticketIds = ticketsQuery.Select(t => t.TicketId).ToList();
            var ticketActivities = _ticketActivityRepository.GetActivitiesByTicketIds(ticketIds).ToList();
            
            var feedbacks = _feedbackRepository.GetFeedbackByTicketIds(ticketIds)
                .ToDictionary(f => f.TicketId, f => _mapper.Map<FeedbackViewModel>(f));

            var activitiesByTicketId = ticketActivities
                .GroupBy(a => a.TicketId)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(a => a.ModifiedAt).ToList());

            // Apply filters
            if (status.HasValue && status != 0)
            {
                ticketsQuery = ticketsQuery.Where(t => t.StatusId == status);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                ticketsQuery = ticketsQuery.Where(t => t.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                                        t.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            }

            ticketsQuery = ticketsQuery.OrderByDescending(t => t.DateCreated);

            var model = ticketsQuery.Select(s => new TicketViewModel
            {
                TicketId = s.TicketId.ToString(),
                Title = s.Title,
                Description = s.Description,
                DateCreated = s.DateCreated,
                CreatorId = s.CreatedBy.ToString(),
                AgentId = s.AssignedAgent.ToString(),
                StatusId = s.StatusId,
                Category = categories.TryGetValue(s.CategoryId, out var categoryName) ? categoryName : "Unknown",
                Priority = priorities.TryGetValue(s.PriorityId, out var priorityName) ? priorityName : "Unknown",
                Status = statuses.TryGetValue(s.StatusId, out var statusName) ? statusName : "Unknown",
                AgentName = s.AssignedAgent.HasValue && users.TryGetValue(s.AssignedAgent.Value, out var agentName) ? agentName : "Unknown",
                CreatorName = users.TryGetValue(s.CreatedBy, out var creatorName) ? creatorName : "Unknown",
                LatestUpdate = activitiesByTicketId.TryGetValue(s.TicketId, out var activities) ?
                    activities.OrderByDescending(a => a.ModifiedAt).FirstOrDefault() : null,
                Feedback = feedbacks.TryGetValue(s.TicketId, out var feedback) ? feedback : null,
                TicketNumber= s.TicketNumber,
            });

            return model.AsQueryable();
        }

        public IEnumerable<TicketViewModel> GetAgentTickets(Guid id, string? status, string? searchTerm, string? sortOrder, int? page)
        {
            // Retrieve IQueryable from repository
            var ticketsQuery = _ticketRepository.GetAgentTicketsById(id);

            // Retrieve additional data
            var categories = _categoryRepository.RetrieveAll().ToDictionary(c => c.CategoryId, c => c.CategoryName);
            var priorities = _priorityRepository.RetrieveAll().ToDictionary(p => p.PriorityId, p => p.PriorityName);
            var statuses = _statusRepository.RetrieveAll().ToDictionary(st => st.StatusId, st => st.StatusName);
            var users = _userRepository.GetUsers().ToDictionary(u => u.UserId, u => u.Name);


            var ticketIds = ticketsQuery.Select(t => t.TicketId).ToList();
            var ticketActivities = _ticketActivityRepository.GetActivitiesByTicketIds(ticketIds).ToList();

            var feedbacks = _feedbackRepository.GetFeedbackByTicketIds(ticketIds)
                .ToDictionary(f => f.TicketId, f => _mapper.Map<FeedbackViewModel>(f));

            var activitiesByTicketId = ticketActivities
                .GroupBy(a => a.TicketId)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(a => a.ModifiedAt).ToList());

            // Start applying filters
            if(status == "Unresolved")
            {
                ticketsQuery = ticketsQuery.Where(t => t.StatusId == 1 || t.StatusId == 2 || t.StatusId == 3);
            } else
            {
                ticketsQuery = ticketsQuery.Where(t => t.StatusId == 4 || t.StatusId == 5 );
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                ticketsQuery = ticketsQuery.Where(t => t.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                                        t.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            }

            ticketsQuery = ticketsQuery.OrderByDescending(t => t.DateCreated);

             
            var model = ticketsQuery.Select(s => new TicketViewModel
            {
                TicketId = s.TicketId.ToString(),
                Title = s.Title,
                Description = s.Description,
                DateCreated = s.DateCreated,
                CreatorId = s.CreatedBy.ToString(),  
                Category = categories.TryGetValue(s.CategoryId, out var categoryName) ? categoryName : "Unknown",
                Priority = priorities.TryGetValue(s.PriorityId, out var priorityName) ? priorityName : "Unknown",
                Status = statuses.TryGetValue(s.StatusId, out var statusName) ? statusName : "Unknown",
                CreatorName = users.TryGetValue(s.CreatedBy, out var creatorName) ? creatorName : "Unknown",
                LatestUpdate = activitiesByTicketId.TryGetValue(s.TicketId, out var activities) ?
                    activities.OrderByDescending(a => a.ModifiedAt).FirstOrDefault() : null,
            });

            return model.AsQueryable();
        }

        public IQueryable<TicketViewModel> GetWeeklyTickets(DateTime startOfWeek, DateTime endOfWeek)
        {
            var ticketsQuery = _ticketRepository.GetWeeklyTickets(startOfWeek, endOfWeek);

            var model = ticketsQuery.Select(s => new TicketViewModel
            {
                TicketId = s.TicketId.ToString(),
                Title = s.Title,
                Description = s.Description,
                DateCreated = s.DateCreated,
                CreatorId = s.CreatedBy.ToString(),
                AgentId = s.AssignedAgent.ToString(),
                StatusId = s.StatusId,
                CategoryId = s.CategoryId,
                PriorityId = s.PriorityId,
            });

            return model;
        }

        public async Task AddAsync(TicketViewModel ticket)
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

            // Add Synchronous Tasks here
            Add(newTicket);

            // Call Async Tasks here
            await FileUploadAsync(ticket.AttachmentFiles, _sessionHelper.GetUserIdFromSession(), newTicket.TicketId);
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

        public void UpdateStatus(string ticketId, byte statusId)
        {
            var existingTicket = _ticketRepository.GetTicketById(Guid.Parse(ticketId));

            existingTicket.StatusId = statusId;

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
            var ticketActivities = _ticketActivityRepository.GetActivitiesByTicketId(guid)
                .OrderByDescending(activity => activity.ModifiedAt)
                .ToList();

            var latestUpdate = ticketActivities.FirstOrDefault();
            var latestUpdateDate = DateTime.Now; // Default value
            var latestUpdateMessage = "No Ticket Activity"; // Default message

            if (latestUpdate != null)
            {
                latestUpdateMessage = latestUpdate.Message;
                latestUpdateDate = latestUpdate.ModifiedAt;
            }


            var attachments = _attachmentRepository.GetAttachmentsByTicketId(ticket.TicketId);

            List<string> attachmentFilePaths = new List<string>();

            if (attachments != null)
            {
                foreach (var attachment in attachments)
                {
                    attachmentFilePaths.Add(attachment.FilePath);
                }
            }

            TicketActivityViewModel ticketActivityViewModel = new TicketActivityViewModel { Message = latestUpdateMessage, ModifiedAt = latestUpdateDate };

            var ticketViewModel = new TicketViewModel
            {
                TicketId = ticket.TicketId.ToString(),
                Title = ticket.Title,
                Description = ticket.Description,
                DateCreated = ticket.DateCreated,
                CategoryId = ticket.CategoryId,
                PriorityId = ticket.PriorityId,
                Category = categories.TryGetValue(ticket.CategoryId, out var categoryName) ? categoryName : "Unknown",
                Priority = priorities.TryGetValue(ticket.PriorityId, out var priorityName) ? priorityName : "Unknown",
                Status = statuses.TryGetValue(ticket.StatusId, out var statusName) ? statusName : "Unknown",
                CreatorName = users.TryGetValue(ticket.CreatedBy, out var creatorName) ? creatorName : "Unknown",
                AgentName = ticket.AssignedAgent.HasValue && users.TryGetValue(ticket.AssignedAgent.Value, out var agentName) ? agentName : "Unknown",
                TeamName = null,
                LatestUpdate = new TicketActivity { Message = latestUpdateMessage, ModifiedAt = latestUpdateDate },
                AttachmentStrings = attachmentFilePaths
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
                        .Select(i => DateTime.Today.AddDays(-6 + i))
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
                                .Where(t => t.StatusId == 5 && t.DateClosed >= startDate && t.DateClosed <= endDate)
                                .GroupBy(t => t.AssignedAgent)
                                .Select(g => new { UserId = g.Key, ResolvedCount = g.Count() })
                                .OrderByDescending(g => g.ResolvedCount)
                                .Take(5)
                                .ToList();
            var result = topResolvers.Select(tr => new UserViewModel
            {
                UserId = tr.UserId.ToString(),
                TeamName = _teamRepository.RetrieveAll().Where(t => t.TeamId ==
                            _userRepository.GetUsers().Where(u => u.UserId == tr.UserId).FirstOrDefault()?.TeamId)
                            .FirstOrDefault()?.TeamName ?? "No Team",
                Name = _userRepository.GetUsers().Where(u => u.UserId == tr.UserId).FirstOrDefault().Name,
                TicketResolved = tr.ResolvedCount,
            }).ToList();

            return result;
        }

        public void AddFeedback(FeedbackViewModel model)
        {
            var existingFeedback = _feedbackRepository.GetFeedbackByTicketId(model.TicketId);

            if (existingFeedback == null)
            {
                Feedback feedback = _mapper.Map<Feedback>(model);
                feedback.UserId = _sessionHelper.GetUserIdFromSession();
                feedback.FeedbackId = Guid.NewGuid();
                feedback.DateCreated = DateTime.Now;
                _feedbackRepository.Add(feedback);
            }
        }

        #region private methods
        private void Add(Ticket ticket)
        {
            // Generate Ticket Number
            var count = _ticketRepository.RetrieveAll().Count() + 1;
            string year = DateTime.Now.Year.ToString();
            string paddedCount = count.ToString().PadLeft(5, '0');
            ticket.TicketNumber = $"TCK-{year}-{paddedCount}";

            _ticketRepository.Add(ticket);

            // Add ticket activity
            TicketActivity newActivity = new TicketActivity();
            newActivity.HistoryId = Guid.NewGuid();
            newActivity.TicketId = ticket.TicketId;
            newActivity.OperationId = 1;
            newActivity.ModifiedBy = ticket.CreatedBy;
            newActivity.ModifiedAt = DateTime.Now;
            newActivity.Message = "Ticket created";
            _ticketActivityRepository.Add(newActivity);


            // Add notification
            Notification newNotification = new Notification();
            newNotification.NotificationId = Guid.NewGuid();
            newNotification.TicketId = ticket.TicketId;
            newNotification.Title = ticket.Title;
            newNotification.Body = ticket.Description;
            newNotification.DateCreated = DateTime.Now;
            newNotification.Status = true;
            newNotification.RecipientId = ticket.CreatedBy;
            _notificationRepository.Add(newNotification);
        }
        
        private async Task FileUploadAsync(List<IFormFile> files, Guid userId, Guid ticketId)
        {
            var folderPath = Path.Combine("wwwroot/uploads", userId.ToString(), ticketId.ToString());
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var filePath = Path.Combine(folderPath, Path.GetFileName(file.FileName));

                    // Ensure the directory exists
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    //After moving add the filePath to the database
                    Attachment attachment = new Attachment();
                    attachment.AttachmentId = Guid.NewGuid();
                    attachment.FilePath = "/uploads/" + userId.ToString() + "/" + ticketId.ToString() + "/" + file.FileName;
                    attachment.UploadedBy = userId;
                    attachment.UploadedAt = DateTime.Now;
                    attachment.TicketId = ticketId;

                    _attachmentRepository.AddAttachment(attachment);
                }
            }
        }
        #endregion
    }
}