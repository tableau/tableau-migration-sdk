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

"""Interoperability utility for filters."""

from inspect import signature
from typing import Callable, Generic, Optional, TypeVar, Union

from migration_engine import PyContentMigrationItem
from migration_engine_hooks_filters import PyContentFilterContextItem, PyFilterStatus
from migration_engine_hooks_interop import _PyHookWrapperBuilderBase

from Tableau.Migration.Engine.Hooks.Filters import ContentFilterBase, ContentFilterContextItem

TContent = TypeVar("TContent")

class PyContentFilterBase(Generic[TContent]):
    """Generic base class for filters."""

    def should_migrate(self, item: PyContentMigrationItem[TContent]) -> bool:
        """Checks if the item should be migrated.
        
        Args:
            item: The item to evaluate.
            
        Returns:
            True if the item should be migrated.
        """
        return True

    def filter(self, item: PyContentFilterContextItem[TContent]) -> None:
        """Considers the content item for filtering.
        
        Args:
            item: The item to potentially filter.
        """
        if item.status != PyFilterStatus.MIGRATE:
            return

        if not self.should_migrate(item):
            item.status = PyFilterStatus.SKIP

def _upgrade_filter_result(ctx, result):
    if(isinstance(result, bool) and not result):
        ctx.status = PyFilterStatus.SKIP
    return ctx

def _upgrade_callback_result(callback: Callable) -> Callable:
    
    def _upgrade_result(ctx):
        return _upgrade_filter_result(ctx, callback(ctx))

    def _upgrade_result_services(ctx, s):
        return _upgrade_filter_result(ctx, callback(ctx, s))
    
    if len(signature(callback).parameters) == 1:
        return _upgrade_result
    else:
        return _upgrade_result_services

class _PyFilterWrapperBuilder(_PyHookWrapperBuilderBase):
    
    def __init__(self, t: Union[type, list], callback: Optional[Callable] = None) -> None:
        if callback is None:
            super().__init__(t)
        else:
            super().__init__(t, _upgrade_callback_result(callback))

    @property
    def python_content_type(self) -> type:
        return self.python_generic_types[0]

    @property
    def dotnet_content_type(self) -> type:
        return self.dotnet_generic_types[0]

    def get_wrapper_base_type(self) -> type:
        return ContentFilterBase[self.dotnet_content_type]

    @property
    def _wrapper_method_name(self) -> str:
        return "Filter"

    @property
    def _wrapper_async(self) -> bool:
        return False

    def _wrapper_context_type(self) -> type:
        return ContentFilterContextItem[self.dotnet_content_type]

    def _wrap_execute_method(self) -> Callable:
        return lambda w : w._inner.filter
    
    def _wrap_context_callback(self) -> Callable:
        return lambda ctx : PyContentFilterContextItem[self.python_content_type](ctx)
    