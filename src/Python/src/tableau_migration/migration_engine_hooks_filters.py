"""Wrapper for classes in Tableau.Migration.Engine.Hooks.Filters namespace."""
from inspect import isclass
from typing_extensions import Self
from System import IServiceProvider, Func
from System.Collections.Generic import IEnumerable
from Tableau.Migration.Engine import ContentMigrationItem
from Tableau.Migration.Engine.Hooks.Filters import IContentFilterBuilder
from tableau_migration.migration_engine_hooks import PyMigrationHookFactoryCollection

class PyContentFilterBuilder():
    """Default IContentFilterBuilder implementation."""

    _dotnet_base = IContentFilterBuilder

    def __init__(self, content_filters_builder: IContentFilterBuilder) -> None:
        """Default init.
        
        Returns: None.
        """
        self._content_filters_builder = content_filters_builder


    def clear(self) -> Self:
        """Removes all currently registered filters.

        Returns:
            The same filter builder object for fluent API calls
        """
        self._content_filters_builder.Clear()
        return self


    def add(self,input_0,input_1,input_2=None) -> Self:
        """Adds a an object to execute one or more filters.

        Args:
            input_0: Either: 
                1) The content type linked to the filter, or;
                2) The filter type to execute, or;
                3) The content type for a callback function;
            input_1: Either:
                1) The filter to execute, or;
                2) The content type linked to the filter, or;
                3) The callback function that will return the content type;
            input_2: Either:
                1) None, or;
                2) None, or the function to resolve the filter type by using the service provider, or;
                3) None;

        Returns:
            The same filter builder object for fluent API calls.
        """
        if isclass(input_0) and isclass(input_1) and input_2 is None:
            self._content_filters_builder.Add[input_0,input_1]()
        elif isclass(input_0) and isclass(input_1) and input_2 is not None and isinstance(input_2,Func[IServiceProvider, input_0]):
            self._content_filters_builder.Add[input_0,input_1](input_2)
        elif isclass(input_0) and isinstance(input_1,Func[IEnumerable[ContentMigrationItem[input_0]], IEnumerable[ContentMigrationItem[input_0]]]):
            self._content_filters_builder.Add[input_0](input_1)
        else:
            self._content_filters_builder.Add[input_0](input_1)
        return self
    

    def by_content_type(self):
        """Gets the currently registered hook factories by their content types.

        Returns:
            The hook factories by their content types.
        """
        return self._content_filters_builder.ByContentType()


    def build(self) -> Self:
        """Builds an immutable collection from the currently added filters.

        Returns:
            The created collection.
        """
        return PyMigrationHookFactoryCollection(self._content_filters_builder.Build())