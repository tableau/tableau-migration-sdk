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

"""Interoperability utility for transformers."""

import json
from inspect import signature
from typing import Callable, Generic, TypeVar
from xml.etree import ElementTree

from migration import _generic_wrapper
from migration_engine_hooks_interop import _PyHookWrapperBuilderBase

import System # System.Xml.Linq must be imported as System
from System.Threading.Tasks import Task
from Tableau.Migration.Engine.Hooks.Transformers import ContentTransformerBase, IJsonContentTransformer, IXmlContentTransformer

TPublish = TypeVar("TPublish")

# Register XML namespaces used in Tableau files (TWB/TDS).
# This ensures that namespace declarations are preserved in XML transformers.
ElementTree.register_namespace("user", "http://www.tableausoftware.com/xml/user")

class _PyTransformerWrapperBuilder(_PyHookWrapperBuilderBase):
    
    @property
    def python_publish_type(self) -> type:
        return self.python_generic_types[0]

    @property
    def dotnet_publish_type(self) -> type:
        return self.dotnet_generic_types[0]

    def get_wrapper_base_type(self) -> type:
        return ContentTransformerBase[self.dotnet_publish_type]

    @property
    def _wrapper_method_name(self) -> str:
        return "TransformAsync"

    def _wrapper_context_type(self) -> type:
        return self.dotnet_publish_type

    def _wrap_execute_method(self) -> Callable:
        def _wrap_transform(w):
            return w._inner.transform
        
        return _wrap_transform
    
    def _wrap_context_callback(self) -> Callable:
        def _wrap_context(ctx):
            return _generic_wrapper(ctx, self.dotnet_publish_type)
        
        return _wrap_context
    
class PyContentTransformerBase(Generic[TPublish]):
    """Generic base class for transformers."""

    _wrapper_builder = _PyTransformerWrapperBuilder

    def transform(self, item_to_transform: TPublish) -> TPublish:
        """Executes the transformation.
        
        Args:
            item_to_transform: The input context from the migration engine or previous hook.
            
        Returns:
            The context, potentially modified to pass on to the next hook or migration engine, or None to continue passing the input context.
        """
        return item_to_transform
    
class _PyXmlTransformerWrapperBuilder(_PyTransformerWrapperBuilder):

    @classmethod
    def read_xml(cls, xml: System.Xml.Linq.XDocument) -> ElementTree.Element:
        return ElementTree.fromstring(xml.ToString())
    
    @classmethod
    def write_xml(cls, orig_xml: System.Xml.Linq.XDocument, new_xml: ElementTree.Element) -> None:
        s = ElementTree.tostring(new_xml, "unicode")
        
        new_root = System.Xml.Linq.XElement.Parse(s, System.Xml.Linq.LoadOptions.PreserveWhitespace)
        orig_xml.Root.ReplaceWith(new_root)

    def get_wrapper_base_type(self) -> type:
        return IXmlContentTransformer[self.dotnet_publish_type]

    def _wrap_execute_method(self) -> Callable:
        def _wrap_transform_xml(w):
            return w._inner._transform_xml
        
        return _wrap_transform_xml
    
    def build_wrapper_execute(self, wrap_method: Callable, wrap_context: Callable, ctx_type: type, is_async: bool) -> Callable:
        def _transform_async(s, ctx, xml, cancel):
            wrap_method(s)(wrap_context(ctx), xml)
            return Task.CompletedTask

        return _transform_async
    
    def build_wrapper_execute_callback(self, callback: Callable, wrap_context: Callable, ctx_type: type, is_async: bool) -> Callable:
        def _transform_async(s, ctx, xml, cancel):
            py_xml = self.read_xml(xml)
            callback(wrap_context(ctx), py_xml)
            self.write_xml(xml, py_xml)
            
            return Task.CompletedTask

        def _transform_services_async(s, ctx, xml, cancel):
            py_xml = self.read_xml(xml)
            callback(wrap_context(ctx), py_xml, s.services)
            self.write_xml(xml, py_xml)

            return Task.CompletedTask

        return _transform_async if len(signature(callback).parameters) == 2 else _transform_services_async

    def set_extra_wrapper_members(self, members: dict, wrap_context: Callable) -> None:
        
        def _wrap_needs_transforming(w, ctx):
            return w._inner.needs_xml_transforming(wrap_context(ctx))

        members["NeedsXmlTransforming"] = _wrap_needs_transforming

