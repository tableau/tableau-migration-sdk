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

"""Interoperability utility for filters."""

from typing import Callable, Generic, TypeVar

from migration_engine import PyContentMigrationItem
from migration_engine_hooks_interop import _PyHookWrapperBase

from Tableau.Migration.Engine import ContentMigrationItem
from Tableau.Migration.Engine.Hooks.Filters import ContentFilterBase

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

class _PyFilterWrapper(_PyHookWrapperBase):
    
    @property
    def python_content_type(self) -> type:
        return self.python_generic_types[0]

    @property
    def dotnet_content_type(self) -> type:
        return self.dotnet_generic_types[0]

    def _wrapper_base_type(self) -> type:
        return ContentFilterBase[self.dotnet_content_type]

    @property
    def _wrapper_method_name(self) -> str:
        return "ShouldMigrate"

    @property
    def _wrapper_async(self) -> bool:
        return False

    def _wrapper_context_type(self) -> type:
        return ContentMigrationItem[self.dotnet_content_type]

    def _wrap_execute_method(self) -> Callable:
        return lambda w : w._hook.should_migrate
    
    def _wrap_context_callback(self) -> Callable:
        return lambda ctx : PyContentMigrationItem[self.python_content_type](ctx)
    