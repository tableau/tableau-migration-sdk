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

"""Wrapper for classes in Tableau.Migration.Api.Rest.Models.Types namespace."""

# region _generated

from migration_enum import StrEnum # noqa: E402, F401

class PyAuthenticationTypes(StrEnum):
    """Class containing authentication type constants.  See https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_users_and_groups.htm#add_user_to_site for documentation."""
    
    """Gets the name of the server default authentication type."""
    SERVER_DEFAULT = "ServerDefault"
    
    """Gets the name of the Open ID authentication type."""
    OPEN_ID = "OpenID"
    
    """Gets the name of the SAML authentication type."""
    SAML = "SAML"
    
    """Gets the name of the Tableau ID with MFA authentication type."""
    TABLEAU_ID_WITH_MFA = "TableauIDWithMFA"
    
class PyDataSourceFileTypes(StrEnum):
    """Class containing data source file type constants.  See https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_data_sources.htm#publish_data_source for documentation."""
    
    """Gets the name of the Hyper data source file type."""
    HYPER = "hyper"
    
    """Gets the name of the Tds data source file type."""
    TDS = "tds"
    
    """Gets the name of the Tdsx data source file type."""
    TDSX = "tdsx"
    
    """Gets the name of the Tde data source file type."""
    TDE = "tde"
    
class PyWorkbookFileTypes(StrEnum):
    """Class containing workbook file type constants.  See https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_publishing.htm#publish_workbook for documentation."""
    
    """Gets the name of the twb workbook file type."""
    TWB = "twb"
    
    """Gets the name of the twbx workbook file type."""
    TWBX = "twbx"
    

# endregion

