# Copyright (c) 2024, Salesforce, Inc.
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

from typing import TypeVar
from uuid import UUID
from xml.etree import ElementTree

from tableau_migration.migration_content import PyPublishableWorkbook, PyUser
from tableau_migration.migration_engine_hooks_transformers import PyContentTransformerBuilder
from tableau_migration.migration_engine_hooks_transformers_interop import PyContentTransformerBase, PyXmlContentTransformerBase
from tableau_migration.migration_services import ScopedMigrationServices

from tests.helpers.autofixture import AutoFixtureTestBase

from System import IServiceProvider
from System.IO import MemoryStream, StreamReader
from System.Threading import CancellationToken
from System.Xml import XmlWriter
from System.Xml.Linq import LoadOptions, XDocument, XName
from Tableau.Migration.Content import IPublishableWorkbook, IUser
from Tableau.Migration.Engine.Hooks import IMigrationHook
from Tableau.Migration.Engine.Hooks.Transformers import ContentTransformerBuilder, IContentTransformer, IXmlContentTransformer

T = TypeVar("T")

class PyTransformer(PyContentTransformerBase[T]):
        
    def transform(self, ctx: T) -> T:
        ctx.email = "test1"
        return ctx

class PyUserTransformer(PyTransformer[PyUser]):
    pass  

def transform_users(ctx: PyUser) -> PyUser:
    ctx.email = "test2"
    return ctx

def transform_users_services(ctx: PyUser, services: ScopedMigrationServices) -> PyUser:
    ctx.email = "test3"
    return ctx

class TestTransformerInterop(AutoFixtureTestBase):
    def test_transformer_interop_class(self):
        hook_builder = PyContentTransformerBuilder(ContentTransformerBuilder())
        
        result = hook_builder.add(PyUserTransformer)
        assert result is hook_builder
        
        hook_factories = hook_builder.build().get_hooks(IContentTransformer[IUser])
        assert len(hook_factories) == 1

        services = self.create(IServiceProvider)
        ctx = self.create(IUser)
        
        hook = hook_factories[0].Create[IMigrationHook[IUser]](services)
        hook_result = hook.ExecuteAsync(ctx, CancellationToken(False)).GetAwaiter().GetResult()

        assert hook_result.Email == "test1"
        assert ctx.Email == "test1"

    def test_transformer_interop_callback(self):
        hook_builder = PyContentTransformerBuilder(ContentTransformerBuilder())

        ctx = self.create(IUser)

        result = hook_builder.add(PyUser, transform_users)
        assert result is hook_builder

        hook_factories = hook_builder.build().get_hooks(IContentTransformer[IUser])
        assert len(hook_factories) == 1
        
        services = self.create(IServiceProvider)
        
        hook = hook_factories[0].Create[IMigrationHook[IUser]](services)
        hook_result = hook.ExecuteAsync(ctx, CancellationToken(False)).GetAwaiter().GetResult()

        assert hook_result.Email == "test2"
        assert ctx.Email == "test2"
    
    def test_transformer_interop_callback_services(self):
        hook_builder = PyContentTransformerBuilder(ContentTransformerBuilder())

        ctx = self.create(IUser)

        result = hook_builder.add(PyUser, transform_users_services)
        assert result is hook_builder

        hook_factories = hook_builder.build().get_hooks(IContentTransformer[IUser])
        assert len(hook_factories) == 1
        
        services = self.create(IServiceProvider)
        
        hook = hook_factories[0].Create[IMigrationHook[IUser]](services)
        hook_result = hook.ExecuteAsync(ctx, CancellationToken(False)).GetAwaiter().GetResult()

        assert hook_result.Email == "test3"
        assert ctx.Email == "test3"
        
class PyXmlTransformer(PyXmlContentTransformerBase[T]):
    def transform(self, ctx: T, xml) -> None:
        pass

_test_twb = """<?xml version='1.0' encoding='utf-8' ?>

<!-- build 20231.24.0312.1557                               -->
<workbook source-build='2023.1.11 (20231.24.0312.1557)' source-platform='win' version='18.1' xmlns:user='http://www.tableausoftware.com/xml/user'>
  <user:test />
</workbook>

"""

_expected_twb = """<?xml version="1.0" encoding="utf-8"?>

<!-- build 20231.24.0312.1557                               -->
<workbook xmlns:user="http://www.tableausoftware.com/xml/user" source-build="2023.1.11 (20231.24.0312.1557)" source-platform="win" version="18.1">
  <user:test />
  <test2 a="b" />
</workbook>

"""

def _transform_xml_content(xml: ElementTree.Element) -> None:
    xml.find("user:test", {"user": "http://www.tableausoftware.com/xml/user"}).tail = "\n  "
    sub = ElementTree.SubElement(xml, "test2", { "a": "b" })
    sub.tail = "\n"

