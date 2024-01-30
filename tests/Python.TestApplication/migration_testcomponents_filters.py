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

"""Filters for the Python.TestApplication."""

import helper
import logging

from Tableau.Migration.Api.Rest.Models import LicenseLevels
from Tableau.Migration.Content import IUser, IGroup, IProject, IDataSource, IWorkbook
from Tableau.Migration.Interop.Hooks.Filters import ISyncContentFilter
from Tableau.Migration.Engine.Hooks.Filters import ContentFilterBase


class PySpecialUserFilter(ContentFilterBase[IUser]):
    """A class to filter special users."""

    __namespace__ = "Python.TestApplication"
    _dotnet_base = ContentFilterBase[IUser]
    
    def __init__(self):
        """Default init to set up logging."""
        self._logger = logging.getLogger(self.__class__.__name__)
        self._logger.setLevel(logging.DEBUG)

    def ShouldMigrate(self,item):   # noqa: N802
        """Implements ShouldMigrate from base."""
        # Need to improve this to be an array not a single item
        if item.SourceItem.Email in helper.config['special_users']['emails']:
            self._logger.debug('%s filtered %s', self.__class__.__name__, item.SourceItem.Email)
            return False
        
        return True
        
    
class PyUnlicensedUserFilter(ContentFilterBase[IUser]):
    """A class to filter unlicensed users."""

    __namespace__ = "Python.TestApplication"
    _dotnet_base = ContentFilterBase[IUser]
    
    def __init__(self):
        """Default init to set up logging."""
        self._logger = logging.getLogger(self.__class__.__name__)
        self._logger.setLevel(logging.DEBUG)
    
    def ShouldMigrate(self,item):   # noqa: N802
        """Implements ShouldMigrate from base."""
        if item.SourceItem.LicenseLevel == LicenseLevels.Unlicensed:
            self._logger.debug('%s filtered %s',self.__class__.__name__, item.SourceItem.Email)
            return False
        
        return True
    


class PySkipFilter_Users(ContentFilterBase[IUser]): # noqa: N801
    """A class to filter all users."""

    __namespace__ = "Python.TestApplication"
    _dotnet_base = ContentFilterBase[IUser]

    def __init__(self):
        """Default init to set up logging."""
        self.logger = logging.getLogger(self.__class__.__name__)
        self.logger.setLevel(logging.DEBUG)

    def ShouldMigrate(self,item):   # noqa: N802
        """Implements ShouldMigrate from base."""
        self.logger.debug('%s is filtering %s', self.__class__.__name__, item.SourceItem.Email)
        return False
    
    
class PySkipFilter_Groups(ContentFilterBase[IGroup]):    # noqa: N801
    """A class to filter all groups."""

    __namespace__ = "Python.TestApplication"
    _dotnet_base = ContentFilterBase[IGroup]
    
    def ShouldMigrate(self,item):   # noqa: N802
        """Implements ShouldMigrate from base."""
        return False
         

class PySkipFilter_Projects(ContentFilterBase[IProject]):    # noqa: N801
    """A class to filter all projects."""

    __namespace__ = "Python.TestApplication"
    _dotnet_base = ContentFilterBase[IProject]
    
    def ShouldMigrate(self,item):   # noqa: N802
        """Implements ShouldMigrate from base."""
        return False
         

class PySkipFilter_DataSources(ContentFilterBase[IDataSource]):  # noqa: N801
    """A class to filter all data sources."""

    __namespace__ = "Python.TestApplication"
    _dotnet_base = ContentFilterBase[IDataSource]
    
    def ShouldMigrate(self,item):   # noqa: N802
        """Implements ShouldMigrate from base."""
        return False
         

class PySkipFilter_Workbooks(ContentFilterBase[IWorkbook]):  # noqa: N801
    """A class to filter all workbooks."""

    __namespace__ = "Python.TestApplication"
    _dotnet_base = ContentFilterBase[IWorkbook]
    
    def ShouldMigrate(self,item):   # noqa: N802
        """Implements ShouldMigrate from base."""
        return False
