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

"""Wrapper for classes in Tableau.Migration.Content.Schedules.Server namespace."""

# region _generated

from tableau_migration.migration import PyContentReference # noqa: E402, F401
from tableau_migration.migration_content_schedules import PySchedule # noqa: E402, F401

from Tableau.Migration.Content.Schedules.Server import IServerSchedule # noqa: E402, F401

class PyServerSchedule(PySchedule, PyContentReference):
    """Interface for server extract refresh schedule."""
    
    _dotnet_base = IServerSchedule
    
    def __init__(self, server_schedule: IServerSchedule) -> None:
        """Creates a new PyServerSchedule object.
        
        Args:
            server_schedule: A IServerSchedule object.
        
        Returns: None.
        """
        self._dotnet = server_schedule
        
    @property
    def type(self) -> str:
        """Gets the schedule's type."""
        return self._dotnet.Type
    
    @property
    def state(self) -> str:
        """Gets the schedule's state."""
        return self._dotnet.State
    
    @property
    def created_at(self) -> str:
        """Gets the schedule's created timestamp."""
        return self._dotnet.CreatedAt
    
    @property
    def updated_at(self) -> str:
        """Gets the schedule's updated timestamp."""
        return self._dotnet.UpdatedAt
    

# endregion

from Tableau.Migration.Content.Schedules.Server import IServerExtractRefreshTask # noqa: E402, F401
from tableau_migration.migration_content_schedules import PyExtractRefreshTask # noqa: E402, F401

class PyServerExtractRefreshTask(PyExtractRefreshTask[PyServerSchedule]):
    """Interface for a server extract refresh task content item."""
    
    _dotnet_base = IServerExtractRefreshTask
    
    def __init__(self, server_extract_refresh_task: IServerExtractRefreshTask) -> None:
        """Creates a new PyServerExtractRefreshTask object.
        
        Args:
            server_extract_refresh_task: A IServerExtractRefreshTask object.
        
        Returns: None.
        """
        self._dotnet = server_extract_refresh_task

