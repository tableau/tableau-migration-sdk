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

"""Wrapper for classes in Tableau.Migration.Engine.Hooks.Pulled namespace."""

# region _generated

from tableau_migration.migration import _generic_wrapper # noqa: E402, F401
from tableau_migration.migration_engine_hooks_filters import PyFilterStatus # noqa: E402, F401
from tableau_migration.migration_engine_manifest import PyMigrationManifestEntryEditor # noqa: E402, F401
from typing import (  # noqa: E402, F401
    Generic,
    TypeVar
)

from Tableau.Migration.Engine.Hooks.Filters import FilterStatus # noqa: E402, F401
from Tableau.Migration.Engine.Hooks.Pulled import ContentItemPulledContext # noqa: E402, F401

TPrepare = TypeVar("TPrepare")

class PyContentItemPulledContext(Generic[TPrepare]):
    """Context for IContentItemPulledHook operations for pulled content items."""
    
    _dotnet_base = ContentItemPulledContext
    
    def __init__(self, content_item_pulled_context: ContentItemPulledContext) -> None:
        """Creates a new PyContentItemPulledContext object.
        
        Args:
            content_item_pulled_context: A ContentItemPulledContext object.
        
        Returns: None.
        """
        self._dotnet = content_item_pulled_context
        
    @property
    def manifest_entry(self) -> PyMigrationManifestEntryEditor:
        """Gets the manifest entry for the content item."""
        return None if self._dotnet.ManifestEntry is None else PyMigrationManifestEntryEditor(self._dotnet.ManifestEntry)
    
    @property
    def pulled_item(self) -> TPrepare:
        """Gets the content item that was pulled."""
        return None if self._dotnet.PulledItem is None else _generic_wrapper(self._dotnet.PulledItem)
    
    @property
    def status(self) -> PyFilterStatus:
        """Gets or sets the current filtering status for the pulled content item."""
        return None if self._dotnet.Status is None else PyFilterStatus(self._dotnet.Status.value__)
    
    @status.setter
    def status(self, value: PyFilterStatus) -> None:
        """Gets or sets the current filtering status for the pulled content item."""
        self._dotnet.Status = FilterStatus(value)
    

# endregion

