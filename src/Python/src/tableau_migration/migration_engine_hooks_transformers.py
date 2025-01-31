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

"""Wrapper for classes in Tableau.Migration.Engine.Hooks.Transformers namespace."""

from typing import Callable, Union
from typing_extensions import Self

from System import IServiceProvider, Func
from Tableau.Migration.Engine.Hooks.Transformers import IContentTransformerBuilder
from tableau_migration.migration_engine_hooks import PyMigrationHookFactoryCollection

class PyContentTransformerBuilder():
    """Default IContentTransformerBuilder implementation."""

    _dotnet_base = IContentTransformerBuilder

    def __init__(self, content_transformer_builder: IContentTransformerBuilder) -> None:
        """Default init.

        Args:
            content_transformer_builder: An object with methods to build IContentTransformer"/
        
        Returns: None.
        """
        self._content_transformer_builder = content_transformer_builder


    def clear(self) -> Self:
        """Removes all currently registered transformers.

        Returns:
            The same transformer builder object for fluent API calls.
        """
        self._content_transformer_builder.Clear()
        return self


    def add(self, input_0: type, input_1: Union[Callable, None] = None, is_xml: bool = False) -> Self:
        """Adds an object or function to execute transformers.

        Args:
            input_0: Either: 
                1) The transformer type to execute, or
                2) The content type for a callback function
            input_1: Either:
                1) The callback function to execute, or
                2) None
            is_xml: True if the given callback function is an xml transformer callback, otherwise false.

        Returns:
            The same mapping builder object for fluent API calls.
        """
        from migration_engine_hooks_transformers_interop import _PyTransformerWrapper, _PyXmlTransformerWrapper

        if input_1 is None:
            wrapper = input_0._wrapper(input_0)
        else:
            wrap_type = _PyXmlTransformerWrapper if is_xml else _PyTransformerWrapper
            wrapper = wrap_type(input_0, input_1)
        
        self._content_transformer_builder.Add[wrapper.wrapper_type, wrapper.dotnet_publish_type](Func[IServiceProvider, wrapper.wrapper_type](wrapper.factory))
        
        return self
    

    def by_content_type(self):
        """Gets the currently registered hook factories by their content types.

        Returns:
            The hook factories by their content types.
        """
        return self._content_transformer_builder.ByContentType()


    def build(self) -> Self:
        """Builds an immutable collection from the currently added transformers.

        Returns:
            The created collection.
        """
        return PyMigrationHookFactoryCollection(self._content_transformer_builder.Build())
