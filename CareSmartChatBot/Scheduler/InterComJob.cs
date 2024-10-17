using Quartz;
using System;
using System.Threading.Tasks;
using CareSmartChatBot.Services;

namespace CareSmartChatBot.Scheduler
{
    public class IntercomJob : IJob
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<IntercomJob> _logger;

        public IntercomJob(IServiceProvider serviceProvider, ILogger<IntercomJob> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("IntercomJob is executing.");

            using (var scope = _serviceProvider.CreateScope())
            {
                var intercomService = scope.ServiceProvider.GetRequiredService<IIntercomService>();
                await intercomService.GetConversations();
            }
        }
    }

}