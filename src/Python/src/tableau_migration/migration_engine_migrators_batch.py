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

"""Wrapper for classes in Tableau.Migration.Engine.Migrators.Batch namespace."""

# region _generated

from tableau_migration.migration import (  # noqa: E402, F401
    _generic_wrapper,
    PyResult
)
from tableau_migration.migration_engine_migrators import PyContentItemMigrationResult # noqa: E402, F401
from typing import (  # noqa: E402, F401
    Generic,
    TypeVar,
    Sequence
)
from typing_extensions import Self # noqa: E402, F401

from Tableau.Migration.Engine.Migrators.Batch import IContentBatchMigrationResult # noqa: E402, F401

TContent = TypeVar("TContent")

class PyContentBatchMigrationResult(Generic[TContent], PyResult):
    """IResult object for a migration action."""
    
    _dotnet_base = IContentBatchMigrationResult
    
    def __init__(self, content_batch_migration_result: IContentBatchMigrationResult) -> None:
        """Creates a new PyContentBatchMigrationResult object.
        
        Args:
            content_batch_migration_result: A IContentBatchMigrationResult object.
        
        Returns: None.
        """
        self._dotnet = content_batch_migration_result
        
    @property
    def perform_next_batch(self) -> bool:
        """Gets whether or not to migrate the next batch, if any."""
        return self._dotnet.PerformNextBatch
    
    @property
    def item_results(self) -> Sequence[PyContentItemMigrationResult[TContent]]:
        """Gets the migration result of each item in the batch, in the order they finished."""
        return None if self._dotnet.ItemResults is None else list((None if x is None else PyContentItemMigrationResult[TContent](x)) for x in self._dotnet.ItemResults)
    
    def for_next_batch(self, perform_next_batch: bool) -> Self:
        """Creates a new PerformNextBatch value.
        
        Args:
            perform_next_batch: Whether or not to migrate the next batch.
        
        Returns: The new IContentBatchMigrationResult object.
        """
        result = self._dotnet.ForNextBatch(perform_next_batch)
        return None if result is None else PyContentBatchMigrationResult[TContent](result)
    

# endregion

