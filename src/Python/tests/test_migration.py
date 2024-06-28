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

# Make sure the test can find the module
import sys
from os.path import abspath
from pathlib import Path
import uuid
import pytest

from tableau_migration.migration import (
    PyContentLocation,
    PyMigrationManifest, 
    PyMigrationResult)
from tableau_migration.migration_engine import (
    PyMigrationPlan)

from tableau_migration.migration_engine_migrators import PyMigrator

import System
from Tableau.Migration import (
    ContentLocation,
    IMigrator, 
    IMigrationManifest, 
    IMigrationPlan,
    MigrationResult,
    MigrationCompletionStatus)
from Tableau.Migration.Engine.Manifest import (
    MigrationManifest, 
    IMigrationManifestEditor)
import Moq
        

class TestPyMigrator():
    def test_init(self):
        from tableau_migration.migration_engine_migrators import PyMigrator
        PyMigrator()
    
    def test_migration_migrator_execute_without_manifest(self):
        migrator_mock = Moq.Mock[IMigrator]()
        plan_mock = Moq.Mock[IMigrationPlan]()

        migrator = PyMigrator()
        migrator._migrator = migrator_mock.Object
    
        plan = PyMigrationPlan(plan_mock.Object)

        migrator.execute(plan)

        invokedMethodNames = [methodInfo.Method.Name for methodInfo in migrator_mock.Invocations]

        assert "ExecuteAsync" in invokedMethodNames


    def test_migration_migrator_execute_with_manifest(self):
        migrator_mock = Moq.Mock[IMigrator]()
        manifest_mock = Moq.Mock[IMigrationManifest]()
        plan_mock = Moq.Mock[IMigrationPlan]()

        migrator = PyMigrator()
        migrator._migrator = migrator_mock.Object

        manifest = PyMigrationManifest(manifest_mock.Object)

        plan = PyMigrationPlan(plan_mock.Object)
    
        migrator.execute(plan, previous_manifest=manifest)

        invokedMethodNames = [methodInfo.Method.Name for methodInfo in migrator_mock.Invocations]

        assert "ExecuteAsync" in invokedMethodNames


    def test_migration_plan_builder_ctor(self):
        """
        Verify that the PyMigrationPlanBuilder can be used
        """
        from tableau_migration.migration_engine import PyMigrationPlanBuilder
        PyMigrationPlanBuilder()


class TestPyMigrationResult():
    def test_init(self):
        status = MigrationCompletionStatus.Completed
        manifest_mock = Moq.Mock[IMigrationManifestEditor]()
        result = MigrationResult(status, manifest_mock.Object)

        PyMigrationResult(result)

class TestPyMigrationManifest():
    def test_init(self):
        manifest_mock = Moq.Mock[IMigrationManifestEditor]()

        PyMigrationManifest(manifest_mock.Object)


    def test_add_error_list(self):
        manifest_mock = Moq.Mock[IMigrationManifestEditor]()
        manifest = PyMigrationManifest(manifest_mock.Object)

        errors = [System.Exception(), System.Exception()]

        manifest.add_errors(errors)
        invokedMethodNames = [methodInfo.Method.Name for methodInfo in manifest_mock.Invocations]
        assert "AddErrors" in invokedMethodNames


    def test_add_error_exception(self):
        manifest_mock = Moq.Mock[IMigrationManifestEditor]()
        manifest = PyMigrationManifest(manifest_mock.Object)

        manifest.add_errors(System.Exception())

        invokedMethodNames = [methodInfo.Method.Name for methodInfo in manifest_mock.Invocations]
        assert "AddErrors" in invokedMethodNames

    def test_add_error_throw_on_bad_type(self):
        manifest_mock = Moq.Mock[IMigrationManifestEditor]()
        manifest = PyMigrationManifest(manifest_mock.Object)
        
        with pytest.raises(Exception):
            manifest.add_errors(Exception()) # Passing a python exception which is not valid

        invokedMethodNames = [methodInfo.Method.Name for methodInfo in manifest_mock.Invocations]
        # dotnet function was not called, as there was no function to call that takes
        # a python exception
        assert not invokedMethodNames
        
class TestPyContentLocation():    
    def test_path_segments(self):
        dotnet = ContentLocation(["parent", "child", "item"])
        py = PyContentLocation(dotnet)
        
        path = py.path_segments
        assert path == ["parent", "child", "item"]
# region _generated

from enum import IntEnum # noqa: E402, F401
from tableau_migration.migration_api_rest import PyRestIdentifiable # noqa: E402, F401
from typing import Sequence # noqa: E402, F401
from typing_extensions import Self # noqa: E402, F401

import System # noqa: E402

from Tableau.Migration import (  # noqa: E402, F401
    ContentLocation,
    IContentReference,
    IResult
)

from tableau_migration.migration import (  # noqa: E402, F401
    PyContentLocation,
    PyContentReference,
    PyMigrationCompletionStatus,
    PyResult
)


from Tableau.Migration import MigrationCompletionStatus

# Extra imports for tests.
from tests.helpers.autofixture import AutoFixtureTestBase # noqa: E402, F401

class TestPyContentLocationGenerated(AutoFixtureTestBase):
    
    def test_ctor(self):
        dotnet = self.create(ContentLocation)
        py = PyContentLocation(dotnet)
        assert py._dotnet == dotnet
    
    def test_path_segments_getter(self):
        dotnet = self.create(ContentLocation)
        py = PyContentLocation(dotnet)
        assert len(dotnet.PathSegments) != 0
        assert len(py.path_segments) == len(dotnet.PathSegments)
    
    def test_path_separator_getter(self):
        dotnet = self.create(ContentLocation)
        py = PyContentLocation(dotnet)
        assert py.path_separator == dotnet.PathSeparator
    
    def test_path_getter(self):
        dotnet = self.create(ContentLocation)
        py = PyContentLocation(dotnet)
        assert py.path == dotnet.Path
    
    def test_name_getter(self):
        dotnet = self.create(ContentLocation)
        py = PyContentLocation(dotnet)
        assert py.name == dotnet.Name
    
    def test_is_empty_getter(self):
        dotnet = self.create(ContentLocation)
        py = PyContentLocation(dotnet)
        assert py.is_empty == dotnet.IsEmpty
    
class TestPyContentReferenceGenerated(AutoFixtureTestBase):
    
    def test_ctor(self):
        dotnet = self.create(IContentReference)
        py = PyContentReference(dotnet)
        assert py._dotnet == dotnet
    
    def test_content_url_getter(self):
        dotnet = self.create(IContentReference)
        py = PyContentReference(dotnet)
        assert py.content_url == dotnet.ContentUrl
    
    def test_location_getter(self):
        dotnet = self.create(IContentReference)
        py = PyContentReference(dotnet)
        assert py.location == None if dotnet.Location is None else PyContentLocation(dotnet.Location)
    
    def test_name_getter(self):
        dotnet = self.create(IContentReference)
        py = PyContentReference(dotnet)
        assert py.name == dotnet.Name
    
class TestPyResultGenerated(AutoFixtureTestBase):
    
    def test_ctor(self):
        dotnet = self.create(IResult)
        py = PyResult(dotnet)
        assert py._dotnet == dotnet
    
    def test_success_getter(self):
        dotnet = self.create(IResult)
        py = PyResult(dotnet)
        assert py.success == dotnet.Success
    
    def test_errors_getter(self):
        dotnet = self.create(IResult)
        py = PyResult(dotnet)
        assert len(dotnet.Errors) != 0
        assert len(py.errors) == len(dotnet.Errors)
    

# endregion

