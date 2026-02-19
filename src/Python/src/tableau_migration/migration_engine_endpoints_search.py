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

"""Wrapper for classes in Tableau.Migration.Engine.Endpoints.Search namespace."""

from typing import (
    Generic,
    TypeVar,
    Type,
    Union,
    Sequence,
    Any
)

from uuid import UUID

from tableau_migration import ( # noqa: E402, F401
    cancellation_token
)

from tableau_migration.migration import ( # noqa: E402, F401
    PyContentLocation,
    PyContentReference
)

from tableau_migration.migration_content_search import PyContentReferenceFinder # noqa: E402, F401
from tableau_migration.migration_interop import (
    _unwrap_async, 
    _wrap_content_location, 
    _wrap_guid
)
from tableau_migration.migration_engine_services import _PyServiceWrapperBuilderBase

from System import Guid # noqa: E402, F401
from System.Threading.Tasks import Task # noqa: E402, F401
from System.Collections.Immutable import ImmutableList # noqa: E402, F401

from Tableau.Migration import ( # noqa: E402, F401
    IContentReference,
    TaskExtensions
)

from Tableau.Migration.Engine.Endpoints.Search import ( # noqa: E402, F401
    IDestinationContentReferenceFinder,
    ISourceContentReferenceFinder,
    ManifestDestinationContentReferenceFinder,
    DestinationContentReferenceFinderFactory,
    SourceContentReferenceFinderFactory
)

TContent = TypeVar("TContent")

class PyDestinationContentReferenceFinder(Generic[TContent], PyContentReferenceFinder):
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
        
    def find_by_source_location(self, source_location: PyContentLocation, cancel = None) -> Union[PyContentReference, None]:
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
    
    def find_by_mapped_location(self, mapped_location: PyContentLocation, cancel = None) -> Union[PyContentReference, None]:
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
    
    def find_by_source_id(self, source_id: UUID, cancel = None) -> Union[PyContentReference, None]:
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
    
    def find_by_id(self, id: UUID, cancel = None) -> Union[PyContentReference, None]:
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
    
    def find_by_source_content_url(self, source_content_url: str, cancel = None) -> Union[PyContentReference, None]:
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
    
class PySourceContentReferenceFinder(Generic[TContent], PyContentReferenceFinder):
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

    def find_by_source_location(self, source_location: PyContentLocation, cancel = None) -> Union[PyContentReference, None]:
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
    
    def find_by_id(self, id: UUID, cancel = None) -> Union[PyContentReference, None]:
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
    
    _dotnet_base = DestinationContentReferenceFinderFactory
    
    def __init__(self, destination_content_reference_finder_factory: DestinationContentReferenceFinderFactory) -> None:
        """Creates a new PyDestinationContentReferenceFinderFactory object.
        
        Args:
            destination_content_reference_finder_factory: A DestinationContentReferenceFinderFactory object.
        
        Returns: None.
        """
        self._dotnet = destination_content_reference_finder_factory
        
    def for_destination_content_type(self, t: Type[TContent]) -> Union[PyDestinationContentReferenceFinder[TContent], None]:
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
    
    _dotnet_base = SourceContentReferenceFinderFactory
    
    def __init__(self, source_content_reference_finder_factory: SourceContentReferenceFinderFactory) -> None:
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


class _PyContentReferenceFinderWrapperBuilderBase(_PyServiceWrapperBuilderBase):
    """Base class for content reference finder wrappers."""
    
    def _build_async_method(self, method_name: str, param_wrapper: callable = None):
        """Build an async method that calls a Python method and wraps the result.
        
        Args:
            method_name: Name of the Python method to call
            param_wrapper: Optional function to wrap the parameter before passing to Python method
        """
        def async_method(self_wrapper, param, cancel):
            python_instance = self_wrapper._inner
            # Wrap parameter if wrapper function provided
            wrapped_param = param_wrapper(param) if param_wrapper else param
            # Call the Python method
            result = getattr(python_instance, method_name)(wrapped_param)
            # Wrap the result for C# interop
            return _unwrap_async(IContentReference, result)
        return async_method
    
    def _build_find_all_async(self):
        """Build the FindAllAsync method which returns a list of content references."""
        def async_method(self_wrapper, cancel):
            python_instance = self_wrapper._inner
            result = python_instance.find_all()
            
            if result is None:
                return Task.FromResult[ImmutableList[IContentReference]](ImmutableList[IContentReference].Empty)
            
            dotnet_list = ImmutableList[IContentReference].Empty
            for item in result:
                if hasattr(item, "_dotnet"):
                    dotnet_list = dotnet_list.Add(item._dotnet)
            
            return Task.FromResult[ImmutableList[IContentReference]](dotnet_list)
        return async_method

