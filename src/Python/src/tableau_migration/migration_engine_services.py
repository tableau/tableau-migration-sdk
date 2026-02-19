# Copyright (c) 2026, Salesforce, Inc.
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

"""Wrapper for classes in Tableau.Migration.Engine.Services namespace."""

from abc import abstractmethod
import inspect
import sys
from typing import Any, Callable, Generic, get_args, Sequence, Tuple, TypeVar, Union
from typing_extensions import Self

from tableau_migration.migration import _generic_wrapper_type
from tableau_migration.migration_interop import (
    _get_type_args,
    _unwrap,
    _PyWrapperBuilderBase,
)

import System # noqa: E402
from System import ( # noqa: E402
    IServiceProvider
)

from Tableau.Migration.Engine.Services import ( # noqa: E402
    MigrationServiceFactory,
    MigrationServiceFactoryContext,
    IMigrationServiceBuilder,
    IMigrationServiceFactoryCollection
)

from Tableau.Migration.Engine.Endpoints import ( # noqa: E402
    IMigrationContentLoader
)

# region Service Wrapper Utility

T = TypeVar("T")

class _PyServiceWrapperBuilderBase(_PyWrapperBuilderBase):

    @staticmethod
    @abstractmethod
    def dotnet_service() -> type:
        ...


    def __init__(self, inner_type: type) -> None:
        super().__init__(inner_type)


    def get_wrapper_base_type(self) -> type:
        return self.dotnet_service()[self.dotnet_generic_types]


_service_wrapper_builder_types = { }


def _find_service_wrapper_builder_type(dotnet_service: System.Type) -> Union[type, None]:
    for module in list(sys.modules.values()):
        for member in inspect.getmembers(module, inspect.isclass):
            t = member[1]
            if hasattr(t, "dotnet_service") and dotnet_service.Equals(t.dotnet_service()):
                return t
    
    return None


def _service_wrapper_builder_type(dotnet_service: System.Type) -> type:
    if dotnet_service not in _service_wrapper_builder_types:
        wrapper_type = _find_service_wrapper_builder_type(dotnet_service)
        _service_wrapper_builder_types[dotnet_service] = wrapper_type
        
    return _service_wrapper_builder_types[dotnet_service]


_service_types = { }


def _find_service_type(wrapper: type) -> Union[type, None]:
    for module in list(sys.modules.values()):
        for member in inspect.getmembers(module, inspect.isclass):
            t = member[1]
            if hasattr(t, "_wrapper_builder") and t._wrapper_builder == wrapper:
                return t
    
    return None

def _service_type(wrapper: type) -> type:
    if wrapper not in _service_types:
        service_type = _find_service_type(wrapper)
        _service_types[wrapper] = service_type
        
    return _service_types[wrapper]


def _get_dotnet_service(service_type: type) -> type:
    dotnet_type = service_type._wrapper_builder.dotnet_service()

    generic_args = get_args(service_type)
    if generic_args == ():
        return dotnet_type
    else:
        return dotnet_type[tuple([t._dotnet_base if hasattr(t, "_dotnet_base") else t for t in generic_args])]


def _get_python_type_argument(t):
    dotnet_type = _generic_wrapper_type(t)
    if dotnet_type is None:
        return t
    else:
        return dotnet_type

class PyServiceType():
    """A service type that can be overriden."""


    def __init__(self, ty: System.Type) -> None:
        """Default init.

        Args:
            ty: The .NET service type.
        
        Returns: None.
        """
        self._dotnet = ty
        self._wrapper_builder = _service_wrapper_builder_type(ty)
        self._py = _service_type(self._wrapper_builder)


    @property
    def dotnet(self) -> System.Type:
        """Gets the .NET type the service is called through."""
        return self._dotnet


    @property
    def service(self) -> type:
        """Gets the Python type to inherit from to implement the service."""
        return self._py


    @property
    def name(self) -> str:
        """Gets the name of the service."""
        return self._py.Name


    def __repr__(self) -> str:
        """Gets the string representation of the service."""
        return self.name


class PyMigrationServiceFactoryContext():
    """Context object for service factories called through an IMigrationServiceFactoryCollection."""

    _dotnet_base = MigrationServiceFactoryContext

    def __init__(self, ctx: MigrationServiceFactoryContext) -> None:
        """Default init.

        Args:
            ctx: The context to wrap.
        
        Returns: None.
        """
        self._dotnet = ctx

    @property
    def services(self):
        """Gets the endpoint or migration scoped service provider."""
        from tableau_migration.migration_services import ScopedMigrationServices
        return ScopedMigrationServices(self._dotnet.Services)

    
    @property
    def type_arguments(self) -> Tuple[type, ...]:
        """Gets the type arguments if the service is generic."""
        if not self._dotnet.Type.IsGenericType:
            return ()
        else:
            return tuple([_get_python_type_argument(t) for t in self._dotnet.Type.GenericTypeArguments])


PyMigrationServiceFactory = Callable[[PyMigrationServiceFactoryContext], Any]


