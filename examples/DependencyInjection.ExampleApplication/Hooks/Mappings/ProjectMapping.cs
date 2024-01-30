using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Hooks.Mappings;
using Tableau.Migration.Resources;

namespace DependencyInjection.ExampleApplication.Hooks.Mappings
{
    public class ProjectMapping : ContentMappingBase<IProject>
    {
        // A unique ID used to distinguish between instances.
        protected readonly Guid InstanceId = Guid.NewGuid();

        private readonly ILogger<ProjectMapping> _logger;

        // The logger instance here will be injected when a ProjectMapping is 
        // retrieved from the service provider.
        public ProjectMapping(
            ISharedResourcesLocalizer localizer,
            ILogger<ProjectMapping> logger) 
                : base(localizer, logger)
        {
            _logger = logger;
            
            _logger.LogInformation("{Mapping} initialized: Instance ID = {Id}", nameof(ProjectMapping), InstanceId);
        }

        public override Task<ContentMappingContext<IProject>?> MapAsync(ContentMappingContext<IProject> context, CancellationToken cancel)
        {
            // For our purposes we're just logging a message here and migrating the project with no modifications.
            // In a real mapping this would contain logic to map source projects to destination ones.
            _logger.LogInformation("{Mapping}: Mapping project {Project}.", nameof(ProjectMapping), context.ContentItem.Name);

            return context.ToTask();
        }
    }
}
