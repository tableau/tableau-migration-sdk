"""Wrapper for classes in Tableau.Migration.Engine.Hooks.Transformers namespace."""
from inspect import isclass
from typing_extensions import Self
from System import IServiceProvider, Func
from Tableau.Migration.Engine.Hooks.Transformers import IContentTransformerBuilder
from tableau_migration.migration_engine_hooks import PyMigrationHookFactoryCollection

class PyContentTransformerBuilder():
    """Default IContentTransformerBuilder implementation."""

    _dotnet_base = IContentTransformerBuilder

    def __init__(self, content_transformer_builder: IContentTransformerBuilder) -> None:
        """Default init.
        
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


    def add(self,input_0,input_1,input_2=None) -> Self:
        """Adds an object to execute one or more transformers.

        Args:
            input_0: Either: 
                1) The type linked to the transformer, or;
                2) The transformer type to execute, or;
                3) The content type for a callback function;
            input_1: Either:
                1) The transformer to execute, or;
                2) The content type linked to the transformer, or;
                3) The callback function that will return the content type;
            input_2: Either:
                1) None, or;
                2) None, or the function to resolve the transformer type by using the service provider, or;
                3) None;

        Returns:
            The same transformer builder object for fluent API calls.
        """
        if isclass(input_0) and isclass(input_1) and input_2 is None:
            self._content_transformer_builder.Add[input_0,input_1]()
        elif isclass(input_0) and isclass(input_1) and input_2 is not None and isinstance(input_2,Func[IServiceProvider, input_0]):
            self._content_transformer_builder.Add[input_0,input_1](input_2)
        elif isclass(input_0) and isinstance(input_1,Func[input_0, input_0]):
            self._content_transformer_builder.Add[input_0](input_1)
        else:
            self._content_transformer_builder.Add[input_0](input_1)
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
