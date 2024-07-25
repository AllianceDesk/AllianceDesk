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
    public class TeamService : ITeamService
    {
        private readonly IUserRepository _repository;
        private readonly IUserPreferenceRepository _userPreferenceRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IMapper _mapper;
        private readonly ISessionHelper _sessionHelper;
        private readonly IDepartmentRepository _departmentRepository;

        public TeamService(IUserRepository repository, 
                            IMapper mapper, 
                            ISessionHelper sessionHelper, 
                            IUserRoleRepository userRoleRepository, 
                            IUserPreferenceRepository userPreferenceRepository, 
                            ITeamRepository teamRepository,
                            IDepartmentRepository departmentRepository)
        {
            _mapper = mapper;
            _sessionHelper = sessionHelper;
            _repository = repository;
            _userPreferenceRepository = userPreferenceRepository;
            _teamRepository = teamRepository;
            _departmentRepository = departmentRepository;
        }

        public void AddTeam(TeamViewModel model)
        {
            var team = new Team();
            if (!_teamRepository.TeamExists(model.TeamName))
            {
                team.TeamId = Guid.NewGuid();
                team.TeamName = model.TeamName;
                team.TeamDescription = model.TeamDescription;
                team.DepartmentId = model.DepartmentId;
                _teamRepository.AddTeam(team);
            }
        }

        public IEnumerable<Department> GetDepartments()
        {
            return _departmentRepository.RetrieveAll();
        }
        public string GetDepartmentName(byte departmentId)
        {
            return _departmentRepository.GetDepartmentById(departmentId).DepartmentName;
        }

        public IEnumerable<UserViewModel> GetAgents()
        {
            var agents = _repository.GetUsers().Where(x => x.RoleId == 2).ToList();

            return _mapper.Map<IEnumerable<UserViewModel>>(agents);
        }

        public int GetTeamNumber(string teamId)
        {
            return GetAgents().Where(u => u.TeamId == teamId).Count();
        }

        public IEnumerable<TeamViewModel> GetTeams()
        {
            var teams = _teamRepository.RetrieveAll().ToList();

            return _mapper.Map<IEnumerable<TeamViewModel>>(teams);
        }

        public UserPreferenceViewModel GetUserPreference()
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
