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
        private readonly ITeamRepository _teamRepository;
        private readonly IMapper _mapper;
        private static readonly Random Random = new Random();

        public UserService(IUserRepository repository, IMapper mapper, IUserRoleRepository userRoleRepository, ITeamRepository teamRepository)
        {
            _mapper = mapper;
            _repository = repository;
            _userRoleRepository = userRoleRepository;
            _teamRepository = teamRepository;
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
            if (!_teamRepository.TeamExists(model.TeamName)){
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

            if (user !=  null)
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
    }
}
