# Copyright (c) 2025, Salesforce, Inc.
# SPDX-License-Identifier: Apache-2
#
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
#     http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.

"""Wrapper for classes in Tableau.Migration.Api.Rest.Models namespace."""

# region _generated

from migration_enum import StrEnum # noqa: E402, F401

class PyAdministratorLevels(StrEnum):
    """The Administrator level for the user to be derived from a siterole in SiteRoles."""
    
    """Name for the level when a user has Site administrator permissions."""
    SITE = "Site"
    
    """Name for the level when a user has no administrator permissions."""
    NONE = "None"
    
class PyContentPermissions(StrEnum):
    """Class containing content permissions constants. See https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_projects.htm#create_project for documentation."""
    
    """Gets the name of the LockedToProject content permission mode."""
    LOCKED_TO_PROJECT = "LockedToProject"
    
    """Gets the name of the ManagedByOwner content permission mode."""
    MANAGED_BY_OWNER = "ManagedByOwner"
    
    """Gets the name of the LockedToProjectWithoutNested content permission mode."""
    LOCKED_TO_PROJECT_WITHOUT_NESTED = "LockedToProjectWithoutNested"
    
class PyExtractEncryptionModes(StrEnum):
    """Class containing extract encryption mode constants. See https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_extract_and_encryption.htm for documentation."""
    
    """The mode to enforce encryption of all extracts on the site."""
    ENFORCED = "enforced"
    
    """The mode to allow users to specify to encrypt all extracts associated with specific published workbooks or data sources."""
    ENABLED = "enabled"
    
    """The mode to disable extract encryption on the site."""
    DISABLED = "disabled"
    
class PyLabelCategories(StrEnum):
    """The built-in categories for labels."""
    
    """Gets the name of the Certification category for a label."""
    CERTIFICATION = "Certification"
    
    """Gets the name of the Data Quality Warning category for a label."""
    DATA_QUALITY_WARNING = "DataQualityWarning"
    
    """Gets the name of the Sensitivity category for a label."""
    SENSITIVITY = "Sensitivity"
    
class PyLicenseLevels(StrEnum):
    """The license levels to be derived from a siterole in SiteRoles."""
    
    """Gets the name of the Creator license level."""
    CREATOR = "Creator"
    
    """Gets the name of the Explorer license level."""
    EXPLORER = "Explorer"
    
    """Gets the name of the Viewer license level."""
    VIEWER = "Viewer"
    
    """Gets the name of the license level when a user is unlicensed."""
    UNLICENSED = "Unlicensed"
    
class PyPermissionsCapabilityModes(StrEnum):
    """The capability modes for a specific content type."""
    
    """The name of the Allow capability mode."""
    ALLOW = "Allow"
    
    """The name of the Deny capability mode."""
    DENY = "Deny"
    
class PyPermissionsCapabilityNames(StrEnum):
    """Enumeration class for the various capability names used in REST API permissions."""
    
    """Gets the name of the None capability."""
    NONE = "None"
    
    """Gets the name of the "Add Comment" capability."""
    ADD_COMMENT = "AddComment"
    
    """Gets the name of the "Change Hierarchy" capability."""
    CHANGE_HIERARCHY = "ChangeHierarchy"
    
    """Gets the name of the "Change Permissions" capability."""
    CHANGE_PERMISSIONS = "ChangePermissions"
    
    """Gets the name of the "Connect" capability."""
    CONNECT = "Connect"
    
    """Gets the name of the "Create Refresh Metrics" capability."""
    CREATE_REFRESH_METRICS = "CreateRefreshMetrics"
    
    """Gets the name of the "Delete" capability."""
    DELETE = "Delete"
    
    """Gets the name of the "Execute" capability."""
    EXECUTE = "Execute"
    
    """Gets the name of the "Export Data" capability."""
    EXPORT_DATA = "ExportData"
    
    """Gets the name of the "Export Image" capability."""
    EXPORT_IMAGE = "ExportImage"
    
    """Gets the name of the "Export XML" capability."""
    EXPORT_XML = "ExportXml"
    
    """Gets the name of the "Extract Refresh" capability."""
    EXTRACT_REFRESH = "ExtractRefresh"
    
    """Gets the name of the "Filter" capability."""
    FILTER = "Filter"
    
    """Gets the name of the "Inherited Project Leader" capability."""
    INHERITED_PROJECT_LEADER = "InheritedProjectLeader"
    
    """Gets the name of the "Project Leader" capability."""
    PROJECT_LEADER = "ProjectLeader"
    
    """Gets the name of the "Read" capability."""
    READ = "Read"
    
    """Gets the name of the "Run Explain Data" capability."""
    RUN_EXPLAIN_DATA = "RunExplainData"
    
    """Gets the name of the "Save As" capability."""
    SAVE_AS = "SaveAs"
    
    """Gets the name of the "Share View" capability."""
    SHARE_VIEW = "ShareView"
    
    """Gets the name of the "View Comments" capability."""
    VIEW_COMMENTS = "ViewComments"
    
    """Gets the name of the "View Underlying Data" capability."""
    VIEW_UNDERLYING_DATA = "ViewUnderlyingData"
    
    """Gets the name of the "Web Authoring" capability."""
    WEB_AUTHORING = "WebAuthoring"
    
    """Gets the name of the "Web Authoring" capability for flows."""
    WEB_AUTHORING_FOR_FLOWS = "WebAuthoringForFlows"
    
    """Gets the name of the "Write" capability."""
    WRITE = "Write"
    
class PySiteRoles(StrEnum):
    """Class containing site role constants. See https://help.tableau.com/current/server/en-us/users_site_roles.htm#site-role-capabilities-summary for documentation."""
    
    """Gets the name of the Creator site role."""
    CREATOR = "Creator"
    
    """Gets the name of the Explorer site role."""
    EXPLORER = "Explorer"
    
    """Gets the name of the Explorer Can Publish site role."""
    EXPLORER_CAN_PUBLISH = "ExplorerCanPublish"
    
    """Gets the name of the Guest site role."""
    GUEST = "Guest"
    
    """Gets the name of the Server Administrator site role."""
    SERVER_ADMINISTRATOR = "ServerAdministrator"
    
    """Gets the name of the Site Administrator Creator site role."""
    SITE_ADMINISTRATOR_CREATOR = "SiteAdministratorCreator"
    
    """Gets the name of the Site Administrator Explorer site role."""
    SITE_ADMINISTRATOR_EXPLORER = "SiteAdministratorExplorer"
    
    """Gets the name of the Support User site role."""
    SUPPORT_USER = "SupportUser"
    
    """Gets the name of the Unlicensed site role."""
    UNLICENSED = "Unlicensed"
    
    """Gets the name of the Viewer site role."""
    VIEWER = "Viewer"
    

# endregion

