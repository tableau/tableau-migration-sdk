"""Wrapper for classes in Tableau.Migration.Engine.Options namespace."""
from typing import Type
from typing_extensions import Self, TypeVar

import System

from Tableau.Migration.Engine.Options import(
    IMigrationPlanOptionsBuilder,
    IMigrationPlanOptionsCollection)
import tableau_migration

T = TypeVar("T")
class PyMigrationPlanOptionsCollection:
    """Default IMigrationPlanOptionsCollection implementation."""
    
    _dotnet_base = IMigrationPlanOptionsCollection
    
    def __init__(self, migration_plan_options_collection: IMigrationPlanOptionsCollection):
        """Default init.
        
        Returns: None.
        """
        self._migration_plan_options_collection = migration_plan_options_collection


    def get(self, type_to_get: Type[T], services: System.IServiceProvider = tableau_migration.migration.get_service_provider()) -> Type[T]:
        """Gets the options for the given type.
        
        or null if no options for the given type have been registered.

        Returns:
            The options for the given type, or null.
        """
        return self._migration_plan_options_collection.Get[type_to_get](services)

class PyMigrationPlanOptionsBuilder():
    """Default IMigrationPlanOptionsBuilder implementation."""

    _dotnet_base = IMigrationPlanOptionsBuilder

    def __init__(self, migration_plan_options_builder: IMigrationPlanOptionsBuilder):
        """Default init.
        
        Returns: None.
        """
        self._migration_plan_options_builder = migration_plan_options_builder

    
    def configure(self, options: Type[T]) -> Self:
        """Sets the configuration for a given options type.

        Returns:
            The same options builder, for fluent API usage.
        """
        self._migration_plan_options_builder.Configure(options)
        return self


    def build(self) -> PyMigrationPlanOptionsCollection:
        """Builds the options collection.

        Returns:
            The options collection.
        """
        return PyMigrationPlanOptionsCollection(self._migration_plan_options_builder.Build()) 



