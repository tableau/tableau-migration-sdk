using System;
using Microsoft.Extensions.Logging;

namespace DependencyInjection.ExampleApplication
{
    public class SingletonService
    {
        // A unique ID used to distinguish between instances.
        protected readonly Guid InstanceId = Guid.NewGuid();

        private readonly ILogger<SingletonService> _logger;

        // The logger instance here will be injected when a SingletonService is 
        // retrieved from the service provider.
        public SingletonService(ILogger<SingletonService> logger)
        {
            _logger = logger;

            _logger.LogInformation("{Service} initialized: Instance ID = {Id}", nameof(SingletonService), InstanceId);
        }
    }
}
