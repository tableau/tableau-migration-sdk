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

"""Interoperability utility for post publish hooks."""

from typing import Callable, Generic, TypeVar

from migration_engine_hooks_interop import _PyHookWrapperBase
from migration_engine_hooks_postpublish import PyBulkPostPublishContext, PyContentItemPostPublishContext

from Tableau.Migration.Engine.Hooks.PostPublish import (
    BulkPostPublishContext, BulkPostPublishHookBase, 
    ContentItemPostPublishContext, ContentItemPostPublishHookBase
)

TSource = TypeVar("TSource")
TPublish = TypeVar("TPublish")
TResult = TypeVar("TResult")

class _PyContentItemPostPublishHookWrapper(_PyHookWrapperBase):
    
    @property
    def python_publish_type(self) -> type:
        return self.python_generic_types[0]
    
    @property
    def python_result_type(self) -> type:
        return self.python_generic_types[1]

    @property
    def dotnet_publish_type(self) -> type:
        return self.dotnet_generic_types[0]
    
    @property
    def dotnet_result_type(self) -> type:
        return self.dotnet_generic_types[1]

    def _wrapper_base_type(self) -> type:
        return ContentItemPostPublishHookBase[self.dotnet_publish_type, self.dotnet_result_type]

    def _wrapper_context_type(self) -> type:
        return ContentItemPostPublishContext[self.dotnet_publish_type, self.dotnet_result_type]

    def _wrap_execute_method(self) -> Callable:
        def _wrap_execute(w):
            return w._hook.execute
        
        return _wrap_execute
    
    def _wrap_context_callback(self) -> Callable:
        def _wrap_context(ctx):
            return PyContentItemPostPublishContext[self.python_publish_type, self.python_result_type](ctx)
        
        return _wrap_context
    
class PyContentItemPostPublishHookBase(Generic[TPublish, TResult]):
    """Generic base class for single item post publish hooks."""

    _wrapper = _PyContentItemPostPublishHookWrapper

    def execute(self, ctx: PyContentItemPostPublishContext[TPublish, TResult]) -> PyContentItemPostPublishContext[TPublish, TResult]:
        """Executes a hook callback.
        
        Args:
            ctx: The input context from the migration engine or previous hook.

        Returns:
            The context, potentially modified to pass on to the next hook or migration engine, or None to continue passing the input context.
        """
        return ctx

class _PyBulkPostPublishHookWrapper(_PyHookWrapperBase):
    
    @property
    def python_source_type(self) -> type:
        return self.python_generic_types[0]

    @property
    def dotnet_source_type(self) -> type:
        return self.dotnet_generic_types[0]

    def _wrapper_base_type(self) -> type:
        return BulkPostPublishHookBase[self.dotnet_source_type]

    def _wrapper_context_type(self) -> type:
        return BulkPostPublishContext[self.dotnet_source_type]

    def _wrap_execute_method(self) -> Callable:
        def _wrap_execute(w):
            return w._hook.execute
        
        return _wrap_execute
    
    def _wrap_context_callback(self) -> Callable:
        def _wrap_context(ctx):
            return PyBulkPostPublishContext[self.python_source_type](ctx)
        
        return _wrap_context

class PyBulkPostPublishHookBase(Generic[TSource]):
    """Generic base class for bulk post publish hooks."""

    _wrapper = _PyBulkPostPublishHookWrapper

    def execute(self, ctx: PyBulkPostPublishContext[TSource]) -> PyBulkPostPublishContext[TSource]:
        """Executes a hook callback.
        
        Args:
            ctx: The input context from the migration engine or previous hook.
            
        Returns:
            The context, potentially modified to pass on to the next hook or migration engine, or None to continue passing the input context.
        """
        return ctx