class _PyDestinationContentReferenceFinderWrapperBuilder(_PyContentReferenceFinderWrapperBuilderBase):
    """Wrapper that makes PyDestinationContentReferenceFinderBase callable from C#."""
    
    @staticmethod
    def dotnet_service() -> type:
        return IDestinationContentReferenceFinder
    
    @property
    def python_content_type(self) -> type:
        return self.python_generic_types[0]

    @property
    def dotnet_content_type(self) -> type:
        return self.dotnet_generic_types[0]
    
    def get_wrapper_base_type(self) -> type:
        return IDestinationContentReferenceFinder[self.dotnet_content_type]

    def add_wrapper_members(self, members: dict[str, Any]) -> dict[str, Any]:
        members["FindBySourceLocationAsync"] = self._build_async_method("find_by_source_location", _wrap_content_location)
        members["FindByMappedLocationAsync"] = self._build_async_method("find_by_mapped_location", _wrap_content_location)
        members["FindBySourceIdAsync"] = self._build_async_method("find_by_source_id", _wrap_guid)
        members["FindByIdAsync"] = self._build_async_method("find_by_id", _wrap_guid)
        members["FindBySourceContentUrlAsync"] = self._build_async_method("find_by_source_content_url", None)
        members["FindAllAsync"] = self._build_find_all_async()
        return members

class PyDestinationContentReferenceFinderBase(Generic[TContent]):
    """Generic base class for destination content reference finders."""
    
    _wrapper_builder = _PyDestinationContentReferenceFinderWrapperBuilder
    
    def find_by_source_location(self, source_location: PyContentLocation) -> Union[PyContentReference, None]:
        """Finds the destination content reference for the source content reference location.
        
        Args:
            source_location: The source content reference location.
        
        Returns: The found destination content reference, or None if no content reference was found.
        """
        return None
    
    def find_by_mapped_location(self, mapped_location: PyContentLocation) -> Union[PyContentReference, None]:
        """Finds the destination content reference for the mapped destination content reference location.
        
        Args:
            mapped_location: The destination mapped content reference location.
        
        Returns: The found destination content reference, or None if no content reference was found.
        """
        return None
    
    def find_by_source_id(self, source_id: UUID) -> Union[PyContentReference, None]:
        """Finds the destination content reference for the source content reference unique identifier.
        
        Args:
            source_id: The source content reference unique identifier.
        
        Returns: The found destination content reference, or None if no content reference was found.
        """
        return None
    
    def find_by_id(self, id: UUID) -> Union[PyContentReference, None]:
        """Finds the content reference by its unique identifier.
        
        Args:
            id: The unique identifier.
        
        Returns: The found content reference, or None if no content reference was found.
        """
        return None
    
    def find_by_source_content_url(self, source_content_url: str) -> Union[PyContentReference, None]:
        """Finds the destination content reference for the source content reference URL.
        
        Args:
            source_content_url: The source content reference URL.
        
        Returns: The found destination content reference, or None if no content reference was found.
        """
        return None
    
    def find_all(self) -> Sequence[PyContentReference]:
        """Finds all available content references.
        
        Returns: The found content references.
        """
        return []

class _PySourceContentReferenceFinderWrapperBuilder(_PyContentReferenceFinderWrapperBuilderBase):
    """Wrapper that makes PySourceContentReferenceFinderBase callable from C#."""
    
    @staticmethod
    def dotnet_service() -> type:
        return ISourceContentReferenceFinder
    
    @property
    def python_content_type(self) -> type:
        return self.python_generic_types[0]

    @property
    def dotnet_content_type(self) -> type:
        return self.dotnet_generic_types[0]
    
    def get_wrapper_base_type(self) -> type:
        return ISourceContentReferenceFinder[self.dotnet_content_type]

    def add_wrapper_members(self, members: dict[str, Any]) -> dict[str, Any]:
        members["FindBySourceLocationAsync"] = self._build_async_method("find_by_source_location", _wrap_content_location)
        members["FindByIdAsync"] = self._build_async_method("find_by_id", _wrap_guid)
        members["FindAllAsync"] = self._build_find_all_async()
        return members

class PySourceContentReferenceFinderBase(Generic[TContent]):
    """Generic base class for source content reference finders."""
    
    _wrapper_builder = _PySourceContentReferenceFinderWrapperBuilder
    
    def find_by_source_location(self, source_location: PyContentLocation) -> Union[PyContentReference, None]:
        """Finds the source content reference for the source content reference location.
        
        Args:
            source_location: The source content reference location.
        
        Returns: The found source content reference, or None if no content reference was found.
        """
        return None
    
    def find_by_id(self, id: UUID) -> Union[PyContentReference, None]:
        """Finds the content reference by its unique identifier.
        
        Args:
            id: The unique identifier.
        
        Returns: The found content reference, or None if no content reference was found.
        """
        return None
    
    def find_all(self) -> Sequence[PyContentReference]:
        """Finds all available content references.
        
        Returns: The found content references.
        """
        return []
