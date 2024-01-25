namespace Tableau.Migration.Content
{
    /// <summary>
    /// Abstract base class for content items that have a domain/username
    /// and use a username-style location.
    /// </summary>
    public abstract class UsernameContentBase : ContentBase, IUsernameContent
    {
        /// <inheritdoc/>
        public override string Name
        {
            get => _name;
            protected set
            {
                _name = value;
                UpdateLocation();
            }
        }
        private string _name = string.Empty;

        /// <inheritdoc/>
        public string Domain
        {
            get => _domain;
            protected set
            {
                _domain = value;
                UpdateLocation();
            }
        }
        private string _domain = string.Empty;

        /// <summary>
        /// Updates the current location based on the current domain/username values.
        /// </summary>
        protected void UpdateLocation()
            => Location = ContentLocation.ForUsername(Domain, Name);

        void IMappableContent.SetLocation(ContentLocation newLocation)
        {
            switch (newLocation.PathSegments.Length)
            {
                case 0:
                    Name = Domain = string.Empty;
                    break;
                case 1:
                    Domain = string.Empty;
                    Name = newLocation.PathSegments[0];
                    break;
                default:
                    Domain = newLocation.PathSegments[0];
                    Name = newLocation.PathSegments[1];
                    break;
            }
        }
    }
}
