# Hooks Implemented in Python

Because the Migration SDK is primarily written in C#, it uses [Python.NET](https://pythonnet.github.io/) to enable the implementation of hooks in Python. The Python hooks interoperate with the C# binaries to achieve the same results as writing hooks in C#.

The SDK's C# hook classes and interfaces are [asynchronous](https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/). Due to the limitations of [Python.NET](https://pythonnet.github.io/), the SDK provides synchronous wrappers to these interfaces. The names of these wrappers start with the keyword `ISync`. You will find references to those in the following examples. 

Various C# interfaces and classes are used in hooks. You will need to import them just like you do Python classes. You can search for them in the [API Reference](~/api/index.md) section of the documentation.

Example:

Here is one of the import statements you will need for filters
```python
from Tableau.Migration.Interop.Hooks.Filters import ISyncContentFilter
```
The namespace for `ISyncContentFilter` is `Tableau.Migration.Interop.Hooks.Filters`. 

## Object Registration

Python.NET allows Python objects to be registered as hooks, with some limitations. Python objects must declare a C# namespace field and must inherit from a class or an interface. The Python hook builders’ “add” method simply passes the Python object as normal.

Examples of Python object implementation:

```python

from Tableau.Migration.Interop.Hooks import ISyncMigrationHook
from Tableau.Migration.Engine.Hooks import IMigrationActionCompletedHook
from Tableau.Migration.Engine.Actions import IMigrationActionResult
from Tableau.Migration.Interop.Hooks.Filters import ISyncContentFilter
from Tableau.Migration.Interop.Hooks.Mappings import ISyncContentMapping
from Tableau.Migration.Interop.Hooks.Transformers import ISyncContentTransformer
from Tableau.Migration.Content import(
    IGroup,
    IUser,
    IProject)
from Tableau.Migration.Engine import ContentMigrationItem

class PyLogActionHook(ISyncMigrationHook[IMigrationActionResult], IMigrationActionCompletedHook):
    __namespace__ = "MyNamespace"
    
    def Execute(self, ctx):
        print("ACTION COMPLETED")
        return ctx

class PyTestGroupFilter(ISyncContentFilter[IGroup]):
    __namespace__ = "MyNamespace"
    
    def Execute(self, ctx):
        filtered = List[ContentMigrationItem[IGroup]]()
        for item in ctx:
            if "Test" not in item.SourceItem.Name:
                filtered.add(item)
        return filtered

class PyTestUsernameMapping(ISyncContentMapping[IUser]):
    __namespace__ = "MyNamespace"

    def Execute(self, ctx):
        domain = ctx.MappedLocation.Parent()
        return ctx.MapTo(domain.Append(ctx.ContentItem.Name + "@salesforce.com"))

class PyTestProjectTransformer(ISyncContentTransformer[IProject]):
    __namespace__ = "MyNamespace"
    
    def Execute(self, ctx):
        ctx.Description = "[From Server]\n" + ctx.Description
        return ctx

```

An example of Python plan builder hook binding for the above examples of object implementation:

```python
log_hook = PyLogActionHook()
filter_hook = PyTestGroupFilter()
mapping_hook = PyTestUsernameMapping()
transformer_hook = PyTestProjectTransformer()
planBuilder = PyMigrationPlanBuilder()
# For hooks
planBuilder._hooks.add(log_hook)
# For filters
planBuilder._filters.add(IGroup, filter_hook)
# For mappings
planBuilder._mappings.add(IUser, mapping_hook)
# For transformers
planBuilder._transformers.add(IProject, transformer_hook)
```

## Callback Registration

Due to Python.NET’s ability to cast Python functions to the C# Func type, Python callbacks can be used as hooks directly. A light “syntax sugar” layer is exposed for all hook types for Python caller convenience:

* The “add” method exposed by hook builder Python wrapper classes inspects if a Python function is passed as an argument. If so it casts the function to the proper Func C# type and invokes the C# hook builder “Add” method.
* Python callbacks are expected to take a single context object and may return a context object or None (Null). The Python function is expected to be synchronous.

Examples of Python Callback implementation:

```python
from Tableau.Migration.Content import IGroup
from Tableau.Migration.Engine import ContentMigrationItem
from System.Collections.Generic import List

def log_callback(ctx):
    print("ACTION COMPLETED")
    return ctx

def filter_group(ctx):
    filtered = List[ContentMigrationItem[IGroup]]()
    for item in ctx:
        if "Test" not in item.SourceItem.Name:
            filtered.add(item)
    return filtered

def map_test_user(ctx):
    domain = ctx.MappedLocation.Parent()
    return ctx.MapTo(domain.Append(ctx.ContentItem.Name + "@salesforce.com"))

def add_project_origin_desc(ctx):
    ctx.Description = "[From Server]\n" + ctx.Description
    return ctx
```