class PyWorkbookXmlTransformer(PyXmlTransformer[PyPublishableWorkbook]):
    
    def needs_xml_transforming(self, ctx: PyPublishableWorkbook) -> bool:
        return ctx.description == "mark"

    def transform(self, ctx: PyPublishableWorkbook, xml: ElementTree.Element) -> None:
        ctx.description = xml.get("version")
        _transform_xml_content(xml)
        
def transform_workbook_xml(ctx: PyPublishableWorkbook, xml: ElementTree.Element) -> None:
    ctx.description = xml.get("version")
    _transform_xml_content(xml)

def transform_workbook_xml_services(ctx: PyPublishableWorkbook, xml: ElementTree.Element, services: ScopedMigrationServices) -> None:
    ctx.description = xml.get("version")
    _transform_xml_content(xml)

class TestXmlTransformerInterop(AutoFixtureTestBase):
    
    def _clean_xml_text(self, xml_text: str) -> str:
        return xml_text.replace("\r", "")

    # Helper method to save XDocument to string that includes the XML declaration.
    def _save_xml(self, xdoc: XDocument) -> str:
        stream = MemoryStream()
        writer = XmlWriter.Create(stream)
        xdoc.Save(writer)
        writer.Flush()
        stream.Position = 0
        reader = StreamReader(stream)
        return self._clean_xml_text(reader.ReadToEnd())
    
    def test_transformer_interop_class(self):
        hook_builder = PyContentTransformerBuilder(ContentTransformerBuilder())
        
        result = hook_builder.add(PyWorkbookXmlTransformer)
        assert result is hook_builder
        
        hook_factories = hook_builder.build().get_hooks(IContentTransformer[IPublishableWorkbook])
        assert len(hook_factories) == 1

        services = self.create(IServiceProvider)
        ctx = self.create(IPublishableWorkbook)
        xml = XDocument.Parse(_test_twb, LoadOptions.PreserveWhitespace)
        
        hook = hook_factories[0].Create[IXmlContentTransformer[IPublishableWorkbook]](services)
        hook.TransformAsync(ctx, xml, CancellationToken(False)).GetAwaiter().GetResult()

        assert ctx.Description == "18.1"

        saved_xml = self._save_xml(xml)
        assert saved_xml == self._clean_xml_text(_expected_twb)

    def test_transformer_needs_transforming(self):
        hook_builder = PyContentTransformerBuilder(ContentTransformerBuilder())
        
        result = hook_builder.add(PyWorkbookXmlTransformer)
        assert result is hook_builder
        
        hook_factories = hook_builder.build().get_hooks(IContentTransformer[IPublishableWorkbook])
        assert len(hook_factories) == 1

        services = self.create(IServiceProvider)
        ctx = self.create(IPublishableWorkbook)
        
        hook = hook_factories[0].Create[IXmlContentTransformer[IPublishableWorkbook]](services)
        
        ctx.Description = "notmark"

        assert hook.NeedsXmlTransforming(ctx) == False

        ctx.Description = "mark"
        
        assert hook.NeedsXmlTransforming(ctx) == True

    def test_transformer_interop_callback(self):
        hook_builder = PyContentTransformerBuilder(ContentTransformerBuilder())

        ctx = self.create(IPublishableWorkbook)
        xml = XDocument.Parse(_test_twb, LoadOptions.PreserveWhitespace)

        result = hook_builder.add(PyPublishableWorkbook, transform_workbook_xml, is_xml = True)
        assert result is hook_builder

        hook_factories = hook_builder.build().get_hooks(IContentTransformer[IPublishableWorkbook])
        assert len(hook_factories) == 1
        
        services = self.create(IServiceProvider)
        
        hook = hook_factories[0].Create[IXmlContentTransformer[IPublishableWorkbook]](services)
        hook.TransformAsync(ctx, xml, CancellationToken(False)).GetAwaiter().GetResult()

        assert ctx.Description == "18.1"
        assert self._save_xml(xml) == self._clean_xml_text(_expected_twb)
    
    def test_transformer_interop_callback_services(self):
        hook_builder = PyContentTransformerBuilder(ContentTransformerBuilder())

        ctx = self.create(IPublishableWorkbook)
        xml = XDocument.Parse(_test_twb, LoadOptions.PreserveWhitespace)

        result = hook_builder.add(PyPublishableWorkbook, transform_workbook_xml_services, is_xml = True)
        assert result is hook_builder

        hook_factories = hook_builder.build().get_hooks(IContentTransformer[IPublishableWorkbook])
        assert len(hook_factories) == 1
        
        services = self.create(IServiceProvider)
        
        hook = hook_factories[0].Create[IXmlContentTransformer[IPublishableWorkbook]](services)
        hook.TransformAsync(ctx, xml, CancellationToken(False)).GetAwaiter().GetResult()

        assert ctx.Description == "18.1"
        assert self._save_xml(xml) == self._clean_xml_text(_expected_twb)
        