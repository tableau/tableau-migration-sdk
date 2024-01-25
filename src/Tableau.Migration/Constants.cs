namespace Tableau.Migration
{
    /// <summary>
    /// Static class containing Tableau Migration SDK constant values.
    /// </summary>
    public static class Constants
    {
        #region - Public Constants -

        /// <summary>
        /// The separator to follow a domain name when a domain name is included in entity names.
        /// </summary>
        public const string DomainNameSeparator = "\\";

        /// <summary>
        /// The default path separator for <see cref="ContentLocation"/>s.
        /// </summary>
        public const string PathSeparator = "/";

        /// <summary>
        /// The local domain name.
        /// </summary>
        public const string LocalDomain = "local";

        /// <summary>
        /// The external domain name.
        /// </summary>
        public const string ExternalDomain = "external";

        /// <summary>
        /// The domain name for the Tableau ID with MFA authentication type.
        /// </summary>
        public const string TableauIdWithMfaDomain = "TABID_WITH_MFA";

        /// <summary>
        /// The username for the built-in system user.
        /// </summary>
        public const string SystemUsername = "_system";

        /// <summary>
        /// The location for the local system user.
        /// </summary>
        public static readonly ContentLocation SystemUserLocation = ContentLocation.ForUsername(LocalDomain, SystemUsername);

        /// <summary>
        /// The name for the built-in default project.
        /// </summary>
        public const string DefaultProjectName = "Default";

        /// <summary>
        /// The name for the built-in admin insights project.
        /// Admin Insight source: https://gitlab.tableausoftware.com/cloud-scale/admin-insights/genesis/blob/master/genesis/src/main/java/com/tableau/online/admindatasources/genesis/IConstants.java#L20
        /// </summary>
        public const string AdminInsightsProjectName = "Admin Insights";

        /// <summary>
        /// Alternate name for the built-in admin insights project.
        /// /// Admin Insight source: https://gitlab.tableausoftware.com/cloud-scale/admin-insights/genesis/blob/master/genesis/src/main/java/com/tableau/online/admindatasources/genesis/IConstants.java#L20
        /// </summary>
        public const string AdminInsightsTableauProjectName = "Admin Insights (Tableau)";

        /// <summary>
        /// Alternate  name for the built-in admin insights project.
        /// /// Admin Insight source: https://gitlab.tableausoftware.com/cloud-scale/admin-insights/genesis/blob/master/genesis/src/main/java/com/tableau/online/admindatasources/genesis/IConstants.java#L20
        /// </summary>
        public const string AdminInsightsTableauOnlineProjectName = "Admin Insights (Tableau Online)";

        #endregion

        #region - Internal Constants -

        /// <summary>
        /// The default prefix for the user agent string
        /// </summary>
        internal const string USER_AGENT_PREFIX = "TableauMigrationSDK";

        /// <summary>
        /// The default suffix for the python user agent string
        /// </summary>
        internal const string USER_AGENT_PYTHON_SUFFIX = "-Python";

        #endregion
    }
}
