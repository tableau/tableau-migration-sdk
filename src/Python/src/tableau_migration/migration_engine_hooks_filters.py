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

"""Wrapper for classes in Tableau.Migration.Engine.Hooks.Filters namespace."""

from typing import  Callable, Union
from typing_extensions import Self

from System import IServiceProvider, Func
from Tableau.Migration.Engine.Hooks.Filters import IContentFilterBuilder
from tableau_migration.migration_engine_hooks import PyMigrationHookFactoryCollection

class PyContentFilterBuilder():
    """Default IContentFilterBuilder implementation."""

    _dotnet_base = IContentFilterBuilder

    def __init__(self, content_filters_builder: IContentFilterBuilder) -> None:
        """Default init.

        Args:
            content_filters_builder: Interface for a builder of filters that use a common content type
        
        Returns: None.
        """
        self._content_filters_builder = content_filters_builder


    def clear(self) -> Self:
        """Removes all currently registered filters.

        Returns:
            The same filter builder object for fluent API calls
        """
        self._content_filters_builder.Clear()
        return self


    def add(self, input_0: type, input_1: Union[Callable, None] = None) -> Self:
        """Adds an object or function to execute filters.

        Args:
            input_0: Either: 
                1) The filter type to execute, or
                2) The content type for a callback function
            input_1: Either:
                1) The callback function to execute, or
                2) None

        Returns:
            The same mapping builder object for fluent API calls.
        """
        from migration_engine_hooks_filters_interop import _PyFilterWrapper
        
        wrapper = _PyFilterWrapper(input_0, input_1)
        self._content_filters_builder.Add[wrapper.wrapper_type, wrapper.dotnet_content_type](Func[IServiceProvider, wrapper.wrapper_type](wrapper.factory))
    
        return self

    def by_content_type(self):
        """Gets the currently registered hook factories by their content types.

        Returns:
            The hook factories by their content types.
        """
        return self._content_filters_builder.ByContentType()


    def build(self) -> Self:
        """Builds an immutable collection from the currently added filters.

        Returns:
            The created collection.
        """
        return PyMigrationHookFactoryCollection(self._content_filters_builder.Build())
