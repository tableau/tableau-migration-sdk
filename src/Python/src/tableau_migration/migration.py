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

"""Wrapper for classes in Tableau.Migration namespace.

Also any method required to start the sdk
"""
import inspect
import sys

from typing import Type, TypeVar, List
from typing_extensions import Self
from uuid import UUID

# region Generic Wrapper Helpers

_wrapper_types = { }

def _find_wrapper_type(search_type):
    for module in list(sys.modules.values()):
        for member in inspect.getmembers(module, inspect.isclass):
            t = member[1]
            if hasattr(t, "_dotnet_base") and t._dotnet_base == search_type:
                return t
    
    return None

def _generic_wrapper(obj, type_override: type = None):
    t = type_override if type_override is not None else type(obj)
    if t not in _wrapper_types:
        wrapper_type = _find_wrapper_type(t)
        if wrapper_type is None:
            raise Exception("Migration SDK wrapper type not found for type" + str(t))
        _wrapper_types[t] = wrapper_type
        
    return _wrapper_types[t](obj)

# endregion

import System # noqa: E402
from System import ( # noqa: E402
    IServiceProvider,
    Func,
    String
)

import System.Collections.Generic # noqa: E402

from Tableau.Migration import ( # noqa: E402
    MigrationResult
)

from Tableau.Migration.Engine.Manifest import ( # noqa: E402
    IMigrationManifestEditor
)

from Tableau.Migration.Interop import ( # noqa: E402
    IServiceCollectionExtensions as InteropSCE
)

from Tableau.Migration.Interop.Logging import ( # noqa: E402
    NonGenericLoggerBase
)

from Microsoft.Extensions.DependencyInjection import ( # noqa: E402
    ServiceCollectionContainerBuilderExtensions, 
    ServiceCollection,
    IServiceCollection,
    ServiceProviderServiceExtensions,
    ServiceProvider
)

import tableau_migration # noqa: E402
from tableau_migration.migration_logger import MigrationLogger # noqa: E402

# region init

T = TypeVar("T")

def get_service(services: IServiceProvider, t: Type[T]) -> T:
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


def _initialize() -> None:
    """Initializes the DI container and returns the services provider."""
    tableau_migration._service_collection = ServiceCollection()
    _configure_services(tableau_migration._service_collection) # moving to init
    _build_service_provider(tableau_migration._service_collection)


def _build_service_provider(service_collection: IServiceCollection) -> None:
    """Gets the IServiceProvider with the Migration SDK registered."""
    tableau_migration._services = ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(service_collection)


def _configure_services(service_collection: IServiceCollection) -> None:
    """Adds migration sdk and python logger to DI."""
    # Create the python logging provider so it's available from the beginning
    InteropSCE.AddPythonSupport(service_collection, Func[String, NonGenericLoggerBase](_get_new_python_logger_delegate))


def _get_new_python_logger_delegate(name: str) -> MigrationLogger:
    """Simply returns the python implemented PyLogger of the given name.

    A new logger will be created for every ILogger implementation that is created.
    Example: ILogger<MigrationPlanBuilder>, ILogger<ResourceManagerStringLocalizer>, etc
    
    """
    tableau_migration._logger_names.append(name)
    return MigrationLogger(name)

# endregion

# region objects

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

        # If something else is passed in, let dotnet handle it. 
        # It's either valid and it works
        # or an exception will be thrown
        self._migration_manifest.AddErrors(errors)

# endregion
        
# region _generated

from enum import IntEnum # noqa: E402, F401
from tableau_migration.migration_api_rest import PyRestIdentifiable # noqa: E402, F401
from typing import Sequence # noqa: E402, F401
from typing_extensions import Self # noqa: E402, F401

import System # noqa: E402

from Tableau.Migration import (  # noqa: E402, F401
    ContentLocation,
    IContentReference,
    IResult
)

