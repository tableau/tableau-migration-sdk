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

"""Python wrappers for C# paging classes."""

from typing import Callable, Generic, TypeVar, List, Union


from System.Collections.Generic import List as DotNetList
from System.Collections.Immutable import ImmutableList
from Tableau.Migration.Paging import MemoryPager, IPager


from tableau_migration.migration_interop import _PyWrapperBuilderBase, _unwrap


T = TypeVar("T")


class _PyPagerWrapperBuilderBase(_PyWrapperBuilderBase):
    """Base wrapper for pager classes that implement C# IPager interface."""
    
    def __init__(self, inner_type: type) -> None:
        super().__init__(inner_type)
    
    def get_wrapper_base_type(self) -> type:
        return IPager[self.dotnet_generic_types[0]]
    
    def add_wrapper_members(self, members: dict) -> dict:
        
        def next_page_async(wrapper, cancel):
            return wrapper._pager.NextPageAsync(cancel)
        
        members["NextPageAsync"] = next_page_async
        return members


class _PyEmptyPagerWrapperBuilder(_PyPagerWrapperBuilderBase):
    
    def __init__(self, inner_type: type) -> None:
        super().__init__(inner_type)

    def add_extra_wrapper_init(self) -> Union[Callable, None]:
        def init_pager(wrapper):
            dotnet_type = wrapper.dotnet_generic_types[0]

            empty_list = ImmutableList[dotnet_type].Empty
            wrapper._pager = MemoryPager[dotnet_type](empty_list, 1)

        return init_pager

class _PyEmptyPager(Generic[T]):
    _wrapper_builder = _PyEmptyPagerWrapperBuilder

    # Stub object, paging is defined in the wrapper-created _pager


def empty_pager(content_type: type):
    """Creates an empty pager.
        
    Args:
        content_type: The content type of the pager items.

    Returns: The pager.
    """
    return _PyEmptyPagerWrapperBuilder(_PyEmptyPager[content_type]).factory(None)


class _PyMemoryPagerWrapperBuilder(_PyPagerWrapperBuilderBase):
    def __init__(self, inner_type: type, python_items: list, page_size: int) -> None:
        super().__init__(inner_type)
        self._python_items = python_items
        self._page_size = page_size

    def add_extra_wrapper_init(self) -> Union[Callable, None]:
        def init_pager(wrapper):
            dotnet_type = wrapper.dotnet_generic_types[0]

            # Convert Python collection to C# collection
            dotnet_list = DotNetList[dotnet_type]()
            for item in self._python_items:
                dotnet_list.Add(_unwrap(item))

            wrapper._pager = MemoryPager[dotnet_type](dotnet_list, self._page_size)

        return init_pager


class _PyMemoryPager(Generic[T]):
    _wrapper_builder = _PyMemoryPagerWrapperBuilder

    # Stub object, paging is defined in the wrapper-created _pager


def memory_pager(content_type: type, python_items: List[T], page_size: int = 100):
    """Creates a Python-wrapped memory pager from Python collection.
        
    Args:
        content_type: The content type of the pager items.
        python_items: Python native collection (list, tuple, etc.) to paginate.
        page_size: Size of each page.

    Returns: The pager.
    """
    return _PyMemoryPagerWrapperBuilder(_PyMemoryPager[content_type], python_items, page_size).factory(None)