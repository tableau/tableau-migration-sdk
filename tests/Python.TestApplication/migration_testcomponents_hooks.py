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

"""Hooks for the Python.TestApplication."""

import logging
import helper
from typing import TypeVar

import tableau_migration
import tableau_migration.migration

from tableau_migration import (
    ContentBatchMigrationCompletedHookBase,
    IContentBatchMigrationResult,
    MigrationActionCompletedHookBase,
    MigrationManifestSerializer,
    IMigrationActionResult,
    IDataSource,
    IGroup,
    IProject,
    IUser,
    IWorkbook
)


TContent = TypeVar("TContent")

class TimeLoggerAfterActionHook(MigrationActionCompletedHookBase):
    """Logs the time when an action is complete."""

    handler = None
    
    def __init__(self):
        """Default init to set up logging."""
        self._logger = logging.getLogger(self.__class__.__name__)
        self._logger.setLevel(logging.DEBUG)
        
        if TimeLoggerAfterActionHook.handler is not None:
            self._logger.addHandler(TimeLoggerAfterActionHook.handler)
            
    def execute(self, ctx: IMigrationActionResult) -> IMigrationActionResult: # noqa: N802
        """Executes the hook."""
        self._logger.info("Migration action completed.")
        return ctx    

class SaveManifestHookBase(ContentBatchMigrationCompletedHookBase[TContent]):
    """Updates the manifest file after a batch is migrated."""

    def __init__(self) -> None:
        """Default __init__."""        
        self._logger = logging.getLogger(self.__class__.__name__)
        self._logger.setLevel(logging.DEBUG)
        
    def execute(self, ctx: IContentBatchMigrationResult[TContent]) -> IContentBatchMigrationResult[TContent]: # noqa: N802
        """Executes the hook."""
        self._logger.debug("Saving manifest.")
        
        serializer = MigrationManifestSerializer()
        manifest = self.services.get_manifest()
        serializer.save(manifest, helper.manifest_path)
    
class SaveUserManifestHook(SaveManifestHookBase[IUser]):
    """Updates the manifest file after a user batch is migrated."""
    pass

class SaveGroupManifestHook(SaveManifestHookBase[IGroup]):
    """Updates the manifest file after a group batch is migrated."""
    pass

class SaveProjectManifestHook(SaveManifestHookBase[IProject]):
    """Updates the manifest file after a project batch is migrated."""
    pass

class SaveDataSourceManifestHook(SaveManifestHookBase[IDataSource]):
    """Updates the manifest file after a data source batch is migrated."""
    pass

class SaveWorkbookManifestHook(SaveManifestHookBase[IWorkbook]):
    """Updates the manifest file after a workbook batch is migrated."""
    pass