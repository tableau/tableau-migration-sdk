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

"""Wrapper for classes in Tableau.Migration.Engine.Endpoints.Caching namespace."""

from abc import ABC, abstractmethod
from typing import Any, Generic, TypeVar

from tableau_migration.migration_interop import _unwrap
from tableau_migration.migration_engine_services import _PyServiceWrapperBuilderBase

from Tableau.Migration.Content.Search import ( # noqa: E402
    BulkContentReferenceCacheLoadStrategy,
    LazyContentReferenceCacheLoadStrategy
)

from Tableau.Migration.Engine.Endpoints.Caching import ( # noqa: E402
    IContentReferenceCacheLoadStrategyProvider
)

TContent = TypeVar("TContent")

class _ContentReferenceCacheLoadStrategyProviderWrapperBase(_PyServiceWrapperBuilderBase):
    
    @staticmethod
    def dotnet_service() -> type:
        return IContentReferenceCacheLoadStrategyProvider
    
    @property
    def python_content_type(self) -> type:
        return self.python_generic_types[0]

    @property
    def dotnet_content_type(self) -> type:
        return self.dotnet_generic_types[0]
    
    def get_wrapper_base_type(self) -> type:
        return IContentReferenceCacheLoadStrategyProvider[self.dotnet_content_type]

    def add_wrapper_members(self, members: dict[str, Any]) -> dict[str, Any]:
        members["GetSourceCacheLoadStrategy"] = lambda wrapper: _unwrap(wrapper._inner.get_source_cache_load_strategy())
        members["GetDestinationCacheLoadStrategy"] = lambda wrapper: _unwrap(wrapper._inner.get_destination_cache_load_strategy())
        return members

class ContentReferenceCacheLoadStrategyProviderBase(ABC, Generic[TContent]):
    """Generic base class for content reference cache load strategy providers."""

    _wrapper_builder = _ContentReferenceCacheLoadStrategyProviderWrapperBase

    @property
    def _dotnet_content_type(self) -> type:
        return self.dotnet_generic_types[0]

    @abstractmethod
    def get_source_cache_load_strategy(self):
        """Gets the cache load strategy for source endpoints.

        Returns: The cache load strategy.
        """
        ...

    @abstractmethod
    def get_destination_cache_load_strategy(self):
        """Gets the cache load strategy for destination endpoints.

        Returns: The cache load strategy.
        """
        ...

class BulkContentReferenceCacheLoadStrategyProvider(ContentReferenceCacheLoadStrategyProviderBase[TContent]):
    """Content reference cache load strategy provider that uses bulk load strategies."""

    def get_source_cache_load_strategy(self):
        """Gets the cache load strategy for source endpoints.

        Returns: The cache load strategy.
        """
        return BulkContentReferenceCacheLoadStrategy[self._dotnet_content_type]()

    def get_destination_cache_load_strategy(self):
        """Gets the cache load strategy for destination endpoints.

        Returns: The cache load strategy.
        """
        return BulkContentReferenceCacheLoadStrategy[self._dotnet_content_type]()

class LazyContentReferenceCacheLoadStrategyProvider(ContentReferenceCacheLoadStrategyProviderBase[TContent]):
    """Content reference cache load strategy provider that uses lazy load strategies."""

    def get_source_cache_load_strategy(self):
        """Gets the cache load strategy for source endpoints.

        Returns: The cache load strategy.
        """
        return LazyContentReferenceCacheLoadStrategy[self._dotnet_content_type]()

    def get_destination_cache_load_strategy(self):
        """Gets the cache load strategy for destination endpoints.

        Returns: The cache load strategy.
        """
        return LazyContentReferenceCacheLoadStrategy[self._dotnet_content_type]()