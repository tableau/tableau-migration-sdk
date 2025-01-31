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

"""Wrapper for classes in Tableau.Migration.Engine.Endpoints.Search namespace."""

from tableau_migration import (
    cancellation_token
)
from tableau_migration.migration import (
    PyContentLocation,
    PyContentReference
)

from Tableau.Migration.Engine.Endpoints.Search import (
    IDestinationContentReferenceFinder,
    ISourceContentReferenceFinder,
    ManifestDestinationContentReferenceFinderFactory,
    ManifestSourceContentReferenceFinderFactory
)

from Tableau.Migration import (
    IContentReference,
    TaskExtensions
)

from System import Guid

from typing import (
    Generic,
    TypeVar,
    Type
)

from uuid import UUID

TContent = TypeVar("TContent")

class PyDestinationContentReferenceFinder(Generic[TContent]):
    """Interface for an object that can find destination content reference for given content information, applying mapping rules."""
    
    _dotnet_base = IDestinationContentReferenceFinder
    
    def __init__(self, destination_content_reference_finder: IDestinationContentReferenceFinder, t: Type[TContent]) -> None:
        """Creates a new PyDestinationContentReferenceFinder object.
        
        Args:
            destination_content_reference_finder: A IDestinationContentReferenceFinder object.
            t: The content type.
        
        Returns: None.
        """
        self._dotnet = destination_content_reference_finder
        self._content_type = t
        
    def find_by_source_location(self, source_location: PyContentLocation, cancel = None) -> PyContentReference:
        """Finds the destination content reference for the source content reference location.
        
        Args:
            source_location: The source content reference location.
            cancel: A cancellation token to obey.
        
        Returns: The found destination content reference, or None if no content reference was found.
        """
        if source_location is None:
            return None
        
        if cancel is None:
            cancel = cancellation_token

        result = TaskExtensions.AwaitResult[IContentReference](self._dotnet.FindBySourceLocationAsync(source_location._dotnet, cancel))
        return None if result is None else PyContentReference(result)
    
    def find_by_mapped_location(self, mapped_location: PyContentLocation, cancel = None) -> PyContentReference:
        """Finds the destination content reference for the mapped destination content reference location.
        
        Args:
            mapped_location: The destination mapped content reference location.
            cancel: A cancellation token to obey.
        
        Returns: The found destination content reference, or None if no content reference was found.
        """
        if mapped_location is None:
            return None
        
        if cancel is None:
            cancel = cancellation_token

        result = TaskExtensions.AwaitResult[IContentReference](self._dotnet.FindByMappedLocationAsync(mapped_location._dotnet, cancel))
        return None if result is None else PyContentReference(result)
    
    def find_by_source_id(self, source_id: UUID, cancel = None) -> PyContentReference:
        """Finds the destination content reference for the source content reference unique identifier.
        
        Args:
            source_id: The source content reference unique identifier.
            cancel: A cancellation token to obey.
        
        Returns: The found destination content reference, or None if no content reference was found.
        """
        if source_id is None:
            return None
        
        if cancel is None:
            cancel = cancellation_token

        result = TaskExtensions.AwaitResult[IContentReference](self._dotnet.FindBySourceIdAsync(Guid.Parse(str(source_id)), cancel))
        return None if result is None else PyContentReference(result)
    
    def find_by_id(self, id: UUID, cancel = None) -> PyContentReference:
        """Finds the content reference by its unique identifier.
        
        Args:
            id: The unique identifier.
            cancel: A cancellation token to obey.
        
        Returns: The found content reference, or None if no content reference was found.
        """
        if id is None:
            return None
        
        if cancel is None:
            cancel = cancellation_token

        result = TaskExtensions.AwaitResult[IContentReference](self._dotnet.FindByIdAsync(Guid.Parse(str(id)), cancel))
        return None if result is None else PyContentReference(result)
    
    def find_by_source_content_url(self, source_content_url: str, cancel = None) -> PyContentReference:
        """Finds the destination content reference for the source content reference URL.
        
        Args:
            source_content_url: The source content reference URL.
            cancel: A cancellation token to obey.
        
        Returns: The found destination content reference, or None if no content reference was found.
        """
        if source_content_url is None or not source_content_url.strip():
            return None

        if cancel is None:
            cancel = cancellation_token

        result = TaskExtensions.AwaitResult[IContentReference](self._dotnet.FindBySourceContentUrlAsync(source_content_url.strip(), cancel))
        return None if result is None else PyContentReference(result)
    