An example of Python plan builder hook binding for the above examples of callback implementation:

```python
planBuilder = PyMigrationPlanBuilder()
# For hooks
planBuilder._hooks.add(ISyncMigrationActionCompletedHook, IMigrationActionResult, Func[IMigrationActionResult, IMigrationActionResult](log_callback))
# For filters
planBuilder._filters.add(IGroup, Func[IEnumerable[ContentMigrationItem[IGroup]], IEnumerable[ContentMigrationItem[IGroup]]](filter_group))
# For mappings
planBuilder._mappings.add(IUser, Func[ContentMappingContext[IUser], ContentMappingContext[IUser]](map_test_user))
# For transformers
planBuilder._transformers.add(IProject, Func[IProject, IProject](add_project_origin_desc))
```

## Factory Registration

Python.NET does not provide out-of-the-box interoperability for Microsoft dependency injection, so a [DI](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection)-type factory cannot work with Python types. Because Python constructors are functions, a Python hook builder can register a non-[DI](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection) factory. This allows Python implementations to access the [scoped](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection#scoped) [DI](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection) service provider for manual dependency injection.

For a Factory Registration, we can keep the same examples from Python object implementation:

```python
from Tableau.Migration.Interop.Hooks import ISyncMigrationHook
from Tableau.Migration.Engine.Actions import IMigrationActionResult
from Tableau.Migration.Engine.Hooks import IMigrationActionCompletedHook
from Tableau.Migration.Interop.Hooks.Filters import ISyncContentFilter
from Tableau.Migration.Interop.Hooks.Mappings import ISyncContentMapping
from Tableau.Migration.Interop.Hooks.Transformers import ISyncContentTransformer
from Tableau.Migration.Content import (
    IGroup,
    IUser,
    IProject)
from Tableau.Migration.Engine import ContentMigrationItem
from System.Collections.Generic import List

class PyLogActionHook(ISyncMigrationHook[IMigrationActionResult], IMigrationActionCompletedHook):
    __namespace__ = "MyNamespace"
    
    def Execute(self, ctx):
        print("ACTION COMPLETED")
        return ctx

class PyTestGroupFilter(ISyncContentFilter[IGroup]):
    __namespace__ = "MyNamespace"
    
    def Execute(self, ctx):
        filtered = List[ContentMigrationItem[IGroup]]()
        for item in ctx:
            if "Test" not in item.SourceItem.Name:
                filtered.add(item)
        return filtered

class PyTestUsernameMapping(ISyncContentMapping[IUser]):
    __namespace__ = "MyNamespace"

    def Execute(self, ctx):
        domain = ctx.MappedLocation.Parent()
        return ctx.MapTo(domain.Append(ctx.ContentItem.Name + "@salesforce.com"))

class PyTestProjectTransformer(ISyncContentTransformer[IProject]):
    __namespace__ = "MyNamespace"
    
    def Execute(self, ctx):
        ctx.Description = "[From Server]\n" + ctx.Description
        return ctx
```

An example of Python plan builder hook binding for the above examples of object implementation:

```python
from System import Func, IServiceProvider
from Microsoft.Extensions.DependencyInjection import ServiceCollectionServiceExtensions

# For registering the service, you can use any of these options:
# - AddSingleton: The container will create a single instance of this class during all the application execution.
# - AddScoped: The container will create a new instance of this class for each Scope of the running application. Inside each scope, the instance will be like a Singleton.
# - AddTransient: The container will create a new instance of this class every time the application tries to use a given class.
ServiceCollectionServiceExtensions.AddSingleton[PyLogActionHook](tableau_migration._service_collection)
ServiceCollectionServiceExtensions.AddSingleton[PyTestGroupFilter](tableau_migration._service_collection)
# This case uses a lambda function to resolve a new instance of this class. This pattern could be used to resolve specific services from the DI container.
ServiceCollectionServiceExtensions.AddSingleton[PyTestUsernameMapping](tableau_migration._service_collection,Func[IServiceProvider, TestUserMapping](lambda service_provider: TestUserMapping()))
ServiceCollectionServiceExtensions.AddSingleton[PyTestProjectTransformer](tableau_migration._service_collection)

planBuilder = PyMigrationPlanBuilder()
# For hooks
planBuilder._hooks.add(PyLogActionHook)
# For filters
planBuilder._filters.add(PyTestGroupFilter, IGroup)
# For mappings
planBuilder._mappings.add(PyTestUsernameMapping, IUser)
# For transformers
planBuilder._transformers.add(PyTestProjectTransformer, IProject)

```
