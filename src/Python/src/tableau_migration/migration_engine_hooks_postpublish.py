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

"""Wrapper for classes in Tableau.Migration.Engine.Hooks.PostPublish namespace."""

# region _generated

from tableau_migration.migration import _generic_wrapper # noqa: E402, F401
from tableau_migration.migration_engine_manifest import PyMigrationManifestEntryEditor # noqa: E402, F401
from typing import (  # noqa: E402, F401
    Generic,
    TypeVar,
    Sequence
)

from Tableau.Migration.Engine.Hooks.PostPublish import (  # noqa: E402, F401
    BulkPostPublishContext,
    ContentItemPostPublishContext
)

TPublish = TypeVar("TPublish")
TResult = TypeVar("TResult")

class PyBulkPostPublishContext(Generic[TPublish]):
    """Context for BulkPostPublishContext operations for published content items."""
    
    _dotnet_base = BulkPostPublishContext
    
    def __init__(self, bulk_post_publish_context: BulkPostPublishContext) -> None:
        """Creates a new PyBulkPostPublishContext object.
        
        Args:
            bulk_post_publish_context: A BulkPostPublishContext object.
        
        Returns: None.
        """
        self._dotnet = bulk_post_publish_context
        
    @property
    def published_items(self) -> Sequence[TPublish]:
        """Gets the content item being published."""
        return None if self._dotnet.PublishedItems is None else list((None if x is None else _generic_wrapper(x)) for x in self._dotnet.PublishedItems)
    
class PyContentItemPostPublishContext(Generic[TPublish, TResult]):
    """Context for ContentItemPostPublishHookBase operations for published content items."""
    
    _dotnet_base = ContentItemPostPublishContext
    
    def __init__(self, content_item_post_publish_context: ContentItemPostPublishContext) -> None:
        """Creates a new PyContentItemPostPublishContext object.
        
        Args:
            content_item_post_publish_context: A ContentItemPostPublishContext object.
        
        Returns: None.
        """
        self._dotnet = content_item_post_publish_context
        
    @property
    def manifest_entry(self) -> PyMigrationManifestEntryEditor:
        """Gets the manifest entry for the content item."""
        return None if self._dotnet.ManifestEntry is None else PyMigrationManifestEntryEditor(self._dotnet.ManifestEntry)
    
    @property
    def published_item(self) -> TPublish:
        """Gets the content item being published."""
        return None if self._dotnet.PublishedItem is None else _generic_wrapper(self._dotnet.PublishedItem)
    
    @property
    def destination_item(self) -> TResult:
        """Gets the returned content item after publishing."""
        return None if self._dotnet.DestinationItem is None else _generic_wrapper(self._dotnet.DestinationItem)
    

# endregion

