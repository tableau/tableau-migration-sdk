"""Wrapper for classes in Tableau.Migration.Engine.Hooks.Mappings namespace."""
from inspect import isclass
from typing_extensions import Self
from System import IServiceProvider, Func
from Tableau.Migration.Engine.Hooks.Mappings import IContentMappingBuilder,ContentMappingContext
from tableau_migration.migration_engine_hooks import PyMigrationHookFactoryCollection

class PyContentMappingBuilder():
    """Default IContentMappingBuilder implementation."""

    _dotnet_base = IContentMappingBuilder

    def __init__(self, content_mapping_builder: IContentMappingBuilder) -> None:
        """Default init.
        
        Returns: None.
        """
        self._content_mapping_builder = content_mapping_builder


    def clear(self) -> Self:
        """Removes all currently registered mappings.

        Returns:
            The same mapping builder object for fluent API calls
        """
        self._content_mapping_builder.Clear()
        return self


    def add(self,input_0,input_1,input_2=None) -> Self:
        """Adds a an object to execute one or more mappings.

        Args:
            input_0: Either: 
                1) The content type linked to the mapping, or;
                2) The mapping type to execute, or;
                3) The content type for a callback function;
            input_1: Either:
                1) The mapping to execute, or;
                2) The content type linked to the mapping, or;
                3) The callback function that will return the content type;
            input_2: Either:
                1) None, or;
                2) None, or the function to resolve the mapping type by using the service provider, or;
                3) None;

        Returns:
            The same mapping builder object for fluent API calls.
        """
        if isclass(input_0)  and isclass(input_1) and input_2 is None:
            self._content_mapping_builder.Add[input_0,input_1]()
        elif isclass(input_0) and isclass(input_1) and input_2 is not None and isinstance(input_2,Func[IServiceProvider, input_0]):
            self._content_mapping_builder.Add[input_0,input_1](input_2)
        elif isclass(input_0) and isinstance(input_1,Func[ContentMappingContext[input_0], ContentMappingContext[input_0]]):
            self._content_mapping_builder.Add[input_0](input_1)
        else:
            self._content_mapping_builder.Add[input_0](input_1)
        return self


    def by_content_type(self):
        """Gets the currently registered hook factories by their content types.

        Returns:
            The hook factories by their content types.
        """
        return self._content_mapping_builder.ByContentType()


    def build(self) -> Self:
        """Builds an immutable collection from the currently added mappings.

        Returns:
            The created collection.
        """
        return PyMigrationHookFactoryCollection(self._content_mapping_builder.Build())
