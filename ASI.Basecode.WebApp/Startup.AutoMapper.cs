using AutoMapper;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.ServiceModels;
using Microsoft.Extensions.DependencyInjection;

namespace ASI.Basecode.WebApp
{
    // AutoMapper configuration
    internal partial class StartupConfigurer
    {
        /// <summary>
        /// Configure auto mapper
        /// </summary>
        private void ConfigureAutoMapper()
        {
            var mapperConfiguration = new MapperConfiguration(config =>
            {
                config.AddProfile(new AutoMapperProfileConfiguration());
            });

            this._services.AddSingleton<IMapper>(sp => mapperConfiguration.CreateMapper());
        }

        private class AutoMapperProfileConfiguration : Profile
        {
            public AutoMapperProfileConfiguration()
            {
                // Mapping is <from, to>
                CreateMap<UserViewModel, User>();
                CreateMap<User, UserViewModel>();
                CreateMap<TicketViewModel, Ticket>();
                CreateMap<NotificationServiceModel, Notification>();
                CreateMap<ArticleViewModel, Article>();
                CreateMap<Team, TeamViewModel>();
                CreateMap<TicketActivity, TicketActivityViewModel>();
                CreateMap<FeedbackViewModel, Feedback>()
                    .ForMember(dest => dest.FeedbackId, opt => opt.Ignore())
                    .ForMember(dest => dest.UserId, opt => opt.Ignore())
                    .ForMember(dest => dest.DateCreated, opt => opt.Ignore());
                CreateMap<Feedback, FeedbackViewModel>();
                CreateMap<UserPreference, UserPreferenceViewModel>();
            }
        }
    }
}
