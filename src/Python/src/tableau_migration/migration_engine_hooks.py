"""Wrapper for classes in Tableau.Migration.Engine.Hooks namespace."""
from inspect import isclass
from typing import Type, TypeVar
from typing_extensions import Self
from System import Func, IServiceProvider
from Tableau.Migration.Engine.Hooks import IMigrationHookBuilder, IMigrationHookFactoryCollection

T = TypeVar("T")

class PyMigrationHookFactoryCollection():
    """Interface for an object that contains MigrationHookFactorys registered for each hook type."""

    _dotnet_base = IMigrationHookFactoryCollection

    def __init__(self, migration_hook_factory_collection: IMigrationHookFactoryCollection) -> None:
        """Default init.
        
        Returns: None.
        """
        self._migration_hook_factory_collection = migration_hook_factory_collection

    def get_hooks(self, type_to_get: Type[T]):
        """Gets the MigrationHookFactorys for the given hook type.
        
        This type has to be an interface that inherits from IMigrationHook.
        """
        return self._migration_hook_factory_collection.GetHooks[type_to_get]()

class PyMigrationHookBuilder():
    """Default IMigrationHookBuilder implementation."""

    _dotnet_base = IMigrationHookBuilder

    def __init__(self, migration_hook_builder: IMigrationHookBuilder) -> None:
        """Default init.
        
        Returns: None.
        """
        self._migration_hook_builder = migration_hook_builder


    def clear(self) -> Self:
        """Removes all currently registered hooks.

        Returns:
            The same hook builder object for fluent API calls.
        """
        self._migration_hook_builder.Clear()
        return self


    def add(self,input_0,input_1=None,input_2=None) -> Self:
        """Adds a an object to execute one or more hooks.

        Args:
            input_0: Either: 
                1) The hook to execute, or;
                2) The hook type to execute, or;
                3) The hook type to execute, or;
            input_1: Either:
                1) None, or;
                2) None, or the function to resolve the type by using the service provider, or;
                3) The context type for a callback function;
            input_2: Either:
                1) None, or;
                2) None, or;
                3) The callback function that will return the context type;

        Returns:
            The same hook builder object for fluent API calls.
        """
        if isclass(input_0) and input_1 is None:
            self._migration_hook_builder.Add[input_0]()
        elif isclass(input_0) and input_1 is not None and isinstance(input_1,Func[IServiceProvider, input_0]):
            self._migration_hook_builder.Add[input_0](input_1)
        elif isclass(input_0) and input_1 is not None and isclass(input_1) and input_2 is not None and isinstance(input_2,Func[input_1, input_1]):
            self._migration_hook_builder.Add[input_0,input_1](input_2)
        else:
            self._migration_hook_builder.Add(input_0)
        return self


    def build(self) -> PyMigrationHookFactoryCollection:
        """Builds an immutable collection from the currently added hooks.

        Returns:
            The created collection.
        """
        return PyMigrationHookFactoryCollection(self._migration_hook_builder.Build())