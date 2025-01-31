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

"""Interoperability utility for mappings."""

from typing import Callable, Generic, TypeVar

from migration_content import PyUser
from migration_engine_hooks_mappings import PyContentMappingContext
from migration_engine_hooks_interop import _PyHookWrapperBase

from Tableau.Migration.Engine.Hooks.Mappings import ContentMappingBase, ContentMappingContext
from Tableau.Migration.Engine.Hooks.Mappings.Default import ITableauCloudUsernameMapping

TContent = TypeVar("TContent")

class PyContentMappingBase(Generic[TContent]):
    """Generic base class for mappings."""

    def map(self, ctx: PyContentMappingContext[TContent]) -> PyContentMappingContext[TContent]:
        """Executes the mapping.
        
        Args:
            ctx: The input context from the migration engine or previous hook.
            
        Returns:
            The context, potentially modified to pass on to the next hook or migration engine, or None to continue passing the input context.
        """
        return ctx

class _PyMappingWrapper(_PyHookWrapperBase):
    
    @property
    def python_content_type(self) -> type:
        return self.python_generic_types[0]

    @property
    def dotnet_content_type(self) -> type:
        return self.dotnet_generic_types[0]

    def _wrapper_base_type(self) -> type:
        return ContentMappingBase[self.dotnet_content_type]

    @property
    def _wrapper_method_name(self) -> str:
        return "MapAsync"

    def _wrapper_context_type(self) -> type:
        return ContentMappingContext[self.dotnet_content_type]

    def _wrap_execute_method(self) -> Callable:
        def _wrap_execute(w):
            return w._hook.map
        
        return _wrap_execute
    
    def _wrap_context_callback(self) -> Callable:
        def _wrap_context(ctx):
            return PyContentMappingContext[self.python_content_type](ctx)
        
        return _wrap_context

class PyTableauCloudUsernameMappingBase(PyContentMappingBase[PyUser]):
    """Base class for mapping users to supply a Tableau Cloud compatible usernames."""
    pass

class _PyTableauCloudUsernameMappingWrapper(_PyMappingWrapper):
    def set_extra_base_types(self, types: tuple) -> tuple:
        return types + (ITableauCloudUsernameMapping,)