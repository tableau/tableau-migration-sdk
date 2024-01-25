namespace Tableau.Migration.Api.Rest.Models
{
    /// <summary>    
    /// Class containing content permissions constants. 
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_projects.htm#create_project">Tableau API Reference</see> 
    /// for documentation.
    /// </summary>
    public class ContentPermissions : StringEnum<ContentPermissions>
    {
        /// <summary>
        /// Gets the name of the LockedToProject content permission mode.
        /// </summary>
        public const string LockedToProject = "LockedToProject";

        /// <summary>
        /// Gets the name of the ManagedByOwner content permission mode.
        /// </summary>
        public const string ManagedByOwner = "ManagedByOwner";

        /// <summary>
        /// Gets the name of the LockedToProjectWithoutNested content permission mode.
        /// </summary>
        public const string LockedToProjectWithoutNested = "LockedToProjectWithoutNested";
    }
}
