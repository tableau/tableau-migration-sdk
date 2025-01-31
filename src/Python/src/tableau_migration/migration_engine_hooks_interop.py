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

"""Interoperability utility for hooks."""

from abc import ABC, abstractmethod
from inspect import signature
from typing import Callable, Generic, get_args, TypeVar, Union
from uuid import uuid4

from migration_engine_actions import PyMigrationActionResult
from migration_engine_hooks_results import PyInitializeMigrationHookResult
from migration_engine_migrators_batch import PyContentBatchMigrationResult

from System import IServiceProvider
from System.Threading.Tasks import Task
from Tableau.Migration.Engine.Actions import IMigrationActionResult
from Tableau.Migration.Engine.Migrators.Batch import IContentBatchMigrationResult
from Tableau.Migration.Engine.Hooks import IInitializeMigrationHookResult
from Tableau.Migration.Interop.Hooks import ISyncContentBatchMigrationCompletedHook, ISyncInitializeMigrationHook, ISyncMigrationActionCompletedHook

TContent = TypeVar("TContent")

def _wrapper_init_object(inner_hook_type) -> Callable:
    from migration_services import ScopedMigrationServices
    
    def _init(self, scoped_services: IServiceProvider) -> None:
        self._hook = inner_hook_type()
        self._hook.services = ScopedMigrationServices(scoped_services)

    return _init

def _wrapper_init_callback() -> Callable:
    from migration_services import ScopedMigrationServices
    
    def _init(self, scoped_services: IServiceProvider) -> None:
        self.services = ScopedMigrationServices(scoped_services)
        
    return _init
    
def _wrapper_create(t: type) -> Callable:
    def _create(scoped_services: IServiceProvider):
        return t(scoped_services)
    
    return _create

def _unwrap(result):
    return result._dotnet if hasattr(result, "_dotnet") else result

def _unwrap_async(ctx_type: type, result):
    dotnet_result = _unwrap(result)
    return Task.FromResult[ctx_type](dotnet_result)

class _PyHookWrapperBase(ABC):

    @property
    def _wrapper_method_name(self) -> str:
        return "ExecuteAsync"

    @property
    def _wrapper_async(self) -> bool:
        return True

    def __init__(self, t: Union[type, list], callback: Union[Callable, None] = None) -> None:
        if callback is None:
            self.python_generic_types = get_args(t.__orig_bases__[0]) if hasattr(t, "__orig_bases__") else []
            module = t.__module__
            init = _wrapper_init_object(t)
            wrap_type_name = t.__name__ + "_InteropWrapper_" + str(uuid4())
        else:
            self.python_generic_types = t if isinstance(t, list) else [t]
            module = callback.__module__
            init = _wrapper_init_callback()
            wrap_type_name = callback.__name__ + "_InteropWrapper_" + str(uuid4())

        self.dotnet_generic_types = [x._dotnet_base for x in self.python_generic_types]

        members = {
            "__namespace__": module,
            "__init__": init
        }

        wrap_context = self._wrap_context_callback()

        if callback is None:
            members[self._wrapper_method_name] = self.build_wrapper_execute(self._wrap_execute_method(), wrap_context, self._wrapper_context_type(), self._wrapper_async)
        else:
            members[self._wrapper_method_name] = self.build_wrapper_execute_callback(callback, wrap_context, self._wrapper_context_type(), self._wrapper_async)

        self.set_extra_wrapper_members(members, wrap_context)

        base_types = (self._wrapper_base_type(),)
        base_types = self.set_extra_base_types(base_types)

        self.wrapper_type = type(wrap_type_name, base_types, members)
        self.factory = _wrapper_create(self.wrapper_type)
        
    def build_wrapper_execute(self, wrap_method: Callable, wrap_context: Callable, ctx_type: type, is_async: bool) -> Callable:
        def _execute(s, ctx):
            result = wrap_method(s)(wrap_context(ctx))
            return _unwrap(result)
    
        def _execute_async(s, ctx, cancel):
            result = wrap_method(s)(wrap_context(ctx))
            return _unwrap_async(ctx_type, result)

        return _execute_async if is_async else _execute
    
    def build_wrapper_execute_callback(self, callback: Callable, wrap_context: Callable, ctx_type: type, is_async: bool) -> Callable:
        def _execute(s, ctx):
            result = callback(wrap_context(ctx))
            return _unwrap(result)

        def _execute_async(s, ctx, cancel):
            result = callback(wrap_context(ctx))
            return _unwrap_async(ctx_type, result)

        def _execute_services(s, ctx):
            result = callback(wrap_context(ctx), s.services)
            return _unwrap(result)

        def _execute_services_async(s, ctx, cancel):
            result = callback(wrap_context(ctx), s.services)
            return _unwrap_async(ctx_type, result)

        if len(signature(callback).parameters) == 1:
            return _execute_async if is_async else _execute
        else:
            return _execute_services_async if is_async else _execute_services

    def set_extra_wrapper_members(self, members: dict, wrap_context: Callable) -> None:
        pass
    
    def set_extra_base_types(self, types: tuple) -> tuple:
        return types

    @abstractmethod
    def _wrapper_base_type(self) -> type:
        ...

    @abstractmethod
    def _wrapper_context_type(self) -> type:
        ...
        
    @abstractmethod
    def _wrap_execute_method(self) -> Callable:
        ...

    @abstractmethod
    def _wrap_context_callback(self) -> Callable:
        ...
        
