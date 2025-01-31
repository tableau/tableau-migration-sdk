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

"""Wrapper for classes in Tableau.Migration.Engine.Hooks namespace."""

from inspect import isclass
from typing import Callable, get_args, get_origin, Type, TypeVar, Union
from typing_extensions import Self

from System import Func, IServiceProvider
from Tableau.Migration.Engine.Hooks import IMigrationHookBuilder, IMigrationHookFactoryCollection

T = TypeVar("T")

class PyMigrationHookFactoryCollection():
    """Interface for an object that contains MigrationHookFactorys registered for each hook type."""

    _dotnet_base = IMigrationHookFactoryCollection

    def __init__(self, migration_hook_factory_collection: IMigrationHookFactoryCollection) -> None:
        """Default init.

        Args:
            migration_hook_factory_collection: A collection that contains MigrationHookFactory registered for each hook type.
        
        Returns: None.
        """
        self._migration_hook_factory_collection = migration_hook_factory_collection

    def get_hooks(self, type_to_get: Type[T]):
        """Gets the MigrationHookFactorys for the given hook type.
        
        This type has to be an interface that inherits from IMigrationHook.

        Args:
            type_to_get: the hook type
        """
        return self._migration_hook_factory_collection.GetHooks[type_to_get]()

def _get_wrapper_from_callback_context(t: type) -> type:
    from migration_engine_actions import PyMigrationActionResult
    from migration_engine_hooks_interop import (
        _PyContentBatchMigrationCompletedHookWrapper, 
        _PyInitializeMigrationHookWrapper,
        _PyMigrationActionCompletedHookWrapper
    )
    from migration_engine_hooks_postpublish import PyBulkPostPublishContext, PyContentItemPostPublishContext
    from migration_engine_hooks_postpublish_interop import _PyBulkPostPublishHookWrapper, _PyContentItemPostPublishHookWrapper
    from migration_engine_hooks_results import PyInitializeMigrationHookResult
    from migration_engine_migrators_batch import PyContentBatchMigrationResult

    types = {
        PyBulkPostPublishContext.__name__: _PyBulkPostPublishHookWrapper,
        PyContentBatchMigrationResult.__name__: _PyContentBatchMigrationCompletedHookWrapper,
        PyContentItemPostPublishContext.__name__: _PyContentItemPostPublishHookWrapper,
        PyInitializeMigrationHookResult.__name__: _PyInitializeMigrationHookWrapper,
        PyMigrationActionResult.__name__: _PyMigrationActionCompletedHookWrapper
    }

    if t.__name__ not in types:
        return None

    return types[t.__name__]

class PyMigrationHookBuilder():
    """Default IMigrationHookBuilder implementation."""

    _dotnet_base = IMigrationHookBuilder

    def __init__(self, migration_hook_builder: IMigrationHookBuilder) -> None:
        """Default init.

        Args:
            migration_hook_builder: An object that contains the hooks to execute at various points during the migration, determined by hook type.
        
        Returns: None.
        """
        self._migration_hook_builder = migration_hook_builder


    def clear(self) -> Self:
        """Removes all currently registered hooks.

        Returns:
            The same hook builder object for fluent API calls.
        """
        self._migration_hook_builder.Clear()
        return self

    
    def add(self, input_0: type, input_1: Union[Callable, None] = None) -> Self:
        """Adds an object or function to execute hooks.

        Args:
            input_0: Either: 
                1) The hook type to execute, or
                2) The hook context type for a callback function
            input_1: Either:
                1) The callback function to execute, or
                2) None

        Returns:
            The same mapping builder object for fluent API calls.
        """
        if input_1 is None:
            wrapper = input_0._wrapper(input_0)
        else:
            t = input_0 if isclass(input_0) else get_origin(input_0)
            wrap_type = _get_wrapper_from_callback_context(t)
            wrapper = wrap_type(list(get_args(input_0)), input_1)
        
        self._migration_hook_builder.Add[wrapper.wrapper_type](Func[IServiceProvider, wrapper.wrapper_type](wrapper.factory))

        return self


    def build(self) -> PyMigrationHookFactoryCollection:
        """Builds an immutable collection from the currently added hooks.

        Returns:
            The created collection.
        """
        return PyMigrationHookFactoryCollection(self._migration_hook_builder.Build())
