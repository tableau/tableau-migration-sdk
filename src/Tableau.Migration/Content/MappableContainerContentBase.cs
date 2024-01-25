namespace Tableau.Migration.Content
{
    /// <summary>
    /// Abstract base class for default <see cref="IMappableContainerContent"/> implementation.
    /// </summary>
    public abstract class MappableContainerContentBase : ContentBase, IMappableContainerContent
    {
        //We use a protected property and an explicit interface implementation
        //So that implementation types can use a more natural name (e.g. Project.ParentProject).

        /// <summary>
        /// Gets or sets the current mappable project/container reference.
        /// </summary>
        protected abstract IContentReference? MappableContainer { get; set; }

        /// <inheritdoc/>
        IContentReference? IMappableContainerContent.Container => MappableContainer;

        /// <inheritdoc/>
        void IMappableContainerContent.SetLocation(IContentReference? container, ContentLocation newLocation)
        {
            MappableContainer = container;
            Location = newLocation;
            Name = newLocation.Name;
        }
    }
}