class PySourceContentReferenceFinder(Generic[TContent]):
    """Interface for an object that can find source content reference."""
    
    _dotnet_base = ISourceContentReferenceFinder
    
    def __init__(self, source_content_reference_finder: ISourceContentReferenceFinder, t: Type[TContent]) -> None:
        """Creates a new PySourceContentReferenceFinder object.
        
        Args:
            source_content_reference_finder: A ISourceContentReferenceFinder object.
            t: The content type.
        
        Returns: None.
        """
        self._dotnet = source_content_reference_finder
        self._content_type = t

    def find_by_source_location(self, source_location: PyContentLocation, cancel = None) -> PyContentReference:
        """Finds the source content reference for the source content reference location.
        
        Args:
            source_location: The source content reference location.
            cancel: A cancellation token to obey.
        
        Returns: The found source content reference, or None if no content reference was found.
        """
        if source_location is None:
            return None
        
        if cancel is None:
            cancel = cancellation_token

        result = TaskExtensions.AwaitResult[IContentReference](self._dotnet.FindBySourceLocationAsync(source_location._dotnet, cancel))
        return None if result is None else PyContentReference(result)
    
    def find_by_id(self, id: UUID, cancel = None) -> PyContentReference:
        """Finds the content reference by its unique identifier.
        
        Args:
            id: The unique identifier.
            cancel: A cancellation token to obey.
        
        Returns: The found content reference, or None if no content reference was found.
        """
        if id is None:
            return None
        
        if cancel is None:
            cancel = cancellation_token

        result = TaskExtensions.AwaitResult[IContentReference](self._dotnet.FindByIdAsync(Guid.Parse(str(id)), cancel))
        return None if result is None else PyContentReference(result)
        
class PyDestinationContentReferenceFinderFactory():
    """Interface for an object that can create destination content reference finders based on content type."""
    
    _dotnet_base = ManifestDestinationContentReferenceFinderFactory
    
    def __init__(self, destination_content_reference_finder_factory: ManifestDestinationContentReferenceFinderFactory) -> None:
        """Creates a new PyDestinationContentReferenceFinderFactory object.
        
        Args:
            destination_content_reference_finder_factory: A ManifestDestinationContentReferenceFinderFactory object.
        
        Returns: None.
        """
        self._dotnet = destination_content_reference_finder_factory
        
    def for_destination_content_type(self, t: Type[TContent]) -> PyDestinationContentReferenceFinder[TContent]:
        """Gets or creates a destination content reference finder for a given content type.
        
        Returns: The content reference finder.
        """
        try:
            if not hasattr(t, "_dotnet_base"):
                return None

            result = self._dotnet.ForDestinationContentType[t._dotnet_base]()
            return PyDestinationContentReferenceFinder[t](result, t)
        except Exception:
            return None

class PySourceContentReferenceFinderFactory():
    """Interface for an object that can create source content reference finders based on content type."""
    
    _dotnet_base = ManifestSourceContentReferenceFinderFactory
    
    def __init__(self, source_content_reference_finder_factory: ManifestSourceContentReferenceFinderFactory) -> None:
        """Creates a new PySourceContentReferenceFinderFactory object.
        
        Args:
            source_content_reference_finder_factory: A ManifestSourceContentReferenceFinderFactory object.
        
        Returns: None.
        """
        self._dotnet = source_content_reference_finder_factory
        
    def for_source_content_type(self, t: Type[TContent]):
        """Gets or creates a source content reference finder for a given content type.
        
        Returns: The content reference finder.
        """
        try:
            if not hasattr(t, "_dotnet_base"):
                return None

            result = self._dotnet.ForSourceContentType[t._dotnet_base]()
            return PySourceContentReferenceFinder[t](result, t)
        except Exception:
            return None