class _PyMigrationActionCompletedHookWrapper(_PyHookWrapperBase):

    @property
    def _wrapper_method_name(self) -> str:
        return "Execute"

    @property
    def _wrapper_async(self) -> bool:
        return False

    def _wrapper_base_type(self) -> type:
        return ISyncMigrationActionCompletedHook

    def _wrapper_context_type(self) -> type:
        return IMigrationActionResult

    def _wrap_execute_method(self) -> Callable:
        def _wrap_execute(w):
            return w._hook.execute
        
        return _wrap_execute
    
    def _wrap_context_callback(self) -> Callable:
        def _wrap_context(ctx):
            return PyMigrationActionResult(ctx)
        
        return _wrap_context

class PyMigrationActionCompletedHookBase():
    """Base class for migration action completed hooks."""

    _wrapper = _PyMigrationActionCompletedHookWrapper

    def execute(self, ctx: PyMigrationActionResult) -> PyMigrationActionResult:
        """Executes a hook callback.
        
        Args:
            ctx: The input context from the migration engine or previous hook.
            
        Returns:
            The context, potentially modified to pass on to the next hook or migration engine, or None to continue passing the input context.
        """
        return ctx

class _PyContentBatchMigrationCompletedHookWrapper(_PyHookWrapperBase):

    @property
    def python_content_type(self) -> type:
        return self.python_generic_types[0]

    @property
    def dotnet_content_type(self) -> type:
        return self.dotnet_generic_types[0]
    
    @property
    def _wrapper_method_name(self) -> str:
        return "Execute"

    @property
    def _wrapper_async(self) -> bool:
        return False

    def _wrapper_base_type(self) -> type:
        return ISyncContentBatchMigrationCompletedHook[self.dotnet_content_type]

    def _wrapper_context_type(self) -> type:
        return IContentBatchMigrationResult[self.dotnet_content_type]

    def _wrap_execute_method(self) -> Callable:
        def _wrap_execute(w):
            return w._hook.execute
        
        return _wrap_execute
    
    def _wrap_context_callback(self) -> Callable:
        def _wrap_context(ctx):
            return PyContentBatchMigrationResult[self.python_content_type](ctx)
        
        return _wrap_context

class PyContentBatchMigrationCompletedHookBase(Generic[TContent]):
    """Generic base class for content batch completion hooks."""

    _wrapper = _PyContentBatchMigrationCompletedHookWrapper

    def execute(self, ctx: PyContentBatchMigrationResult[TContent]) -> PyContentBatchMigrationResult[TContent]:
        """Executes a hook callback.
        
        Args:
            ctx: The input context from the migration engine or previous hook.
            
        Returns:
            The context, potentially modified to pass on to the next hook or migration engine, or None to continue passing the input context.
        """
        return ctx

class _PyInitializeMigrationHookWrapper(_PyHookWrapperBase):

    @property
    def _wrapper_method_name(self) -> str:
        return "Execute"

    @property
    def _wrapper_async(self) -> bool:
        return False

    def _wrapper_base_type(self) -> type:
        return ISyncInitializeMigrationHook

    def _wrapper_context_type(self) -> type:
        return IInitializeMigrationHookResult

    def _wrap_execute_method(self) -> Callable:
        def _wrap_execute(w):
            return w._hook.execute
        
        return _wrap_execute
    
    def _wrap_context_callback(self) -> Callable:
        def _wrap_context(ctx):
            return PyInitializeMigrationHookResult(ctx)
        
        return _wrap_context

class PyInitializeMigrationHookBase:
    """Base class for initialize migration hooks."""

    _wrapper = _PyInitializeMigrationHookWrapper

    def execute(self, ctx: PyInitializeMigrationHookResult) -> PyInitializeMigrationHookResult:
        """Executes a hook callback.
        
        Args:
            ctx: The input context from the migration engine or previous hook.
            
        Returns:
            The context, potentially modified to pass on to the next hook or migration engine, or None to continue passing the input context.
        """
        return ctx