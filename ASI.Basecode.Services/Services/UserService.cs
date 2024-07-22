using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Data.Repositories;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.Manager;
using ASI.Basecode.Services.ServiceModels;
using AutoMapper;
using NetTopologySuite.Algorithm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IUserPreferenceRepository _userPreferenceRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IMapper _mapper;
        private readonly ITicketActivityRepository _ticketActivityRepository;
        private readonly ITicketActivityOperationRepository _ticketActivityOperationRepository;
        private readonly ITicketRepository _ticketRepository;
        private static readonly Random Random = new Random();
        private readonly ISessionHelper _sessionHelper;

        public UserService(IUserRepository repository, 
                            IMapper mapper, 
                            ISessionHelper sessionHelper, 
                            IUserRoleRepository userRoleRepository, 
                            IUserPreferenceRepository userPreferenceRepository, 
                            ITeamRepository teamRepository,
                            ITicketActivityOperationRepository ticketActivityOperationRepository,
                            ITicketActivityRepository ticketActivityRepository,
                            ITicketRepository ticketRepository)
        {
            _mapper = mapper;
            _sessionHelper = sessionHelper;
            _repository = repository;
            _userRoleRepository = userRoleRepository;
            _userPreferenceRepository = userPreferenceRepository;
            _teamRepository = teamRepository;
            _ticketActivityRepository = ticketActivityRepository;
            _ticketActivityOperationRepository = ticketActivityOperationRepository;
            _ticketRepository = ticketRepository;
        }

        public LoginResult AuthenticateUser(string userName, string password, ref User user)
        {
            user = new User();
            var passwordKey = PasswordManager.EncryptPassword(password);
            user = _repository.GetUsers().Where(x => x.Username == userName &&
                                                     x.Password == passwordKey).FirstOrDefault();

            return user != null ? LoginResult.Success : LoginResult.Failed;
        }

        public int GetLogInUserRole(string userName)
        {
            var user = _repository.GetUsers().Where(u => u.Username == userName).FirstOrDefault();
            if (user != null)
            {
                return user.RoleId;
            }
            return 0;
        }

        public void AddUser(UserViewModel model)
        {
            var user = new User();
            if (!_repository.UserExists(model.Email))
            {
                _mapper.Map(model, user);
                user.UserId = Guid.NewGuid();
                Console.WriteLine($"Generated UserId: {user.UserId}");
                user.Username = model.UserName;
                user.Email = model.Email;
                user.Password = PasswordManager.EncryptPassword(model.Password);
                /*user.CreatedTime = DateTime.Now;
                user.UpdatedTime = DateTime.Now;
                user.CreatedBy = System.Environment.UserName;
                user.UpdatedBy = System.Environment.UserName;*/
                user.RoleId = model.RoleId;
                _repository.AddUser(user);

                // Create User Preference
                var preference = new UserPreference();
                preference.PreferenceId = Guid.NewGuid();
                preference.UserId = user.UserId;
                preference.InAppNotifications = true;
                preference.EmailNotifications = true;
                preference.DefaultTicketView = "Open";
                preference.DefaultTicketPerPage = 5;
                _userPreferenceRepository.Add(preference);
            }
            else
            {
                throw new InvalidDataException(Resources.Messages.Errors.UserExists);
            }
        }

        public void UpdateUser(UserViewModel model)
        {
            var existingData = _repository.GetUsers().Where(u => u.UserId.ToString() == model.UserId).FirstOrDefault();
            if (existingData != null)
            {
                _mapper.Map(model, existingData);
                existingData.Name = model.UserName;
                existingData.Username = model.UserName;
                existingData.Password = PasswordManager.EncryptPassword(model.Password);
                existingData.Email = model.Email;
                existingData.RoleId = model.RoleId;
                if (!string.IsNullOrEmpty(model.TeamId))
                {
                    existingData.TeamId = Guid.Parse(model.TeamId);
                }
                else
                {
                    // Handle the case where model.TeamId is null or empty
                    existingData.TeamId = null; // or set a default value if appropriate
                }

                _repository.UpdateUser(existingData);
            }

        }

        public void DeleteUser(string userId)
        {
            _repository.DeleteUser(userId);
        }

        public void AddTeam(UserViewModel model)
        {
            var team = new Team();
            if (!_teamRepository.TeamExists(model.TeamName))
            {
                team.TeamId = Guid.NewGuid();
                team.TeamName = model.TeamName;
                _teamRepository.AddTeam(team);
            }
        }

        public IEnumerable<UserRole> GetUserRoles()
        {
            return _userRoleRepository.RetrieveAll();
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _repository.GetUsers();
        }

        public IEnumerable<User> GetUsers()
        {
            return _repository.GetUsers().Where(u => u.RoleId == 3);
        }

        public User GetUserById(string id)
        {
            User user = _repository.GetUsers().Where(x => x.UserId.ToString() == id).FirstOrDefault();

            if (user != null)
            {
                return user;
            }

            return null;
        }

        public IEnumerable<UserViewModel> GetAgents()
        {
            var agents = _repository.GetUsers().Where(x => x.RoleId == 2).ToList();

            return _mapper.Map<IEnumerable<UserViewModel>>(agents);
        }

        public IEnumerable<TeamViewModel> GetTeams()
        {
            var teams = _teamRepository.RetrieveAll().ToList();

            return _mapper.Map<IEnumerable<TeamViewModel>>(teams);
        }

        public string GeneratePassword()
        {
            int length = 8;
            const string lowerCase = "abcdefghijklmnopqrstuvwxyz";
            const string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string digits = "1234567890";
            const string symbols = ".!@#$%^&*()";
            const string allChars = lowerCase + upperCase + digits + symbols;

            var password = new StringBuilder();

            // Ensure at least one character from each required set
            password.Append(lowerCase[Random.Next(lowerCase.Length)]);
            password.Append(upperCase[Random.Next(upperCase.Length)]);
            password.Append(symbols[Random.Next(symbols.Length)]);
            password.Append(digits[Random.Next(digits.Length)]);

            // Fill the remaining characters with random characters from allChars
            for (int i = password.Length; i < length; i++)
            {
                password.Append(allChars[Random.Next(allChars.Length)]);
            }

            // Shuffle the password to ensure randomness
            return new string(password.ToString().ToCharArray().OrderBy(s => (Random.Next(2) % 2) == 0).ToArray());
        }

        public List<TicketActivityViewModel> GetRecentUserActivity()
        {
            var userActivity = _ticketActivityRepository.RetrieveAll()
                                .OrderByDescending(a => a.ModifiedAt).Take(5)
                                .Select(t => new TicketActivityViewModel
                                {
                                    HistoryId = t.HistoryId.ToString(),
                                    TicketId = t.TicketId.ToString(),
                                    Title = _ticketRepository.RetrieveAll().Where(i => i.TicketId == t.TicketId).FirstOrDefault().Title,
                                    ModifiedBy = t.ModifiedBy.ToString(),
                                    ModifiedByName = _repository.GetUsers().Where(u => u.UserId == t.ModifiedBy).FirstOrDefault().Name,
                                    ModifiedAt = t.ModifiedAt,
                                    Date = t.ModifiedAt.ToString("dd MMM yyyy, h:mm tt"),
                                    OperationId = t.OperationId,
                                    OperationName = _ticketActivityOperationRepository.RetrieveAll().Where(o => o.OperationId == t.OperationId).FirstOrDefault().Name,
                                    message = t.Message,
                                }).ToList();
            return userActivity;
        }
        public UserPreferenceViewModel GetPreferenceView()
        {
            Guid userId = _sessionHelper.GetUserIdFromSession();
            var user = _repository.GetUserById(userId);

            if (user != null)
            {
                var preference = _userPreferenceRepository.GetUserPreferencesByUserId(userId);
                var model = _mapper.Map<UserPreferenceViewModel>(preference);
                model.Name = user.Name;
                model.Email = user.Email;
                return model;
            }

            return null;
        }

        public void UpdatePreference(UserPreferenceViewModel model)
        {
            var user = _repository.GetUserById(_sessionHelper.GetUserIdFromSession());
            if (user != null)
            {
                user.Name = model.Name;
                user.Email = model.Email;

                if (model.Password != "")
                {
                    user.Password = PasswordManager.EncryptPassword(model.Password);
                }

                _repository.UpdateUser(user);
            }

            var preference = _userPreferenceRepository.GetUserPreferencesByUserId(_sessionHelper.GetUserIdFromSession());
            if (preference != null)
            {
                preference.InAppNotifications = model.InAppNotifications;
                preference.EmailNotifications = model.EmailNotifications;
                preference.DefaultTicketView = model.DefaultTicketView;
                preference.DefaultTicketPerPage = model.DefaultTicketPerPage;
                _userPreferenceRepository.Update(preference);
            }
        }

   
    }
}
