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

"""Filters for the Python.TestApplication."""

import helper
import logging

from typing import (
    Generic,
    TypeVar
)
from tableau_migration import (
    ContentFilterBase,
    ContentMigrationItem,
    IDataSource,
    IGroup,
    IProject,
    IServerExtractRefreshTask,
    IUser,
    IWorkbook,
    LicenseLevels
)

class SpecialUserFilter(ContentFilterBase[IUser]):
    """A class to filter special users."""
    
    def __init__(self):
        """Default init to set up logging."""
        self._logger = logging.getLogger(self.__class__.__name__)
        self._logger.setLevel(logging.DEBUG)

    def should_migrate(self, item: ContentMigrationItem[IUser]) -> bool:
        # Need to improve this to be an array not a single item
        if item.source_item.email in helper.config['special_users']['emails']:
            self._logger.debug('%s filtered %s', self.__class__.__name__, item.source_item.email)
            return False
        
        return True        
    
class UnlicensedUserFilter(ContentFilterBase[IUser]):
    """A class to filter unlicensed users."""
    
    def __init__(self):
        """Default init to set up logging."""
        self._logger = logging.getLogger(self.__class__.__name__)
        self._logger.setLevel(logging.DEBUG)
    
    def should_migrate(self, item: ContentMigrationItem[IUser]) -> bool:
        if item.source_item.license_level == LicenseLevels.UNLICENSED:
            self._logger.debug('%s filtered %s',self.__class__.__name__, item.source_item.email)
            return False
        
        return True

class SkipAllUsersFilter(ContentFilterBase[IUser]): # noqa: N801
    """A class to filter all users."""

    def __init__(self):
        """Default init to set up logging."""
        self.logger = logging.getLogger(self.__class__.__name__)
        self.logger.setLevel(logging.DEBUG)

    def should_migrate(self, item: ContentMigrationItem[IUser]) -> bool:
        self.logger.debug('%s is filtering "%s"', self.__class__.__name__, item.source_item.name)
        return False

class SkipAllGroupsFilter(ContentFilterBase[IGroup]): # noqa: N801
    """A class to filter all groups."""

    def __init__(self):
        """Default init to set up logging."""
        self.logger = logging.getLogger(self.__class__.__name__)
        self.logger.setLevel(logging.DEBUG)

    def should_migrate(self, item: ContentMigrationItem[IGroup]) -> bool:
        self.logger.debug('%s is filtering "%s"', self.__class__.__name__, item.source_item.name)
        return False
    
class SkipAllProjectsFilter(ContentFilterBase[IProject]): # noqa: N801
    """A class to filter all projects."""

    def __init__(self):
        """Default init to set up logging."""
        self.logger = logging.getLogger(self.__class__.__name__)
        self.logger.setLevel(logging.DEBUG)

    def should_migrate(self, item: ContentMigrationItem[IProject]) -> bool:
        self.logger.debug('%s is filtering "%s"', self.__class__.__name__, item.source_item.name)
        return False
    
class SkipAllDataSourcesFilter(ContentFilterBase[IDataSource]): # noqa: N801
    """A class to filter all data sources."""

    def __init__(self):
        """Default init to set up logging."""
        self.logger = logging.getLogger(self.__class__.__name__)
        self.logger.setLevel(logging.DEBUG)

    def should_migrate(self, item: ContentMigrationItem[IDataSource]) -> bool:
        self.logger.debug('%s is filtering "%s"', self.__class__.__name__, item.source_item.name)
        return False

class SkipAllWorkbooksFilter(ContentFilterBase[IWorkbook]): # noqa: N801
    """A class to filter all workbooks."""

    def __init__(self):
        """Default init to set up logging."""
        self.logger = logging.getLogger(self.__class__.__name__)
        self.logger.setLevel(logging.DEBUG)

    def should_migrate(self, item: ContentMigrationItem[IWorkbook]) -> bool:
        self.logger.debug('%s is filtering "%s"', self.__class__.__name__, item.source_item.name)
        return False

class SkipAllExtractRefreshTasksFilter(ContentFilterBase[IServerExtractRefreshTask]): # noqa: N801
    """A class to filter all extract refresh tasks."""

    def __init__(self):
        """Default init to set up logging."""
        self.logger = logging.getLogger(self.__class__.__name__)
        self.logger.setLevel(logging.DEBUG)

    def should_migrate(self, item: ContentMigrationItem[IServerExtractRefreshTask]) -> bool:
        self.logger.debug('%s is filtering "%s"', self.__class__.__name__, item.source_item.name)
        return False

TContent = TypeVar("TContent")

class _SkipContentByParentLocationFilter(Generic[TContent]): # noqa: N801
    """Generic base filter wrapper to filter content by parent location."""
    
    def __init__(self, logger_name: str):
        """Default init to set up logging."""
        self._logger = logging.getLogger(logger_name)
        self._logger.setLevel(logging.DEBUG)

    def should_migrate(self, item: ContentMigrationItem[TContent], services) -> bool:
        if item.source_item.location.parent().path != helper.config['skipped_project']:
            return True

        source_project_finder = services.get_source_finder(IProject)

        content_reference = source_project_finder.find_by_source_location(item.source_item.location.parent())

        self._logger.info('Skipping %s that belongs to "%s" (Project ID: %s)', self.__orig_class__.__args__[0].__name__, helper.config['skipped_project'], content_reference.id)
        return False

class SkipProjectByParentLocationFilter(ContentFilterBase[IProject]): # noqa: N801
    """A class to filter projects from a given parent location."""

    def __init__(self):
        """Default init to set up wrapper."""
        self._filter = _SkipContentByParentLocationFilter[IProject](self.__class__.__name__)

    def should_migrate(self, item: ContentMigrationItem[IProject]) -> bool:
        return self._filter.should_migrate(item, self.services)

class SkipDataSourceByParentLocationFilter(ContentFilterBase[IDataSource]): # noqa: N801
    """A class to filter data sources from a given parent location."""

    def __init__(self):
        """Default init to set up wrapper."""
        self._filter = _SkipContentByParentLocationFilter[IDataSource](self.__class__.__name__)

    def should_migrate(self, item: ContentMigrationItem[IDataSource]) -> bool:
        return self._filter.should_migrate(item, self.services)

class SkipWorkbookByParentLocationFilter(ContentFilterBase[IWorkbook]): # noqa: N801
    """A class to filter workbooks from a given parent location."""

    def __init__(self):
        """Default init to set up wrapper."""
        self._filter = _SkipContentByParentLocationFilter[IWorkbook](self.__class__.__name__)

    def should_migrate(self, item: ContentMigrationItem[IWorkbook]) -> bool:
        return self._filter.should_migrate(item, self.services)