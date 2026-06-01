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

"""Interoperability utility for hooks."""

from abc import abstractmethod
from inspect import signature
from typing import Any, Callable, Generic, Optional, TypeVar, Union
from uuid import uuid4

from tableau_migration.migration_engine_actions import PyMigrationActionResult
from tableau_migration.migration_engine_hooks_initializemigration import PyInitializeMigrationHookResult
from tableau_migration.migration_engine_migrators_batch import PyContentBatchMigrationResult
from tableau_migration.migration_interop import _PyWrapperBuilderBase, _unwrap, _unwrap_async

from System import IServiceProvider
from Tableau.Migration.Engine.Actions import IMigrationActionResult
from Tableau.Migration.Engine.Migrators.Batch import IContentBatchMigrationResult
from Tableau.Migration.Engine.Hooks.InitializeMigration import IInitializeMigrationHookResult
from Tableau.Migration.Interop.Hooks import ISyncContentBatchMigrationCompletedHook, ISyncInitializeMigrationHook, ISyncMigrationActionCompletedHook

TContent = TypeVar("TContent")

def _wrapper_init_callback() -> Callable:
    from tableau_migration.migration_services import ScopedMigrationServices
    
    def _init(self, scoped_services: IServiceProvider) -> None:
        self.services = ScopedMigrationServices(scoped_services)
        
    return _init


class _PyHookWrapperBuilderBase(_PyWrapperBuilderBase):


    @property
    def _wrapper_method_name(self) -> str:
        return "ExecuteAsync"


    @property
    def _wrapper_async(self) -> bool:
        return True


    @property
    def is_callback_hook(self) -> bool:
        return hasattr(self, "callback")


    def __init__(self, t: Union[type, list], callback: Optional[Callable] = None) -> None:
        if callback is None:
            super().__init__(t)
            return
        else:
            self.callback = callback
            self.input_generic_types = t
            super().__init__(None)
        

    def get_python_generic_types(self) -> tuple[type, ...]:
        if self.is_callback_hook:
            types = self.input_generic_types if isinstance(self.input_generic_types, list) else [self.input_generic_types]
            return tuple(types)
        else:
            return super().get_python_generic_types()
            

    def get_wrapper_namespace(self) -> str:
        if self.is_callback_hook:
            return self.callback.__name__
        else:
            return super().get_wrapper_namespace()
            

    def get_wrapper_type_name(self) -> str:
        if self.is_callback_hook:
            return self.callback.__name__ + "_InteropWrapper_" + str(uuid4())
        else:
            return super().get_wrapper_type_name()


    def get_wrapper_init(self) -> Callable:
        if self.is_callback_hook:
            return _wrapper_init_callback()
        else:
            return super().get_wrapper_init()            


    def add_wrapper_members(self, members: dict[str, Any]) -> dict[str, Any]:
        wrap_context = self._wrap_context_callback()

        if self.is_callback_hook:
            members[self._wrapper_method_name] = self.build_wrapper_execute_callback(self.callback, wrap_context, self._wrapper_context_type(), self._wrapper_async)
        else:
            members[self._wrapper_method_name] = self.build_wrapper_execute(self._wrap_execute_method(), wrap_context, self._wrapper_context_type(), self._wrapper_async)

        self.set_extra_wrapper_members(members, wrap_context)

        return members


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


    @abstractmethod
    def _wrapper_context_type(self) -> type:
        ...
        

    @abstractmethod
    def _wrap_execute_method(self) -> Callable:
        ...


    @abstractmethod
    def _wrap_context_callback(self) -> Callable:
        ...

        
class _PyMigrationActionCompletedHookWrapperBuilder(_PyHookWrapperBuilderBase):

    @property
    def _wrapper_method_name(self) -> str:
        return "Execute"

    @property
    def _wrapper_async(self) -> bool:
        return False

    def get_wrapper_base_type(self) -> type:
        return ISyncMigrationActionCompletedHook

    def _wrapper_context_type(self) -> type:
        return IMigrationActionResult

    def _wrap_execute_method(self) -> Callable:
        def _wrap_execute(w):
            return w._inner.execute
        
        return _wrap_execute
    
    def _wrap_context_callback(self) -> Callable:
        def _wrap_context(ctx):
            return PyMigrationActionResult(ctx)
        
        return _wrap_context

class PyMigrationActionCompletedHookBase():
    """Base class for migration action completed hooks."""

    _wrapper_builder = _PyMigrationActionCompletedHookWrapperBuilder

    def execute(self, ctx: PyMigrationActionResult) -> PyMigrationActionResult:
        """Executes a hook callback.
        
        Args:
            ctx: The input context from the migration engine or previous hook.
            
        Returns:
            The context, potentially modified to pass on to the next hook or migration engine, or None to continue passing the input context.
        """
        return ctx

class _PyContentBatchMigrationCompletedHookWrapperBuilder(_PyHookWrapperBuilderBase):

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

    def get_wrapper_base_type(self) -> type:
        return ISyncContentBatchMigrationCompletedHook[self.dotnet_content_type]

    def _wrapper_context_type(self) -> type:
        return IContentBatchMigrationResult[self.dotnet_content_type]

    def _wrap_execute_method(self) -> Callable:
        def _wrap_execute(w):
            return w._inner.execute
        
        return _wrap_execute
    
    def _wrap_context_callback(self) -> Callable:
        def _wrap_context(ctx):
            return PyContentBatchMigrationResult[self.python_content_type](ctx)
        
        return _wrap_context

class PyContentBatchMigrationCompletedHookBase(Generic[TContent]):
    """Generic base class for content batch completion hooks."""

    _wrapper_builder = _PyContentBatchMigrationCompletedHookWrapperBuilder

    def execute(self, ctx: PyContentBatchMigrationResult[TContent]) -> PyContentBatchMigrationResult[TContent]:
        """Executes a hook callback.
        
        Args:
            ctx: The input context from the migration engine or previous hook.
            
        Returns:
            The context, potentially modified to pass on to the next hook or migration engine, or None to continue passing the input context.
        """
        return ctx

class _PyInitializeMigrationHookWrapperBuilder(_PyHookWrapperBuilderBase):

    @property
    def _wrapper_method_name(self) -> str:
        return "Execute"

    @property
    def _wrapper_async(self) -> bool:
        return False

    def get_wrapper_base_type(self) -> type:
        return ISyncInitializeMigrationHook

    def _wrapper_context_type(self) -> type:
        return IInitializeMigrationHookResult

    def _wrap_execute_method(self) -> Callable:
        def _wrap_execute(w):
            return w._inner.execute
        
        return _wrap_execute
    
    def _wrap_context_callback(self) -> Callable:
        def _wrap_context(ctx):
            return PyInitializeMigrationHookResult(ctx)
        
        return _wrap_context

class PyInitializeMigrationHookBase:
    """Base class for initialize migration hooks."""

    _wrapper_builder = _PyInitializeMigrationHookWrapperBuilder

    def execute(self, ctx: PyInitializeMigrationHookResult) -> PyInitializeMigrationHookResult:
        """Executes a hook callback.
        
        Args:
            ctx: The input context from the migration engine or previous hook.
            
        Returns:
            The context, potentially modified to pass on to the next hook or migration engine, or None to continue passing the input context.
        """
        return ctx