class PyContentLocation():
    """Structure representing a logical location of a content item on a Tableau site. For example, for workbooks this represents the project path and the workbook name."""
    
    _dotnet_base = ContentLocation
    
    def __init__(self, content_location: ContentLocation) -> None:
        """Creates a new PyContentLocation object.
        
        Args:
            content_location: A ContentLocation object.
        
        Returns: None.
        """
        self._dotnet = content_location
        
    @property
    def path_segments(self) -> Sequence[str]:
        """Gets the individual segments of the location path."""
        return None if self._dotnet.PathSegments is None else list(self._dotnet.PathSegments)
    
    @property
    def path_separator(self) -> str:
        """Gets the separator to use between segments in the location path."""
        return self._dotnet.PathSeparator
    
    @property
    def path(self) -> str:
        """Gets the full path of the location."""
        return self._dotnet.Path
    
    @property
    def name(self) -> str:
        """Gets the non-pathed name of the location."""
        return self._dotnet.Name
    
    @property
    def is_empty(self) -> bool:
        """Gets whether this location reprents an empty path."""
        return self._dotnet.IsEmpty
    
    @classmethod
    def for_username(cls, domain: str, username: str) -> Self:
        """Creates a new ContentLocation value with the standard user/group name separator.
        
        Args:
            domain: The user/group domain.
            username: The user/group name.
        
        Returns: The newly created ContentLocation.
        """
        result = ContentLocation.ForUsername(domain, username)
        return None if result is None else PyContentLocation(result)
    
    @classmethod
    def from_path(cls, content_location_path: str, path_separator: str) -> Self:
        """Creates a new ContentLocation value from a string.
        
        Args:
            content_location_path: The full path of the location.
            path_separator: The separator to use between segments in the location path.
        
        Returns: The newly created ContentLocation.
        """
        result = ContentLocation.FromPath(content_location_path, path_separator)
        return None if result is None else PyContentLocation(result)
    
    def append(self, name: str) -> Self:
        """Creates a new ContentLocation with a new path segment appended.
        
        Args:
            name: The name to append to the path.
        
        Returns: The new ContentLocation with the appended path.
        """
        result = self._dotnet.Append(name)
        return None if result is None else PyContentLocation(result)
    
    def rename(self, new_name: str) -> Self:
        """Creates a new ContentLocation with the last path segment replaced.
        
        Args:
            new_name: The new name to replace the last path segment with.
        
        Returns: The renamed ContentLocation.
        """
        result = self._dotnet.Rename(new_name)
        return None if result is None else PyContentLocation(result)
    
    def parent(self) -> Self:
        """Creates a new ContentLocation with the last path segment removed.
        
        Returns: The new ContentLocation with the parent path.
        """
        result = self._dotnet.Parent()
        return None if result is None else PyContentLocation(result)
    
class PyContentReference(PyRestIdentifiable):
    """Interface for an object that describes information on how to reference an item of content, for example through a Tableau API."""
    
    _dotnet_base = IContentReference
    
    def __init__(self, content_reference: IContentReference) -> None:
        """Creates a new PyContentReference object.
        
        Args:
            content_reference: A IContentReference object.
        
        Returns: None.
        """
        self._dotnet = content_reference
        
    @property
    def content_url(self) -> str:
        """Get the site-unique "content URL" of the content item, or an empty string if the content type does not use a content URL."""
        return self._dotnet.ContentUrl
    
    @property
    def location(self) -> PyContentLocation:
        """Gets the logical location path of the content item, for project-level content this is the project path and the content item name."""
        return None if self._dotnet.Location is None else PyContentLocation(self._dotnet.Location)
    
    @property
    def name(self) -> str:
        """Gets the name of the content item. This is equivalent to the last segment of the Location. Renames should be performed through mapping."""
        return self._dotnet.Name
    
class PyMigrationCompletionStatus(IntEnum):
    """Enumeration of the various ways a migration can reach completion."""
    
    """The migration reached completion normally."""
    COMPLETED = 0
    
    """The migration was canceled before completion."""
    CANCELED = 1
    
    """The migration had a fatal error that interrupted completion."""
    FATAL_ERROR = 2
    
class PyResult():
    """Interface representing the result of an operation."""
    
    _dotnet_base = IResult
    
    def __init__(self, result: IResult) -> None:
        """Creates a new PyResult object.
        
        Args:
            result: A IResult object.
        
        Returns: None.
        """
        self._dotnet = result
        
    @property
    def success(self) -> bool:
        """Gets whether the operation was successful."""
        return self._dotnet.Success
    
    @property
    def errors(self) -> Sequence[System.Exception]:
        """Gets any exceptions encountered during the operation."""
        return None if self._dotnet.Errors is None else list(self._dotnet.Errors)
    

# endregion

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
