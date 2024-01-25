namespace Tableau.Migration
{
    internal class UserAgentSuffixProvider : IUserAgentSuffixProvider
    {
        /// <summary>
        /// The default User agent suffix is empty. 
        /// </summary>
        public string UserAgentSuffix { get; init; } = string.Empty;
    }
}
