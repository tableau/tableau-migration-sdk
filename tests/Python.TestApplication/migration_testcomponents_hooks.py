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

"""Hooks for the Python.TestApplication."""

import logging 
import helper 

import tableau_migration
import tableau_migration.migration
from migration_testcomponents_engine_manifest import PyMigrationManifestSerializer 

from Tableau.Migration.Content import IUser, IGroup, IProject, IDataSource, IWorkbook
from Tableau.Migration.Engine import IMigration
from Tableau.Migration.Engine.Actions import IMigrationActionResult
from Tableau.Migration.Interop.Hooks import ISyncContentBatchMigrationCompletedHook, ISyncMigrationActionCompletedHook

from System import IServiceProvider

# This gets called every time the hook is called, meaning this object
# is created every single time
# This is less then ideal
def save_manifest_after_batch_user_factory(services: IServiceProvider):
    """Factory function to build PySaveManifestAfterBatch_User."""
    migration = services.GetService(IMigration)
    ret = PySaveManifestAfterBatch_User(PyMigrationManifestSerializer(), migration)
    return ret

def save_manifest_after_batch_group_factory(services: IServiceProvider):
    """Factory function to build PySaveManifestAfterBatch_Group."""
    migration = services.GetService(IMigration)
    ret = PySaveManifestAfterBatch_Group(PyMigrationManifestSerializer(), migration)
    return ret

def save_manifest_after_batch_project_factory(services: IServiceProvider):
    """Factory function to build PySaveManifestAfterBatch_Project."""
    migration = services.GetService(IMigration)
    ret = PySaveManifestAfterBatch_Project(PyMigrationManifestSerializer(), migration)
    return ret

def save_manifest_after_batch_datasource_factory(services: IServiceProvider):
    """Factory function to build PySaveManifestAfterBatch_DataSource."""
    migration = services.GetService(IMigration)
    ret = PySaveManifestAfterBatch_DataSource(PyMigrationManifestSerializer(), migration)
    return ret

def save_manifest_after_batch_workbook_factory(services: IServiceProvider):
    """Factory function to build PySaveManifestAfterBatch_Workbook."""
    migration = services.GetService(IMigration)
    ret = PySaveManifestAfterBatch_Workbook(PyMigrationManifestSerializer(), migration)
    return ret


class PyTimeLoggerAfterActionHook(ISyncMigrationActionCompletedHook):
    """Logs the time when an action is complete."""
    
    __namespace__ = "Python.TestApplication"
    _dotnet_base = ISyncMigrationActionCompletedHook
    
    def __init__(self, handler: logging.Handler | None = None):
        """Default init to set up logging."""
        self._logger = logging.getLogger(self.__class__.__name__)
        self._logger.setLevel(logging.DEBUG)
        
        if handler is not None:
            self._logger.addHandler(handler)
            
    def Execute(self, ctx : IMigrationActionResult):       # noqa: N802
        """Implements Execute from base."""
        self._logger.info("Migration action completed")
        return ctx
    

class PySaveManifestAfterBatch_User(ISyncContentBatchMigrationCompletedHook[IUser]):
    """A update the manifest file after a user batch is migrated."""

    __namespace__ = "Python.TestApplication"
    _dotnet_base = ISyncContentBatchMigrationCompletedHook[IUser]
    
    
    def __init__(self, manifest_serializer: PyMigrationManifestSerializer, migration: IMigration) -> None:
        """Default __init__."""
        self._manifest_serializer = manifest_serializer
        self._migration = migration
        
        self._logger = logging.getLogger(self.__class__.__name__)
        self._logger.setLevel(logging.DEBUG)
        

    def Execute(self, ctx):    # noqa: N802
        """Implements Execute from base."""
        self._logger.debug("Saving manifest")
        self._manifest_serializer.save(tableau_migration.migration.PyMigrationManifest(self._migration.Manifest), helper.manifest_path)

         
class PySaveManifestAfterBatch_Group(ISyncContentBatchMigrationCompletedHook[IGroup]):
    """A update the manifest file after group batch is migrated."""

    __namespace__ = "Python.TestApplication"
    _dotnet_base = ISyncContentBatchMigrationCompletedHook[IGroup]
    
    
    def __init__(self, manifest_serializer: PyMigrationManifestSerializer, migration: IMigration) -> None:
        """Default __init__."""
        self._manifest_serializer = manifest_serializer
        self._migration = migration
        
        self._logger = logging.getLogger(self.__class__.__name__)
        self._logger.setLevel(logging.DEBUG)
        

    def Execute(self, ctx):    # noqa: N802
        """Implements Execute from base."""
        self._logger.debug("Saving manifest")
        self._manifest_serializer.save(tableau_migration.migration.PyMigrationManifest(self._migration.Manifest), helper.manifest_path)
        

class PySaveManifestAfterBatch_Project(ISyncContentBatchMigrationCompletedHook[IProject]):
    """A update the manifest file after project batch is migrated."""

    __namespace__ = "Python.TestApplication"
    _dotnet_base = ISyncContentBatchMigrationCompletedHook[IProject]
    
    
    def __init__(self, manifest_serializer: PyMigrationManifestSerializer, migration: IMigration) -> None:
        """Default __init__."""
        self._manifest_serializer = manifest_serializer
        self._migration = migration
        
        self._logger = logging.getLogger(self.__class__.__name__)
        self._logger.setLevel(logging.DEBUG)
        

    def Execute(self, ctx):    # noqa: N802
        """Implements Execute from base."""
        self._logger.debug("Saving manifest")
        self._manifest_serializer.save(tableau_migration.migration.PyMigrationManifest(self._migration.Manifest), helper.manifest_path)
        

class PySaveManifestAfterBatch_DataSource(ISyncContentBatchMigrationCompletedHook[IDataSource]):
    """A update the manifest file after Data Source batch type is migrated."""

    __namespace__ = "Python.TestApplication"
    _dotnet_base = ISyncContentBatchMigrationCompletedHook[IDataSource]
    
    
    def __init__(self, manifest_serializer: PyMigrationManifestSerializer, migration: IMigration) -> None:
        """Default __init__."""
        self._manifest_serializer = manifest_serializer
        self._migration = migration
        
        self._logger = logging.getLogger(self.__class__.__name__)
        self._logger.setLevel(logging.DEBUG)
        

    def Execute(self, ctx):    # noqa: N802
        """Implements Execute from base."""
        self._logger.debug("Saving manifest")
        self._manifest_serializer.save(tableau_migration.migration.PyMigrationManifest(self._migration.Manifest), helper.manifest_path)
        

class PySaveManifestAfterBatch_Workbook(ISyncContentBatchMigrationCompletedHook[IWorkbook]):
    """A update the manifest file after a content type is migrated."""

    __namespace__ = "Python.TestApplication"
    _dotnet_base = ISyncContentBatchMigrationCompletedHook[IWorkbook]
    
    
    def __init__(self, manifest_serializer: PyMigrationManifestSerializer, migration: IMigration) -> None:
        """Default __init__."""
        self._manifest_serializer = manifest_serializer
        self._migration = migration
        
        self._logger = logging.getLogger(self.__class__.__name__)
        self._logger.setLevel(logging.DEBUG)
        

    def Execute(self, ctx):    # noqa: N802
        """Implements Execute from base."""
        self._logger.debug("Saving manifest")
        self._manifest_serializer.save(tableau_migration.migration.PyMigrationManifest(self._migration.Manifest), helper.manifest_path)