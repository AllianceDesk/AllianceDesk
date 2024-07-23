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

            try
            {
                // Perform the ticket cleanup within a scope
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

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var now = DateTime.Now;
                    var nextRun = now.Date.AddDays(1);

                    if (now > nextRun) // If we've already passed midnight, execute the task immediately
                    {
                        nextRun = nextRun.AddDays(1); // Schedule for the next midnight instead
                    }

                    var delay = nextRun - now; // Calculate the delay until the next 12:00 AM

                    await Task.Delay(delay, stoppingToken); // Wait until the scheduled time

                    // Perform the ticket cleanup within a scope
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
            }

            _logger.LogInformation("TicketCleanupBackgroundService is stopping.");
        }
    }
}
