# Copyright (c) 2024, Salesforce, Inc.
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
from typing import (
    Generic,
    TypeVar
)
from tableau_migration import (
    ContentLocation,
    ContentMappingBase,
    ContentMappingContext,
    IDataSource,
    IProject,
    IUser,
    IWorkbook,
    LicenseLevels,
    TableauCloudUsernameMappingBase
)

class TestTableauCloudUsernameMapping(TableauCloudUsernameMappingBase):
    """Mapping that takes a base email and appends the source item name to the email username."""

    def __init__(self):
        """Default init to set up logging."""
        self._logger = logging.getLogger(self.__class__.__name__)
        self._logger.setLevel(logging.DEBUG)
        
        self._always_override_address = helper.config['test_tableau_cloud_username_options']['always_override_address']
        
        self._base_username, self._base_domain = helper.config['test_tableau_cloud_username_options']['base_override_mail_address'].split('@')
        
    def map(self, ctx: ContentMappingContext[IUser]) -> ContentMappingContext[IUser]:  # noqa: N802
        """Implements Execute from base."""
        parent_domain = ctx.mapped_location.parent()

        if self._always_override_address is False and ctx.content_item.email:
            ctx = ctx.map_to(parent_domain.append(ctx.content_item.email))
            return ctx

        # Takes the existing "Name" and appends the default domain to build the email
        item_name: str = ctx.content_item.name
        item_name = item_name.replace(' ', '')

        test_email = f'{self._base_username}+{item_name}@{self._base_domain}'
        ctx = ctx.map_to(parent_domain.append(test_email))
        self._logger.debug('Mapped %s to %s', ctx.content_item.email, str(ctx.mapped_location))

        return ctx

class SpecialUserMapping(ContentMappingBase[IUser]):
    """A class to map users to server admin."""

    _admin_username = ContentLocation.for_username(helper.config['special_users']['admin_domain'], helper.config['special_users']['admin_username'])
        
    def __init__(self):
        """Default init to set up logging."""
        self._logger = logging.getLogger(self.__class__.__name__)
        self._logger.setLevel(logging.DEBUG)
    
    def map(self, ctx: ContentMappingContext[IUser]) -> ContentMappingContext[IUser]:
        if ctx.content_item.email in helper.config['special_users']['emails']:
            ctx = ctx.map_to(self._admin_username)        
            self._logger.debug('Mapped %s to %s', ctx.content_item.email, str(ctx.mapped_location))
        return ctx
    
class UnlicensedUserMapping(ContentMappingBase[IUser]):
    """A class to map unlicensed users to server admin."""

    _admin_username = ContentLocation.for_username(helper.config['special_users']['admin_domain'], helper.config['special_users']['admin_username'])
        
    def __init__(self):
        """Default init to set up logging."""
        self._logger = logging.getLogger(self.__class__.__name__)
        self._logger.setLevel(logging.DEBUG)    
    
    def map(self, ctx: ContentMappingContext[IUser]) -> ContentMappingContext[IUser]:
        if ctx.content_item.license_level == LicenseLevels.UNLICENSED:
            ctx = ctx.map_to(self._admin_username)        
            self._logger.debug('Mapped %s to %s', ctx.content_item.email, str(ctx.mapped_location))

        return ctx

TContent = TypeVar("TContent")

class _ContentWithinSkippedLocationMapping(Generic[TContent]):
    """Generic base mapping wrapper for content within skipped location."""
    
    def __init__(self, logger_name: str):
        """Default init to set up logging."""
        self._logger = logging.getLogger(logger_name)
        self._logger.setLevel(logging.DEBUG)

    def map(self, ctx: ContentMappingContext[TContent], services) -> ContentMappingContext[TContent]:
        """Executes the mapping.
        
        Args:
            ctx: The input context from the migration engine or previous hook.
            
        Returns:
            The context, potentially modified to pass on to the next hook or migration engine, or None to continue passing the input context.
        """
        if helper.config['skipped_project'] == '':
            return ctx

        path_replaced = ctx.content_item.location.path.replace(helper.config['skipped_project'],'')
        path_separator = ctx.content_item.location.path_separator

        if not ctx.content_item.location.path.startswith(helper.config['skipped_project']) or \
           path_replaced == '' or \
           len(path_replaced.split(path_separator)) <= 2: # considering the first empty value before the first slash
            return ctx
        
        destination_project_finder = services.get_destination_finder(IProject)

        mapped_destination = ContentLocation.from_path(helper.config['skipped_parent_destination'], path_separator)

        project_reference = destination_project_finder.find_by_mapped_location(mapped_destination)

        if project_reference is None:
            self._logger.error('Cannot map %s "%s" that belongs to "%s" to the project "%s". You must create the destination location first.', self.__orig_class__.__args__[0].__name__, ctx.content_item.name, helper.config['skipped_project'], helper.config['skipped_parent_destination'])
            return ctx
            
        mapped_list = list(mapped_destination.path_segments)
        
        for i in range(len(helper.config['skipped_project'].split(ctx.content_item.location.path_separator))+1,len(ctx.content_item.location.path_segments)):
            mapped_list.append(ctx.content_item.location.path_segments[i])

        self._logger.info('Mapping the %s "%s" that belongs to "%s" to the project "%s" (Id: %s).', self.__orig_class__.__args__[0].__name__, ctx.content_item.name, helper.config['skipped_project'], helper.config['skipped_parent_destination'], project_reference.id)

        ctx = ctx.map_to(ContentLocation.from_path(path_separator.join(mapped_list),path_separator))

        return ctx
    
class ProjectWithinSkippedLocationMapping(ContentMappingBase[IProject]):
    """A class to map projects within skipped project to a configured destination project."""

    def __init__(self):
        """Default init to set up wrapper."""
        self._mapper = _ContentWithinSkippedLocationMapping[IProject](self.__class__.__name__)
    
    def map(self, ctx: ContentMappingContext[IProject]) -> ContentMappingContext[IProject]:
        return self._mapper.map(ctx, self.services)
    
class DataSourceWithinSkippedLocationMapping(ContentMappingBase[IDataSource]):
    """A class to map datasources within skipped project to a configured destination project."""

    def __init__(self):
        """Default init to set up wrapper."""
        self._mapper = _ContentWithinSkippedLocationMapping[IDataSource](self.__class__.__name__)
    
    def map(self, ctx: ContentMappingContext[IDataSource]) -> ContentMappingContext[IDataSource]:
        return self._mapper.map(ctx, self.services)
    
class WorkbookWithinSkippedLocationMapping(ContentMappingBase[IWorkbook]):
    """A class to map workbooks within skipped project to a configured destination project."""

    def __init__(self):
        """Default init to set up wrapper."""
        self._mapper = _ContentWithinSkippedLocationMapping[IWorkbook](self.__class__.__name__)
    
    def map(self, ctx: ContentMappingContext[IWorkbook]) -> ContentMappingContext[IWorkbook]:
        return self._mapper.map(ctx, self.services)
