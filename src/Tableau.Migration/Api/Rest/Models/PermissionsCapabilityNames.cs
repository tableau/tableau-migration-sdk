//
//  Copyright (c) 2025, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the "License") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

namespace Tableau.Migration.Api.Rest.Models
{
    /// <summary>
    /// Enumeration class for the various capability names used in REST API permissions.
    /// </summary>
    public class PermissionsCapabilityNames : StringEnum<PermissionsCapabilityNames>
    {
        /// <summary>
        /// Gets the name of the None capability.
        /// </summary>
        public const string None = "None";

        /// <summary>
        /// Gets the name of the "Add Comment" capability.
        /// </summary>
        public const string AddComment = "AddComment";

        /// <summary>
        /// Gets the name of the "Change Hierarchy" capability.
        /// </summary>        
        public const string ChangeHierarchy = "ChangeHierarchy";

        /// <summary>
        /// Gets the name of the "Change Permissions" capability.
        /// </summary> 
        public const string ChangePermissions = "ChangePermissions";

        /// <summary>
        /// Gets the name of the "Connect" capability.
        /// </summary> 
        public const string Connect = "Connect";

        /// <summary>
        /// Gets the name of the "Create Refresh Metrics" capability.
        /// </summary>       
        public const string CreateRefreshMetrics = "CreateRefreshMetrics";

        /// <summary>
        /// Gets the name of the "Delete" capability.
        /// </summary> 
        public const string Delete = "Delete";

        /// <summary>
        /// Gets the name of the "Execute" capability.
        /// </summary>        
        public const string Execute = "Execute";

        /// <summary>
        /// Gets the name of the "Export Data" capability.
        /// </summary> 
        public const string ExportData = "ExportData";

        /// <summary>
        /// Gets the name of the "Export Image" capability.
        /// </summary> 
        public const string ExportImage = "ExportImage";

        /// <summary>
        /// Gets the name of the "Export XML" capability.
        /// </summary>       
        public const string ExportXml = "ExportXml";

        /// <summary>
        /// Gets the name of the "Extract Refresh" capability.
        /// </summary>
        public const string ExtractRefresh = "ExtractRefresh";

        /// <summary>
        /// Gets the name of the "Filter" capability.
        /// </summary> 
        public const string Filter = "Filter";

        /// <summary>
        /// Gets the name of the "Inherited Project Leader" capability.
        /// </summary> 
        public const string InheritedProjectLeader = "InheritedProjectLeader";

        /// <summary>
        /// Gets the name of the "Project Leader" capability.
        /// </summary> 
        public const string ProjectLeader = "ProjectLeader";

        /// <summary>
        /// Gets the name of the "Read" capability.
        /// </summary> 
        public const string Read = "Read";

        /// <summary>
        /// Gets the name of the "Run Explain Data" capability.
        /// </summary> 
        public const string RunExplainData = "RunExplainData";

        /// <summary>
        /// Gets the name of the "Save As" capability.
        /// </summary> 
        public const string SaveAs = "SaveAs";

        /// <summary>
        /// Gets the name of the "Share View" capability.
        /// </summary> 
        public const string ShareView = "ShareView";

        /// <summary>
        /// Gets the name of the "View Comments" capability.
        /// </summary> 
        public const string ViewComments = "ViewComments";

        /// <summary>
        /// Gets the name of the "View Underlying Data" capability.
        /// </summary> 
        public const string ViewUnderlyingData = "ViewUnderlyingData";

        /// <summary>
        /// Gets the name of the "Web Authoring" capability.
        /// </summary> 
        public const string WebAuthoring = "WebAuthoring";

        /// <summary>
        /// Gets the name of the "Web Authoring" capability for flows.
        /// </summary> 
        public const string WebAuthoringForFlows = "WebAuthoringForFlows";

        /// <summary>
        /// Gets the name of the "Write" capability.
        /// </summary> 
        public const string Write = "Write";
    }
}