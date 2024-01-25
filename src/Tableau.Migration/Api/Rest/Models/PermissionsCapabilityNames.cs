namespace Tableau.Migration.Api.Rest.Models
{
    /// <summary>
    /// The Capability names used in the test API.
    /// </summary>
    public class PermissionsCapabilityNames : StringEnum<PermissionsCapabilityNames>
    {
        /// <summary>
        /// Gets the name of capability name for no cabapibilities.
        /// </summary>
        public const string None = "None";

        /// <summary>
        /// Gets the name of capability name for AddComment.
        /// </summary>
        public const string AddComment = "AddComment";

        /// <summary>
        /// Gets the name of capability name for ChangeHierarchy.
        /// </summary>        
        public const string ChangeHierarchy = "ChangeHierarchy";

        /// <summary>
        /// Gets the name of capability name for ChangePermissions.
        /// </summary> 
        public const string ChangePermissions = "ChangePermissions";

        /// <summary>
        /// Gets the name of capability name for Connect.
        /// </summary> 
        public const string Connect = "Connect";

        /// <summary>
        /// Gets the name of capability name for CreateRefreshMetrics.
        /// </summary>       
        public const string CreateRefreshMetrics = "CreateRefreshMetrics";

        /// <summary>
        /// Gets the name of capability name for Delete.
        /// </summary> 
        public const string Delete = "Delete";

        /// <summary>
        /// Gets the name of capability name for Execute.
        /// </summary>        
        public const string Execute = "Execute";

        /// <summary>
        /// Gets the name of capability name for ExportData.
        /// </summary> 
        public const string ExportData = "ExportData";

        /// <summary>
        /// Gets the name of capability name for ExportImage.
        /// </summary> 
        public const string ExportImage = "ExportImage";

        /// <summary>
        /// Gets the name of capability name for ExportXml.
        /// </summary>       
        public const string ExportXml = "ExportXml";

        /// <summary>
        /// Gets the name of capability name for Filter.
        /// </summary> 
        public const string Filter = "Filter";

        /// <summary>
        /// Gets the name of capability name for InheritedProjectLeader.
        /// </summary> 
        public const string InheritedProjectLeader = "InheritedProjectLeader";

        /// <summary>
        /// Gets the name of capability name for ProjectLeader.
        /// </summary> 
        public const string ProjectLeader = "ProjectLeader";

        /// <summary>
        /// Gets the name of capability name for Read.
        /// </summary> 
        public const string Read = "Read";

        /// <summary>
        /// Gets the name of capability name for RunExplainData.
        /// </summary> 
        public const string RunExplainData = "RunExplainData";

        /// <summary>
        /// Gets the name of capability name for SaveAs.
        /// </summary> 
        public const string SaveAs = "SaveAs";

        /// <summary>
        /// Gets the name of capability name for ShareView.
        /// </summary> 
        public const string ShareView = "ShareView";

        /// <summary>
        /// Gets the name of capability name for ViewComments.
        /// </summary> 
        public const string ViewComments = "ViewComments";

        /// <summary>
        /// Gets the name of capability name for ViewUnderlyingData.
        /// </summary> 
        public const string ViewUnderlyingData = "ViewUnderlyingData";

        /// <summary>
        /// Gets the name of capability name for WebAuthoring.
        /// </summary> 
        public const string WebAuthoring = "WebAuthoring";

        /// <summary>
        /// Gets the name of capability name for WebAuthoringForFlows.
        /// </summary> 
        public const string WebAuthoringForFlows = "WebAuthoringForFlows";

        /// <summary>
        /// Gets the name of capability name for Write.
        /// </summary> 
        public const string Write = "Write";
    }
}