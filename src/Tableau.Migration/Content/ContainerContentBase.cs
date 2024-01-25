namespace Tableau.Migration.Content
{
    /// <summary>
    /// Abstract base class for <see cref="IContainerContent"/> content items.
    /// </summary>
    public abstract class ContainerContentBase : MappableContainerContentBase, IContainerContent
    {
        /// <summary>
        /// Creates a new <see cref="ContainerContentBase"/> object.
        /// </summary>
        /// <param name="container">The content container.</param>
        protected ContainerContentBase(IContentReference container)
        {
            Container = container;
        }

        /// <inheritdoc/>
        virtual public IContentReference Container { get; protected set; }

        /// <inheritdoc/>
        protected override IContentReference? MappableContainer
        {
            get => Container;
            set => Container = Guard.AgainstNull(value, () => value);
        }
    }
}