class _PyJsonTransformerWrapperBuilder(_PyTransformerWrapperBuilder):

    @classmethod
    def read_json(cls, json_node: System.Text.Json.Nodes.JsonNode):
        return json.loads(json_node.ToJsonString())

    @classmethod
    def write_json(cls, orig_json: System.Text.Json.Nodes.JsonNode, new_json) -> None:
        new_json_node = System.Text.Json.Nodes.JsonNode.Parse(json.dumps(new_json))

        # Json transformers operate on file roots, which are expected to be JSON objects.
        # Copy values into the original root so references held by the SDK remain valid.
        if isinstance(orig_json, System.Text.Json.Nodes.JsonObject) and isinstance(new_json_node, System.Text.Json.Nodes.JsonObject):
            orig_obj = orig_json.AsObject()
            new_obj = new_json_node.AsObject()

            orig_obj.Clear()
            for kvp in new_obj:
                value = None if kvp.Value is None else kvp.Value.DeepClone()
                orig_obj.Add(kvp.Key, value)
            return

        raise TypeError("JSON transformer root must be a JSON object.")

    def get_wrapper_base_type(self) -> type:
        return IJsonContentTransformer[self.dotnet_publish_type]

    def _wrap_execute_method(self) -> Callable:
        def _wrap_transform_json(w):
            return w._inner._transform_json

        return _wrap_transform_json

    def build_wrapper_execute(self, wrap_method: Callable, wrap_context: Callable, ctx_type: type, is_async: bool) -> Callable:
        def _transform_async(s, ctx, json_node, cancel):
            wrap_method(s)(wrap_context(ctx), json_node)
            return Task.CompletedTask

        return _transform_async

    def build_wrapper_execute_callback(self, callback: Callable, wrap_context: Callable, ctx_type: type, is_async: bool) -> Callable:
        def _transform_async(s, ctx, json_node, cancel):
            py_json = self.read_json(json_node)
            callback(wrap_context(ctx), py_json)
            self.write_json(json_node, py_json)

            return Task.CompletedTask

        def _transform_services_async(s, ctx, json_node, cancel):
            py_json = self.read_json(json_node)
            callback(wrap_context(ctx), py_json, s.services)
            self.write_json(json_node, py_json)

            return Task.CompletedTask

        return _transform_async if len(signature(callback).parameters) == 2 else _transform_services_async

    def set_extra_wrapper_members(self, members: dict, wrap_context: Callable) -> None:

        def _wrap_needs_transforming(w, ctx):
            return w._inner.needs_json_transforming(wrap_context(ctx))

        members["NeedsJsonTransforming"] = _wrap_needs_transforming
    
class PyXmlContentTransformerBase(Generic[TPublish]):
    """Generic base class for XML transformers."""

    _wrapper_builder = _PyXmlTransformerWrapperBuilder
    
    def needs_xml_transforming(self, ctx: TPublish) -> bool:
        """Finds whether the content item needs any XML changes, returning false prevents file IO from occurring.
        
        Args:
            ctx: The content item to inspect.
            
        Returns: Whether or not the content item needs XML changes.
        """
        return True
    
    def _transform_xml(self, ctx: TPublish, xml) -> None:
        py_xml = _PyXmlTransformerWrapperBuilder.read_xml(xml)
        self.transform(ctx, py_xml)
        _PyXmlTransformerWrapperBuilder.write_xml(xml, py_xml)

    def transform(self, ctx: TPublish, xml: ElementTree.Element) -> None:
        """Transforms the XML of the content item.
        
        Args:
            ctx: The content item being transformed.
            xml: The XML of the content item to transform. Any changes made to the XML are persisted back to the file before publishing.
        """
        pass

class PyJsonContentTransformerBase(Generic[TPublish]):
    """Generic base class for JSON transformers."""

    _wrapper_builder = _PyJsonTransformerWrapperBuilder

    def needs_json_transforming(self, ctx: TPublish) -> bool:
        """Finds whether the content item needs any JSON changes, returning false prevents file IO from occurring.

        Args:
            ctx: The content item to inspect.

        Returns: Whether or not the content item needs JSON changes.
        """
        return True

    def _transform_json(self, ctx: TPublish, json_node) -> None:
        py_json = _PyJsonTransformerWrapperBuilder.read_json(json_node)
        self.transform(ctx, py_json)
        _PyJsonTransformerWrapperBuilder.write_json(json_node, py_json)

    def transform(self, ctx: TPublish, json_obj) -> None:
        """Transforms the JSON of the content item.

        Args:
            ctx: The content item being transformed.
            json_obj: The JSON of the content item to transform. Any changes made to the JSON are persisted back to the file before publishing.
        """
        pass
