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

"""Wrapper for classes in Tableau.Migration.Content.Search namespace."""

# region _generated

from tableau_migration import cancellation_token # noqa: E402, F401
from tableau_migration.migration import PyContentReference # noqa: E402, F401
from typing import Sequence # noqa: E402, F401
from uuid import UUID # noqa: E402, F401

from System import Guid # noqa: E402, F401
from System.Collections.Immutable import IImmutableList # noqa: E402, F401
from Tableau.Migration import (  # noqa: E402, F401
    TaskExtensions,
    IContentReference
)
from Tableau.Migration.Content.Search import IContentReferenceFinder # noqa: E402, F401

class PyContentReferenceFinder():
    """Interface for an object that can find IContentReferences for given search criteria."""
    
    _dotnet_base = IContentReferenceFinder
    
    def __init__(self, content_reference_finder: IContentReferenceFinder) -> None:
        """Creates a new PyContentReferenceFinder object.
        
        Args:
            content_reference_finder: A IContentReferenceFinder object.
        
        Returns: None.
        """
        self._dotnet = content_reference_finder
        
    def find_all(self) -> Sequence[PyContentReference]:
        """Finds all available content references.
        
        Returns: The found content references.
        """
        result = TaskExtensions.AwaitResult[IImmutableList[IContentReference]](self._dotnet.FindAllAsync(cancellation_token))
        return None if result is None else list((None if x is None else PyContentReference(x)) for x in result)
    
    def find_by_id(self, id: UUID) -> PyContentReference:
        """Finds the content reference by its unique identifier.
        
        Args:
            id: The unique identifier.
        
        Returns: The found content reference, or null if no content reference was found.
        """
        result = TaskExtensions.AwaitResult[IContentReference](self._dotnet.FindByIdAsync(None if id is None else Guid.Parse(str(id)), cancellation_token))
        return None if result is None else PyContentReference(result)
    

# endregion

