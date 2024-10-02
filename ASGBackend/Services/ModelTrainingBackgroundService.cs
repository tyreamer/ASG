using ASGBackend.Agents;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ASGBackend.Services
{
    public class ModelTrainingBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public ModelTrainingBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var userClusteringAgent = scope.ServiceProvider.GetRequiredService<UserClusteringAgent>();
                    await userClusteringAgent.TrainModelAsync();
                }

                // Wait for a day before retraining
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
    }

}
