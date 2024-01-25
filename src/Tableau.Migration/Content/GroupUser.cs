namespace Tableau.Migration.Content
{
    internal sealed class GroupUser : IGroupUser
    {
        /// <inheritdoc />
        public IContentReference User { get; set; }

        public GroupUser(
            IContentReference user)
        {
            User = Guard.AgainstNull(user, () => user);
        }
    }
}
