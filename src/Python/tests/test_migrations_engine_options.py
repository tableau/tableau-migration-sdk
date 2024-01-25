# Make sure the test can find the module
import sys
from os.path import abspath
from pathlib import Path
import uuid
import pytest
from tableau_migration.migration import (
   get_service_provider,
   get_service )

from tableau_migration.migration_engine_options import (
    PyMigrationPlanOptionsBuilder,
    PyMigrationPlanOptionsCollection)

import System

from Tableau.Migration.Tests import TestPlanOptions

from Tableau.Migration.Engine.Hooks import IMigrationHook

from Tableau.Migration.Engine.Options import (
    IMigrationPlanOptionsBuilder,
    IMigrationPlanOptionsCollection)

import Moq
_dist_path = abspath(Path(__file__).parent.resolve().__str__() + "/../src/tableau_migration")
sys.path.append(_dist_path)


class TestMigrationPlanOptions():
    def test_init_collections(self):
        migration_plan_options_colllection_mock = Moq.Mock[IMigrationPlanOptionsCollection]()
        PyMigrationPlanOptionsCollection(migration_plan_options_colllection_mock.Object)
    
    def test_get(self):
        """Verify that the MigrationPlanOptionsBuilder can return the options that were configured"""
        input_option = TestPlanOptions()

        services = get_service_provider()
        dotnet_plan_options_builder = get_service(services, IMigrationPlanOptionsBuilder)
        
        builder = PyMigrationPlanOptionsBuilder(dotnet_plan_options_builder) 
        builder.configure(input_option)

        options = builder.build().get(TestPlanOptions)

        assert input_option == options
