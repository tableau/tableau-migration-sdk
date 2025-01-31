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

"""Wrapper for classes in Tableau.Migration.Content.Schedules.Cloud namespace."""

# region _generated

from tableau_migration.migration_content_schedules import PySchedule # noqa: E402, F401

from Tableau.Migration.Content.Schedules.Cloud import ICloudSchedule # noqa: E402, F401

class PyCloudSchedule(PySchedule):
    """Interface for a Tableau Cloud schedule."""
    
    _dotnet_base = ICloudSchedule
    
    def __init__(self, cloud_schedule: ICloudSchedule) -> None:
        """Creates a new PyCloudSchedule object.
        
        Args:
            cloud_schedule: A ICloudSchedule object.
        
        Returns: None.
        """
        self._dotnet = cloud_schedule
        

# endregion

from Tableau.Migration.Content.Schedules.Cloud import ICloudExtractRefreshTask # noqa: E402, F401
from tableau_migration.migration_content_schedules import PyExtractRefreshTask # noqa: E402, F401

class PyCloudExtractRefreshTask(PyExtractRefreshTask[PyCloudSchedule]):
    """Interface for a cloud extract refresh task content item."""
    
    _dotnet_base = ICloudExtractRefreshTask
    
    def __init__(self, cloud_extract_refresh_task: ICloudExtractRefreshTask) -> None:
        """Creates a new PyCloudExtractRefreshTask object.
        
        Args:
            cloud_extract_refresh_task: A ICloudExtractRefreshTask object.
        
        Returns: None.
        """
        self._dotnet = cloud_extract_refresh_task

