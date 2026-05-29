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

"""Wrapper for classes in Tableau.Migration.Engine.Hooks.Filters namespace."""

# region _generated

from enum import IntEnum # noqa: E402, F401
from tableau_migration.migration import _generic_wrapper # noqa: E402, F401
from tableau_migration.migration_engine import PyContentMigrationItem # noqa: E402, F401
from typing import (  # noqa: E402, F401
    Generic,
    TypeVar,
    Sequence
)

from Tableau.Migration.Engine.Hooks.Filters import (  # noqa: E402, F401
    ContentFilterContext,
    ContentFilterContextItem,
    FilterStatus
)

TContent = TypeVar("TContent")

class PyFilterStatus(IntEnum):
    """Enumeration of the various filter states for a content item."""
    
    #: The content item will attempt to migrate.
    MIGRATE = 0
    
    #: The content item will not migrate, but items that reference this one will still migrate.
    SKIP = 1
    
    #: The content item will not migrate, and items that reference this one will also not migrate.
    CASCADE_SKIP = 2
    
class PyContentFilterContextItem(Generic[TContent], PyContentMigrationItem[TContent]):
    """Context for IContentFilter operations, determining whether a content item should be migrated and whether to cascade filtering to dependent content types."""
    
    _dotnet_base = ContentFilterContextItem
    
    def __init__(self, content_filter_context_item: ContentFilterContextItem) -> None:
        """Creates a new PyContentFilterContextItem object.
        
        Args:
            content_filter_context_item: A ContentFilterContextItem object.
        
        Returns: None.
        """
        self._dotnet = content_filter_context_item
        
    @property
    def status(self) -> PyFilterStatus:
        """Gets or sets the current filtering status for the content item."""
        return None if self._dotnet.Status is None else PyFilterStatus(self._dotnet.Status.value__)
    
    @status.setter
    def status(self, value: PyFilterStatus) -> None:
        """Gets or sets the current filtering status for the content item."""
        self._dotnet.Status = FilterStatus(value)
    
class PyContentFilterContext(Generic[TContent]):
    """Context for IContentFilter operations, determining which items should be migrated and whether to cascade filtering to dependent content types."""
    
    _dotnet_base = ContentFilterContext
    
    def __init__(self, content_filter_context: ContentFilterContext) -> None:
        """Creates a new PyContentFilterContext object.
        
        Args:
            content_filter_context: A ContentFilterContext object.
        
        Returns: None.
        """
        self._dotnet = content_filter_context
        
    @property
    def items(self) -> Sequence[PyContentFilterContextItem[TContent]]:
        """Gets the items to potentially filter."""
        return None if self._dotnet.Items is None else list((None if x is None else PyContentFilterContextItem[TContent](x)) for x in self._dotnet.Items)
    

# endregion

