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

"""Wrapper for classes in Tableau.Migration namespace.

Also any method required to start the sdk
"""
from typing import Type, TypeVar, List
from typing_extensions import Self
from uuid import UUID

import System 
import System.Collections.Generic

# We are import Tableau.Migration again. This is because IServiceCollectionExtension
# is defined in multiple namespaces. If we were to import it via a 'from import' statement it would
# defined in the default python namespace and would get overriden by the next 'from import' statement.

from Tableau.Migration import (
    IServiceCollectionExtensions as MigrationSCE,
    MigrationResult,
    IResult
)

from Tableau.Migration.Engine.Manifest import (
    IMigrationManifestEditor
)

from enum import IntEnum

from Tableau.Migration.Interop import (
    IServiceCollectionExtensions as InteropSCE)

from Tableau.Migration.Interop.Logging import (
    NonGenericLoggerProvider, 
    NonGenericLoggerBase)

from Microsoft.Extensions.DependencyInjection import (
    ServiceCollectionContainerBuilderExtensions, 
    ServiceCollection,
    IServiceCollection,
    ServiceProviderServiceExtensions,ServiceProvider)


import tableau_migration 
from tableau_migration.migration_logger import MigrationLogger

# region init

T = TypeVar("T")
Action1 = getattr(System, "Action`1")

def get_service(services: System.IServiceProvider, t: Type[T]) -> T:
    """Gets service of type T.

    Args:
        services: The services provider
        t: The type to return
    
    Returns: the object of type T
    """
    return ServiceProviderServiceExtensions.GetRequiredService[t](services)

def get_service_provider() -> ServiceProvider:
    """Gets the Dependency Injection Service Provider.
    
    https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection
    """
    if tableau_migration._services is not None:
        return tableau_migration._services
    else:
        raise Exception("Service provider is not initialized.")

def _initialize() -> ServiceProvider:
    """Initializes the DI container and returns the services provider."""
    tableau_migration._service_collection = ServiceCollection()
    _configure_services(tableau_migration._service_collection) # moving to init
    tableau_migration._services = _build_service_provider()
    return tableau_migration._services

def _build_service_provider() -> ServiceProvider:
    """Gets the IServiceProvider with the Migration SDK registered."""
    tableau_migration._services = ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(tableau_migration._service_collection)
    return tableau_migration._services


def _configure_services(service_collection: IServiceCollection) -> None:
    """Adds migration sdk and python logger to DI."""
    # Create the python logging provider so it's available from the beginning
    MigrationSCE.AddTableauMigrationSdk(service_collection)
    InteropSCE.AddPythonSupport(service_collection, System.Func[System.IServiceProvider, NonGenericLoggerProvider](_get_new_python_logging_provider_delegate))


def _get_new_python_logging_provider_delegate(services: IServiceCollection):
    """Simply returns a new NonGenericLoggerProvider."""
    return NonGenericLoggerProvider(System.Func[System.String, NonGenericLoggerBase](_get_new_python_logger_delegate))


def _get_new_python_logger_delegate(name: str) -> MigrationLogger:
    """Simply returns the python implemented PyLogger of the given name.

    A new logger will be created for every ILogger implementation that is created.
    Example: ILogger<MigrationPlanBuilder>, ILogger<ResourceManagerStringLocalizer>, etc
    
    """
    tableau_migration._logger_names.append(name)
    return MigrationLogger(name)

# endregion

# region objects
class PyMigrationCompletionStatus(IntEnum):
    """Enumeration of the various ways a migration can reach completion."""

    Completed = 0
    """
    The migration reached completion normally.
    """
    Canceled = 1
    """
    The migration was canceled before completion.
    """
    FatalError = 2
    """
    The migration had a fatal error that interrupted completion.
    """


class PyMigrationManifest():
    """Interface for an object that describes the various Tableau data items found to migrate and their migration results."""

    _dotnet_base = IMigrationManifestEditor

    def __init__(self, migration_manifest: IMigrationManifestEditor) -> None:
        """Default init.

        Args:
            migration_manifest: IMigrationManifest that can be edited
            
        Returns: None.
        """
        self._migration_manifest = migration_manifest

    @property
    def plan_id(self) -> UUID:
        """Gets the unique identifier of the IMigrationPlan that was executed to produce this manifest."""
        return UUID(self._migration_manifest.PlanId.ToString())

    @property
    def migration_id(self) -> UUID:
        """Gets the unique identifier of the migration run that produced this manifest."""
        return UUID(self._migration_manifest.MigrationId.ToString())

    @property
    def manifest_version(self) -> int:
        """Gets the version of this manifest. Used for serialization."""
        return self._migration_manifest.ManifestVersion

    @property
    def errors(self):
        """Gets top-level errors that are not related to any Tableau content item but occurred during the migration."""
        return self._migration_manifest.Errors

    @property
    def entries(self):
        """Gets the collection of manifest entries."""
        return self._migration_manifest.Entries

    def add_errors(self, errors) -> Self:
        """Adds top-level errors that are not related to any Tableau content item.

        Args:
            errors: The errors to add. Either a List[System.Exception] or System.Exception

        Returns: This manifest editor, for fluent API usage.
        """
        # If a list is passed in, marshal it to a dotnet list and pass it on
        if(isinstance(errors, List)):
            marshalled_error = System.Collections.Generic.List[System.Exception]()
            [marshalled_error.Add(error) for error in errors]
            self._migration_manifest.AddErrors(marshalled_error)
            return

        # If somethign else is passed in, let dotnet handle it. 
        # It's either valid and it works
        # or an exception will be thrown
        self._migration_manifest.AddErrors(errors)

class PyMigrationResult():
    """Interface for a result of a migration."""

    _dotnet_base = MigrationResult # MigrationResult is a record struct, not an interface, hence it doesn't start with I

    def __init__(self, migration_result: MigrationResult) -> None:
        """Default init.

        Args:
            migration_result: Interface for a result of a migration.
        
        Returns: None.
        """
        self._migrationResult = migration_result

    @property
    def status(self) -> PyMigrationCompletionStatus:
        """How the migration reached completion."""
        return PyMigrationCompletionStatus(self._migrationResult.Status.value__)

    @property 
    def manifest(self) -> PyMigrationManifest:
        """Gets the MigrationManifest the migration produced."""
        return PyMigrationManifest(self._migrationResult.Manifest)

class PyResult():
    """Interface representing the result of an operation."""

    _dotnet_base = IResult
        
    def __init__(self, result: IResult) -> None:
        """Default init.

        Args:
            result: Representation of the result of an operation
        
        Returns: None.
        """
        self._result = result

    """
    Interface representing the result of an operation.
    """
    @property
    def success(self) -> bool:
        """Gets whether the operation was successful."""
        return self._result.Success

    @property
    def errors(self):
        """Gets any exceptions encountered during the operation."""
        return self._result.Errors

# endregion