def _wrap_service_factory(factory: PyMigrationServiceFactory) -> MigrationServiceFactory:
    def wrapped_service_factory(ctx):
        wrapped_ctx = PyMigrationServiceFactoryContext(ctx)
        return factory(wrapped_ctx)

    return MigrationServiceFactory(wrapped_service_factory)


def _create_service_wrapper_factory(service: type) -> PyMigrationServiceFactory:
    def service_wrapper_factory(ctx: PyMigrationServiceFactoryContext):
        # Check for open generics, and populate type arguments based on what was requested.
        service_type_args = [t for t in _get_type_args(service) if type(t) is TypeVar]
        if not service_type_args:
            wrapper_builder = service._wrapper_builder(service) # non-generic, or closed generic
        else:
            wrapper_builder = service._wrapper_builder(service[ctx.type_arguments]) # open generic

        return wrapper_builder.factory(ctx.services)

    return service_wrapper_factory

# endregion

# region Content Loader
class _PyMigrationContentLoaderWrapperBuilderBase(_PyServiceWrapperBuilderBase):
    """Base class for migration content loader wrapper builders."""
    
    def _build_get_migration_content_pager_method(self):
        """Build the GetMigrationContentPager method wrapper."""
        def get_pager_method(wrapper, page_size):
            
            python_instance = wrapper._inner
            # Call the get_migration_content_pager method
            result = python_instance.get_migration_content_pager(page_size)
            return _unwrap(result)
        return get_pager_method

class _PyMigrationContentLoaderWrapperBuilder(_PyMigrationContentLoaderWrapperBuilderBase):
    
    @staticmethod
    def dotnet_service() -> type:
        return IMigrationContentLoader

    @property
    def python_content_type(self) -> type:
        return self.python_generic_types[0]

    @property
    def dotnet_content_type(self) -> type:
        return self.dotnet_generic_types[0]
    
    def get_wrapper_base_type(self) -> type:
        return IMigrationContentLoader[self.dotnet_content_type]

    def add_wrapper_members(self, members: dict[str, Any]) -> dict[str, Any]:
        members["GetMigrationContentPager"] = self._build_get_migration_content_pager_method()
        return members

class PyMigrationContentLoaderBase(Generic[T]):
    """Generic base class for migration content loaders that users can inherit from to create custom loaders."""

    _wrapper_builder = _PyMigrationContentLoaderWrapperBuilder

    def get_migration_content_pager(self, page_size: int):
        """Gets a pager for content to consider for migration.

        Args:
            page_size: The page size to configure the pager for.
        
        Returns: The pager.
        """
        pass
    
# endregion


class PyMigrationServiceFactoryCollection():
    """Interface for an object that contains service factory overrides registered for each service type."""

    _dotnet_base = IMigrationServiceFactoryCollection

    def __init__(self, service_collection: IMigrationServiceFactoryCollection) -> None:
        """Default init.

        Args:
            service_collection: The service collection to wrap."/
        
        Returns: None.
        """
        self._dotnet = service_collection


    def get_service_factory(self, service_type: type) -> Union[MigrationServiceFactory, None]:
        """Gets the service factory override for the given service type.

        Args:
            service_type: The service type.

        Returns: A service factory to use to get the service, or null to use the default service.
        """
        return self._dotnet.GetServiceFactory(_get_dotnet_service(service_type))


    def get_service(self, service_type: type, services: IServiceProvider):
        """Gets a service, either from a registered service factory override, or from the service provider as a fallback.

        Args:
            service_type: The service to get.
            services: The service provider.

        Returns: The service.
        """
        return self._dotnet.GetService[_get_dotnet_service(service_type)](services)


class PyMigrationServiceBuilder(PyMigrationServiceFactoryCollection):
    """Interface for an object that can register service factory overrides for supported service types."""

    _dotnet_base = IMigrationServiceBuilder

    def __init__(self, service_builder: IMigrationServiceBuilder) -> None:
        """Default init.

        Args:
            service_builder: The service builder to wrap."/
        
        Returns: None.
        """
        self._dotnet = service_builder
        self._supported_services = [PyServiceType(s) for s in self._dotnet.SupportedServices]


    @property
    def supported_services(self) -> Sequence[PyServiceType]:
        """Gets the list of service types that can be overriden with this service builder."""
        return self._supported_services


    def remove(self, service_type: type) -> Self:
        """Removes any previously registered service override for the given service type.

        Args:
            service_type: The service type to remove any override for.

        Returns: This service builder, for fluent API usage.
        """
        self._dotnet.Remove(_get_dotnet_service(service_type))
        return self


    def set(self, service_type: type, service_factory: Union[type, PyMigrationServiceFactory]) -> Self:
        """Overrides a service for the given service type.

        Args:
            service_type: The service type to override.
            service_factory: The service type to use, or a service factory to create the service with.

        Returns: This service builder, for fluent API usage.
        """
        if inspect.isclass(service_factory):
            factory = _wrap_service_factory(_create_service_wrapper_factory(service_factory))
        else:
            factory = _wrap_service_factory(service_factory)

        self._dotnet.Set(_get_dotnet_service(service_type), factory)
        return self

