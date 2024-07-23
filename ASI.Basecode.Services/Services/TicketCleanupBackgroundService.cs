using ASI.Basecode.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Services
{
    public class TicketCleanupBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<TicketCleanupBackgroundService> _logger;

        public TicketCleanupBackgroundService(IServiceScopeFactory serviceScopeFactory, ILogger<TicketCleanupBackgroundService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("TicketCleanupBackgroundService is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var ticketCleanupService = scope.ServiceProvider.GetRequiredService<ITicketCleanupService>();
                        _logger.LogInformation("Running ticket cleanup...");
                        await ticketCleanupService.CleanupTicketsAsync();
                        _logger.LogInformation("Ticket cleanup completed.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while cleaning up tickets.");
                }

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken); // Adjust delay as needed
            }

            _logger.LogInformation("TicketCleanupBackgroundService is stopping.");
        }
    }
}
