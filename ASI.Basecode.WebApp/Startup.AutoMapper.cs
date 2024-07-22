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
                CreateMap<UserViewModel, User>(); // ViewModel to Model
                CreateMap<User, UserViewModel>(); // Model to ViewModel
                CreateMap<TicketViewModel, Ticket>();
                CreateMap<NotificationServiceModel, Notification>();
                CreateMap<ArticleViewModel, Article>();
                CreateMap<Team, TeamViewModel>();

                CreateMap<TicketActivity, TicketActivityViewModel>()
                    .ForMember(dest => dest.HistoryId, opt => opt.MapFrom(src => src.HistoryId.ToString()))
                    .ForMember(dest => dest.TicketId, opt => opt.MapFrom(src => src.TicketId.ToString()))
                    .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.ModifiedBy.ToString()))
                    .ForMember(dest => dest.ModifiedByName, opt => opt.MapFrom(src => src.ModifiedByNavigation.Name))
                    .ForMember(dest => dest.OperationName, opt => opt.MapFrom(src => src.Operation.Name))
                    .ForMember(dest => dest.message, opt => opt.MapFrom(src => src.Message));

                CreateMap<FeedbackViewModel, Feedback>()
                    .ForMember(dest => dest.FeedbackId, opt => opt.Ignore()) // Ignore properties not directly mapped
                    .ForMember(dest => dest.UserId, opt => opt.Ignore())
                    .ForMember(dest => dest.DateCreated, opt => opt.Ignore());

                CreateMap<Feedback, FeedbackViewModel>();

                CreateMap<UserPreference, UserPreferenceViewModel>();
            }
        }
    }
}
