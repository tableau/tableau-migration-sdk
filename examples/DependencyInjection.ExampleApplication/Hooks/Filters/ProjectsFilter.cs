using System;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Content;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Hooks.Filters;
using Tableau.Migration.Resources;

namespace DependencyInjection.ExampleApplication.Hooks.Filters
{
    public class ProjectsFilter : ContentFilterBase<IProject>
    {
        // A unique ID used to distinguish between instances.
        protected readonly Guid InstanceId = Guid.NewGuid();

        private readonly ILogger<ProjectsFilter> _logger;

        // The logger instance here will be injected when a ProjectsFilter is 
        // retrieved from the service provider.
        public ProjectsFilter(ISharedResourcesLocalizer localizer, ILogger<ProjectsFilter> logger) 
            : base (localizer, logger)
        {
            _logger = logger;

            _logger.LogInformation("{Filter} initialized: Instance ID = {Id}", nameof(ProjectsFilter), InstanceId);
        }

        public override bool ShouldMigrate(ContentMigrationItem<IProject> item)
        {
            // For our purposes we're just logging a message here and migrating the project with no modifications.
            // In a real filter this would contain logic to determine whether to migrate the project.
            _logger.LogInformation("{Filter}: Migrating project {Project}.", nameof(ProjectsFilter), item.SourceItem.Name);

            return true;
        }
    }
}
