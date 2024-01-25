using System;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Responses;

namespace Tableau.Migration.Content
{
    internal class Project : MappableContainerContentBase, IProject
    {
        /// <inheritdoc/>
        public string Description { get; set; }

        /// <inheritdoc/>
        public string ContentPermissions { get; set; }

        /// <inheritdoc/>
        public IContentReference? ParentProject { get; private set; }

        /// <inheritdoc/>
        public IContentReference Owner { get; set; }

        /// <inheritdoc/>
        protected override IContentReference? MappableContainer
        {
            get => ParentProject;
            set => ParentProject = value;
        }

        public Project(IProjectType response, IContentReference? parentProject, IContentReference owner)
        {
            Id = Guard.AgainstDefaultValue(response.Id, () => response.Id);
            Name = Guard.AgainstNullEmptyOrWhiteSpace(response.Name, () => response.Name);
            Description = response.Description ?? string.Empty;
            ContentPermissions = Guard.AgainstNullEmptyOrWhiteSpace(response.ContentPermissions, () => response.ContentPermissions);

            ParentProject = parentProject;

            if (ParentProject is null)
            {
                Location = new(Name);
            }
            else
            {
                var responseProjectId = response.GetParentProjectId();
                if (responseProjectId != ParentProject.Id)
                {
                    throw new ArgumentException(
                        $"The input parent project's ID {ParentProject.Id} is different from that in the Tableau Server response {responseProjectId}.",
                        nameof(parentProject));
                }

                Location = new(ParentProject.Location, Name);
            }

            Owner = owner;
        }

        public Project(ProjectsResponse.ProjectType response, IContentReference? parentProject, IContentReference owner)
            : this((IProjectType)response, parentProject, owner)
        { }

        public Project(CreateProjectResponse response, IContentReference? parentProject, IContentReference owner)
            : this(Guard.AgainstNull(response.Item, () => response.Item), parentProject, owner)
        { }

        protected Project(IProject project)
        {
            Id = project.Id;
            ContentUrl = project.ContentUrl;
            Name = project.Name;
            Description = project.Description;
            ContentPermissions = project.ContentPermissions;
            ParentProject = project.ParentProject;
            Location = project.Location;
            Owner = project.Owner;
        }
    }
}
