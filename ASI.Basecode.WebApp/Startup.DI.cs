using ASI.Basecode.Data;
using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Repositories;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.Services.Services;
using ASI.Basecode.WebApp.Authentication;
using ASI.Basecode.WebApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ASI.Basecode.WebApp
{
    // Other services configuration
    internal partial class StartupConfigurer
    {
        /// <summary>
        /// Configures the other services.
        /// </summary>
        private void ConfigureOtherServices()
        {
            // Framework
            this._services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            this._services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

            // Common
            this._services.AddScoped<TokenProvider>();
            this._services.TryAddSingleton<TokenProviderOptionsFactory>();
            this._services.TryAddSingleton<TokenValidationParametersFactory>();
            this._services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Services
            this._services.TryAddSingleton<TokenValidationParametersFactory>();
            this._services.AddScoped<IUserService, UserService>();
            this._services.AddScoped<ITicketService, TicketService>();
            this._services.AddScoped<IArticleService, ArticleService>();
            this._services.AddScoped<INotificationService, NotificationService>();
           
            // Repositories
            this._services.AddScoped<IUserRepository, UserRepository>();
            this._services.AddScoped<ITicketRepository, TicketRepository>();
            this._services.AddScoped<ICategoryRepository, CategoryRepository>();
            this._services.AddScoped<ITicketPriorityRepository, TicketPriorityRepository>();
            this._services.AddScoped<ITicketStatusRepository, TicketStatusRepository>();
            this._services.AddScoped<ITicketActivityOperationRepository, TicketActivityOperationRepository>();
            this._services.AddScoped<ITicketActivityRepository, TicketActivityRepository>();
            this._services.AddScoped<ITicketMessageRepository, TicketMessageRepository>();
            this._services.AddScoped<IUserPreferenceRepository, UserPreferenceRepository>();
            this._services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            this._services.AddScoped<ITeamRepository, TeamRepository>();
            this._services.AddScoped<IArticleRepository, ArticleRepository>();
            this._services.AddScoped<INotificationRepository, NotificationRepository>();
            this._services.AddScoped<IFavoriteRepository, FavoriteRepository>();
            this._services.AddScoped<IFeedbackRepository, FeedbackRepository>();
            this._services.AddScoped<IAttachmentRepository, AttachmentRepository>();

            // Manager Class
            this._services.AddScoped<SignInManager>();
            this._services.AddHttpClient();
        }
    }
}
