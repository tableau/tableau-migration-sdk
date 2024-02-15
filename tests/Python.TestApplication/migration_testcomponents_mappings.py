# Copyright (c) 2023, Salesforce, Inc.
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

"""Mappings for the Python.TestApplication."""

import logging
import helper

from Tableau.Migration import ContentLocation
from Tableau.Migration.Api.Rest.Models import LicenseLevels
from Tableau.Migration.Content import IUser
from Tableau.Migration.Interop.Hooks.Mappings import ISyncContentMapping
from Tableau.Migration.Engine.Hooks.Mappings.Default import ITableauCloudUsernameMapping

class PyTestTableauCloudUsernameMapping(ISyncContentMapping[IUser]):
    """Mapping that takes a base email and appends the source item name to the email username."""
    
    __namespace__ = "Python.TestApplication"
    _dotnet_base = ISyncContentMapping[IUser]

    def __init__(self):
        """Default init to set up logging."""
        self._logger = logging.getLogger(self.__class__.__name__)
        self._logger.setLevel(logging.DEBUG)
        
        self._always_override_address = helper.config['test_tableau_cloud_username_options']['always_override_address']
        
        self._base_username, self._base_domain = helper.config['test_tableau_cloud_username_options']['base_override_mail_address'].split('@')
        
    def Execute(self,ctx):  # noqa: N802
        """Implements Execute from base."""
        parent_domain = ctx.MappedLocation.Parent()

        if self._always_override_address is False and ctx.ContentItem.Email:
            ctx = ctx.MapTo(parent_domain.Append(ctx.ContentItem.Email))
            return ctx

        # Takes the existing "Name" and appends the default domain to build the email
        item_name: str = ctx.ContentItem.Name
        item_name = item_name.replace(' ', '')

        test_email = f'{self._base_username}+{item_name}@{self._base_domain}'
        ctx = ctx.MapTo(parent_domain.Append(test_email))
        self._logger.debug('Mapped %s to %s', ctx.ContentItem.Email, ctx.MappedLocation.ToString())

        return ctx

class PySpecialUserMapping(ISyncContentMapping[IUser]):
    """A class to map users to server admin."""

    __namespace__ = "Python.TestApplication"
    _dotnet_base = ISyncContentMapping[IUser]
    _admin_username = ContentLocation.ForUsername(helper.config['special_users']['admin_domain'], helper.config['special_users']['admin_username'])
        
    def __init__(self):
        """Default init to set up logging."""
        self._logger = logging.getLogger(self.__class__.__name__)
        self._logger.setLevel(logging.DEBUG)
    
    def Execute(self,ctx):  # noqa: N802
        """Implements Execute from base."""
        if ctx.ContentItem.Email in helper.config['special_users']['emails']:
            ctx=ctx.MapTo(self._admin_username)        
            self._logger.debug('Mapped %s to %s', ctx.ContentItem.Email, ctx.MappedLocation.ToString())
        return ctx
    
class PyUnlicensedUserMapping(ISyncContentMapping[IUser]):
    """A class to map unlicensed users to server admin."""

    __namespace__ = "Python.TestApplication"
    _dotnet_base = ISyncContentMapping[IUser]
    _admin_username = ContentLocation.ForUsername(helper.config['special_users']['admin_domain'], helper.config['special_users']['admin_username'])
        
    def __init__(self):
        """Implements Execute from base."""
        self._logger = logging.getLogger(self.__class__.__name__)
        self._logger.setLevel(logging.DEBUG)    
    
    def Execute(self,ctx):  # noqa: N802
        """Implements Execute from base."""
        if ctx.ContentItem.LicenseLevel == LicenseLevels.Unlicensed:
            ctx=ctx.MapTo(self._admin_username)        
            self._logger.debug('Mapped %s to %s', ctx.ContentItem.Email, ctx.MappedLocation.ToString())

        return ctx
