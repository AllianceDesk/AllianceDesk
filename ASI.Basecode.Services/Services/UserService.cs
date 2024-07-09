using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Data.Repositories;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.Manager;
using ASI.Basecode.Services.ServiceModels;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IMapper _mapper;

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

        public void AddUser(UserViewModel model)
        {
            var user = new User();
            if (!_repository.UserExists(model.Email))
            {
                _mapper.Map(model, user);
                user.UserId = Guid.NewGuid();
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

        public void AddTeam(UserViewModel model)
        {
            var team = new Team();
            if (!_teamRepository.TeamExists(model.TeamName)){
                _mapper.Map(model, team);
                team.TeamId = Guid.NewGuid();
                team.TeamName = model.TeamName;
                _teamRepository.AddTeam(team);
            }
        }
        public IEnumerable<Team> GetTeams()
        {
            return _teamRepository.RetrieveAll();
        }

        public IEnumerable<UserRole> GetUserRoles()
        {
            return _userRoleRepository.RetrieveAll();
        }
    }
}
