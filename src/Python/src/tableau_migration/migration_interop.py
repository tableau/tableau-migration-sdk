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

"""Interoperability utility."""

from abc import ABC, abstractmethod
from typing import Any, Callable, get_args, Optional, Union
from uuid import uuid4

from System import IServiceProvider

def _get_type_args(t: Union[type, Any]) -> tuple[type, ...]:
    if hasattr(t, "__orig_bases__"):
        return get_args(t.__orig_bases__[0])
    else:
        return get_args(t)


def _init_wrapper_object(inner_type: type, python_generic_types: tuple[type, ...], dotnet_generic_types: tuple[type, ...], extra_init: Union[Callable, None]) -> Callable:
    from tableau_migration.migration_services import ScopedMigrationServices
    
    def _init(self, scoped_services: IServiceProvider) -> None:
        self.python_generic_types = python_generic_types
        self.dotnet_generic_types = dotnet_generic_types
        self._inner = inner_type()
        self._inner.services = ScopedMigrationServices(scoped_services)
        self._inner.python_generic_types = python_generic_types
        self._inner.dotnet_generic_types = dotnet_generic_types
        
        if(extra_init is not None):
            extra_init(self)

    return _init


def _wrapper_factory(t: type) -> Callable:
    def _create(scoped_services: IServiceProvider):
        return t(scoped_services)
    
    return _create


class _PyWrapperBuilderBase(ABC):


    @abstractmethod
    def get_wrapper_base_type(self) -> type:
        ...


    def __init__(self, inner_type: Optional[type]) -> None:

        self.inner_type = inner_type

        self.python_generic_types = self.get_python_generic_types()
        self.dotnet_generic_types = tuple([t._dotnet_base if hasattr(t, "_dotnet_base") else t for t in self.python_generic_types])

        base_types = (self.get_wrapper_base_type(),)
        base_types = self.add_extra_wrapper_base_types(base_types)

        members = {
            "__namespace__": self.get_wrapper_namespace(),
            "__init__": self.get_wrapper_init()
        }

        members = self.add_wrapper_members(members)        

        self.wrapper_type = type(self.get_wrapper_type_name(), base_types, members)
        self.factory = _wrapper_factory(self.wrapper_type)


    def _enforce_inner_type(self) -> type:
        if self.inner_type is None:
            raise Exception("Wrapper inner type required.")

        return self.inner_type


    def get_python_generic_types(self) -> tuple[type, ...]:
        return _get_type_args(self._enforce_inner_type())


    def get_wrapper_namespace(self) -> str:
        return self.inner_type.__module__


    def get_wrapper_type_name(self) -> str:
        return self._enforce_inner_type().__name__ + "_InteropWrapper_" + str(uuid4())


    def get_wrapper_init(self) -> Callable:
        return _init_wrapper_object(self._enforce_inner_type(), self.python_generic_types, self.dotnet_generic_types, self.add_extra_wrapper_init())


    def add_extra_wrapper_base_types(self, types: tuple) -> tuple:
        return types


    def add_extra_wrapper_init(self) -> Union[Callable, None]:
        return None


    def add_wrapper_members(self, members: dict[str, Any]) -> dict[str, Any]:
        return members


def _unwrap(result):
    """Unwrap Python objects to their .NET equivalents."""
    return result._dotnet if hasattr(result, "_dotnet") else result


def _unwrap_async(ctx_type: type, result):
    """Wraps a Python result in a completed Task for C# async interop."""
    from System.Threading.Tasks import Task
    dotnet_result = _unwrap(result)
    return Task.FromResult[ctx_type](dotnet_result)


def _wrap_content_location(location):
    """Wrap C# ContentLocation to Python PyContentLocation."""
    from tableau_migration.migration import PyContentLocation
    return PyContentLocation(location) if location is not None else None


def _wrap_content_reference(reference):
    """Wrap C# IContentReference to Python PyContentReference."""
    from tableau_migration.migration import PyContentReference
    return PyContentReference(reference) if reference is not None else None


def _wrap_guid(guid):
    """Convert C# Guid to Python UUID."""
    from uuid import UUID
    return UUID(str(guid)) if guid is not None else None