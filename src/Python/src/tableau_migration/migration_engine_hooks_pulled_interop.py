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

"""Interoperability utility for item pulled hooks."""

from typing import Callable, Generic, Optional, TypeVar

from migration_engine_hooks_interop import _PyHookWrapperBuilderBase
from migration_engine_hooks_pulled import PyContentItemPulledContext

from Tableau.Migration.Engine.Hooks.Pulled import (
    ContentItemPulledContext,
    ContentItemPulledHookBase
)

TPrepare = TypeVar("TPrepare")

class _PyContentItemPulledHookWrapperBuilder(_PyHookWrapperBuilderBase):
    
    @property
    def python_prepare_type(self) -> type:
        return self.python_generic_types[0]

    @property
    def dotnet_prepare_type(self) -> type:
        return self.dotnet_generic_types[0]

    def get_wrapper_base_type(self) -> type:
        return ContentItemPulledHookBase[self.dotnet_prepare_type]

    def _wrapper_context_type(self) -> type:
        return ContentItemPulledContext[self.dotnet_prepare_type]

    def _wrap_execute_method(self) -> Callable:
        def _wrap_execute(w):
            return w._inner.execute
        
        return _wrap_execute
    
    def _wrap_context_callback(self) -> Callable:
        def _wrap_context(ctx):
            return PyContentItemPulledContext[self.python_prepare_type](ctx)
        
        return _wrap_context
    
class PyContentItemPulledHookBase(Generic[TPrepare]):
    """Generic base class for item pulled hooks."""

    _wrapper_builder = _PyContentItemPulledHookWrapperBuilder

    def execute(self, ctx: PyContentItemPulledContext[TPrepare]) -> Optional[PyContentItemPulledContext[TPrepare]]:
        """Executes a hook callback.
        
        Args:
            ctx: The input context from the migration engine or previous hook.

        Returns:
            The context, potentially modified to pass on to the next hook or migration engine, or None to continue passing the input context.
        """
        return ctx
