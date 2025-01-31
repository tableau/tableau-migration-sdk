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

"""Wrapper for classes in Tableau.Migration.Engine.Hooks.Mappings namespace."""

from typing import  Callable, Union
from typing_extensions import Self

from System import IServiceProvider, Func
from Tableau.Migration.Engine.Hooks.Mappings import IContentMappingBuilder
from tableau_migration.migration_engine_hooks import PyMigrationHookFactoryCollection

class PyContentMappingBuilder():
    """Default IContentMappingBuilder implementation."""

    _dotnet_base = IContentMappingBuilder

    def __init__(self, content_mapping_builder: IContentMappingBuilder) -> None:
        """Default init.

        Args:
            content_mapping_builder: An object with methods to build IContentMapping"
        
        Returns: None.
        """
        self._content_mapping_builder = content_mapping_builder


    def clear(self) -> Self:
        """Removes all currently registered mappings.

        Returns:
            The same mapping builder object for fluent API calls
        """
        self._content_mapping_builder.Clear()
        return self


    def add(self, input_0: type, input_1: Union[Callable, None] = None) -> Self:
        """Adds an object or function to execute mappings.

        Args:
            input_0: Either: 
                1) The mapping type to execute, or
                2) The content type for a callback function
            input_1: Either:
                1) The callback function to execute, or
                2) None

        Returns:
            The same mapping builder object for fluent API calls.
        """
        from migration_engine_hooks_mappings_interop import _PyMappingWrapper
        
        wrapper = _PyMappingWrapper(input_0, input_1)
        self._content_mapping_builder.Add[wrapper.wrapper_type, wrapper.dotnet_content_type](Func[IServiceProvider, wrapper.wrapper_type](wrapper.factory))

        return self


    def by_content_type(self):
        """Gets the currently registered hook factories by their content types.

        Returns:
            The hook factories by their content types.
        """
        return self._content_mapping_builder.ByContentType()


    def build(self) -> PyMigrationHookFactoryCollection:
        """Builds an immutable collection from the currently added mappings.

        Returns:
            The created collection.
        """
        return PyMigrationHookFactoryCollection(self._content_mapping_builder.Build())

# region _generated

from tableau_migration.migration import (  # noqa: E402, F401
    _generic_wrapper,
    PyContentLocation
)
from typing import (  # noqa: E402, F401
    Generic,
    TypeVar
)
from typing_extensions import Self # noqa: E402, F401

from Tableau.Migration.Engine.Hooks.Mappings import ContentMappingContext # noqa: E402, F401

TContent = TypeVar("TContent")

class PyContentMappingContext(Generic[TContent]):
    """Context for IContentMapping operations mapping a content item to an intended destination location for publishing and content references."""
    
    _dotnet_base = ContentMappingContext
    
    def __init__(self, content_mapping_context: ContentMappingContext) -> None:
        """Creates a new PyContentMappingContext object.
        
        Args:
            content_mapping_context: A ContentMappingContext object.
        
        Returns: None.
        """
        self._dotnet = content_mapping_context
        
    @property
    def content_item(self) -> TContent:
        """Gets the content item being mapped."""
        return None if self._dotnet.ContentItem is None else _generic_wrapper(self._dotnet.ContentItem)
    
    @property
    def mapped_location(self) -> PyContentLocation:
        """Gets the destination location the content item will be mapped and/or published to."""
        return None if self._dotnet.MappedLocation is None else PyContentLocation(self._dotnet.MappedLocation)
    
    def map_to(self, mapped_location: PyContentLocation) -> Self:
        """Maps the content item to a new destination location.
        
        Args:
            mapped_location: The destination location to map to.
        
        Returns: A new context for the content item with the mapped location.
        """
        result = self._dotnet.MapTo(None if mapped_location is None else mapped_location._dotnet)
        return None if result is None else PyContentMappingContext[TContent](result)
    

# endregion

