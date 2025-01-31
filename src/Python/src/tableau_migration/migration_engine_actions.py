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

"""Wrapper for classes in Tableau.Migration.Engine.Actions namespace."""

# region _generated

from tableau_migration.migration import PyResult # noqa: E402, F401
from typing_extensions import Self # noqa: E402, F401

from Tableau.Migration.Engine.Actions import IMigrationActionResult # noqa: E402, F401

class PyMigrationActionResult(PyResult):
    """IResult object for a migration action."""
    
    _dotnet_base = IMigrationActionResult
    
    def __init__(self, migration_action_result: IMigrationActionResult) -> None:
        """Creates a new PyMigrationActionResult object.
        
        Args:
            migration_action_result: A IMigrationActionResult object.
        
        Returns: None.
        """
        self._dotnet = migration_action_result
        
    @property
    def perform_next_action(self) -> bool:
        """Gets whether or not to perform the next action in the pipeline."""
        return self._dotnet.PerformNextAction
    
    def for_next_action(self, perform_next_action: bool) -> Self:
        """Creates a new PerformNextAction value.
        
        Args:
            perform_next_action: Whether or not to perform the next action in the pipeline.
        
        Returns: The new IMigrationActionResult object.
        """
        result = self._dotnet.ForNextAction(perform_next_action)
        return None if result is None else PyMigrationActionResult(result)
    

# endregion

