# Make sure the test can find the module
import pytest
from tableau_migration.migration import get_service
from tableau_migration.migration_engine_hooks_mappings import PyContentMappingBuilder
from System import IServiceProvider, Func
from Microsoft.Extensions.DependencyInjection import (
    ServiceProviderServiceExtensions,
    ServiceCollectionServiceExtensions,
    ServiceCollection,
    ServiceCollectionContainerBuilderExtensions)
from Tableau.Migration.Content import IUser,IProject
from Tableau.Migration.Engine.Hooks import IMigrationHook
from Tableau.Migration.Engine.Hooks.Mappings import ContentMappingBuilder, IContentMapping, ContentMappingContext
from Tableau.Migration.Interop.Hooks.Mappings import ISyncContentMapping

class MappingImplementation(ISyncContentMapping[IUser]):
    __namespace__ = "Tableau.Migration.Custom.Hooks.Mappings.Lifetime"
    
    def Execute(self, ctx: ContentMappingContext[IUser]) -> ContentMappingContext[IUser]:
        return ctx

class SubMappingImplementation(MappingImplementation):
    __namespace__ = "Tableau.Migration.Custom.Hooks.Mappings.Lifetime"
    
    def Execute(self, ctx: ContentMappingContext[IUser]) -> ContentMappingContext[IUser]:
        return ctx

class TestContentMappingBuilderLifetimeTests():
    @pytest.fixture(autouse=True, scope="class")
    def setup(self):
        TestContentMappingBuilderLifetimeTests.MappingImplementation = MappingImplementation
        TestContentMappingBuilderLifetimeTests.SubMappingImplementation = SubMappingImplementation
            
        yield
        
        del TestContentMappingBuilderLifetimeTests.SubMappingImplementation
        del TestContentMappingBuilderLifetimeTests.MappingImplementation
        
    def test_lifetime_buildandcreate_object(self):
        try:
            # Arrange
            provider = ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(ServiceCollection())
            hook = TestContentMappingBuilderLifetimeTests.SubMappingImplementation()
            hookFactory = PyContentMappingBuilder(ContentMappingBuilder()).add(IUser,hook).build().get_hooks(IContentMapping[IUser])
        
            assert len(hookFactory) == 1

            # Act
            try:
                scope1 = ServiceProviderServiceExtensions.CreateScope(provider)
                firstScopeHook1 = hookFactory[0].Create[IContentMapping[IUser]](scope1.ServiceProvider)
                firstScopeHook2 = hookFactory[0].Create[IContentMapping[IUser]](scope1.ServiceProvider)
            finally:
                scope1.Dispose()

            try:
                scope2 = ServiceProviderServiceExtensions.CreateScope(provider)
                lastScopeHook = hookFactory[0].Create[IContentMapping[IUser]](scope2.ServiceProvider)
            finally:
                scope2.Dispose()

            # Assert
            assert hook == firstScopeHook1
            assert hook is not firstScopeHook1
            assert hook == firstScopeHook2
            assert hook is not firstScopeHook2
            assert hook == lastScopeHook
            assert hook is not lastScopeHook
        finally:
            provider.Dispose()

    def test_lifetime_buildandcreate_noinitializersingleton(self):
        try:
            # Arrange
            servicecollection = ServiceCollection()
            ServiceCollectionServiceExtensions.AddSingleton[TestContentMappingBuilderLifetimeTests.SubMappingImplementation](servicecollection)
            provider = ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(servicecollection)
            hookFactory = PyContentMappingBuilder(ContentMappingBuilder()).add(TestContentMappingBuilderLifetimeTests.SubMappingImplementation,IUser).build().get_hooks(IContentMapping[IUser])
        
            assert len(hookFactory) == 1

            # Act
            try:
                scope1 = ServiceProviderServiceExtensions.CreateScope(provider)
                firstScopeHook1 = hookFactory[0].Create[IContentMapping[IUser]](scope1.ServiceProvider)
                firstScopeHook2 = hookFactory[0].Create[IContentMapping[IUser]](scope1.ServiceProvider)
            finally:
                scope1.Dispose()

            try:
                scope2 = ServiceProviderServiceExtensions.CreateScope(provider)
                lastScopeHook = hookFactory[0].Create[IContentMapping[IUser]](scope2.ServiceProvider)
            finally:
                scope2.Dispose()

            # Assert
            assert firstScopeHook1 == firstScopeHook2
            assert firstScopeHook1 is not firstScopeHook2
            assert firstScopeHook1 == lastScopeHook
            assert firstScopeHook1 is not lastScopeHook
        finally:
            provider.Dispose()

    def test_lifetime_buildandcreate_noinitializerscoped(self):
        try:
            # Arrange
            servicecollection = ServiceCollection()
            ServiceCollectionServiceExtensions.AddScoped[TestContentMappingBuilderLifetimeTests.SubMappingImplementation](servicecollection)
            provider = ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(servicecollection)
            hookFactory = PyContentMappingBuilder(ContentMappingBuilder()).add(TestContentMappingBuilderLifetimeTests.SubMappingImplementation,IUser).build().get_hooks(IContentMapping[IUser])
        
            assert len(hookFactory) == 1

            # Act
            try:
                scope1 = ServiceProviderServiceExtensions.CreateScope(provider)
                firstScopeHook1 = hookFactory[0].Create[IContentMapping[IUser]](scope1.ServiceProvider)
                firstScopeHook2 = hookFactory[0].Create[IContentMapping[IUser]](scope1.ServiceProvider)
            finally:
                scope1.Dispose()

            try:
                scope2 = ServiceProviderServiceExtensions.CreateScope(provider)
                lastScopeHook = hookFactory[0].Create[IContentMapping[IUser]](scope2.ServiceProvider)
            finally:
                scope2.Dispose()

            # Assert
            assert firstScopeHook1 == firstScopeHook2
            assert firstScopeHook1 is not firstScopeHook2
            assert firstScopeHook1 != lastScopeHook
        finally:
            provider.Dispose()

    def test_lifetime_buildandcreate_noinitializertransient(self):
        try:
            # Arrange
            servicecollection = ServiceCollection()
            ServiceCollectionServiceExtensions.AddTransient[TestContentMappingBuilderLifetimeTests.SubMappingImplementation](servicecollection)
            provider = ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(servicecollection)
            hookFactory = PyContentMappingBuilder(ContentMappingBuilder()).add(TestContentMappingBuilderLifetimeTests.SubMappingImplementation,IUser).build().get_hooks(IContentMapping[IUser])
        
            assert len(hookFactory) == 1

            # Act
            try:
                scope1 = ServiceProviderServiceExtensions.CreateScope(provider)
                firstScopeHook1 = hookFactory[0].Create[IContentMapping[IUser]](scope1.ServiceProvider)
                firstScopeHook2 = hookFactory[0].Create[IContentMapping[IUser]](scope1.ServiceProvider)
            finally:
                scope1.Dispose()

            try:
                scope2 = ServiceProviderServiceExtensions.CreateScope(provider)
                lastScopeHook = hookFactory[0].Create[IContentMapping[IUser]](scope2.ServiceProvider)
            finally:
                scope2.Dispose()

            # Assert
            assert firstScopeHook1 != firstScopeHook2
            assert firstScopeHook1 != lastScopeHook
        finally:
            provider.Dispose()

    def test_lifetime_buildandcreate_initializerobjectreference(self):
        try:
            # Arrange
            hook = TestContentMappingBuilderLifetimeTests.SubMappingImplementation()
            def subsubclassinitialize(provider: IServiceProvider):
                return hook
            provider = ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(ServiceCollection())
            hookFactory = PyContentMappingBuilder(ContentMappingBuilder()).add(TestContentMappingBuilderLifetimeTests.SubMappingImplementation, IUser, Func[IServiceProvider, TestContentMappingBuilderLifetimeTests.SubMappingImplementation](subsubclassinitialize)).build().get_hooks(IContentMapping[IUser])
        
            assert len(hookFactory) == 1

            # Act
            try:
                scope1 = ServiceProviderServiceExtensions.CreateScope(provider)
                firstScopeHook1 = hookFactory[0].Create[IContentMapping[IUser]](scope1.ServiceProvider)
                firstScopeHook2 = hookFactory[0].Create[IContentMapping[IUser]](scope1.ServiceProvider)
            finally:
                scope1.Dispose()

            try:
                scope2 = ServiceProviderServiceExtensions.CreateScope(provider)
                lastScopeHook = hookFactory[0].Create[IContentMapping[IUser]](scope2.ServiceProvider)
            finally:
                scope2.Dispose()

            # Assert
            assert firstScopeHook1 == firstScopeHook2
            assert firstScopeHook1 is not firstScopeHook2
            assert firstScopeHook1 == lastScopeHook
            assert firstScopeHook1 is not lastScopeHook
        finally:
            provider.Dispose()

    def test_lifetime_buildandcreate_initializernewobject(self):
        try:
            # Arrange
            def subsubclassinitialize(provider: IServiceProvider):
                return TestContentMappingBuilderLifetimeTests.SubMappingImplementation()
            provider = ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(ServiceCollection())
            hookFactory = PyContentMappingBuilder(ContentMappingBuilder()).add(TestContentMappingBuilderLifetimeTests.SubMappingImplementation, IUser, Func[IServiceProvider, TestContentMappingBuilderLifetimeTests.SubMappingImplementation](subsubclassinitialize)).build().get_hooks(IContentMapping[IUser])
        
            assert len(hookFactory) == 1

            # Act
            try:
                scope1 = ServiceProviderServiceExtensions.CreateScope(provider)
                firstScopeHook1 = hookFactory[0].Create[IContentMapping[IUser]](scope1.ServiceProvider)
                firstScopeHook2 = hookFactory[0].Create[IContentMapping[IUser]](scope1.ServiceProvider)
            finally:
                scope1.Dispose()

            try:
                scope2 = ServiceProviderServiceExtensions.CreateScope(provider)
                lastScopeHook = hookFactory[0].Create[IContentMapping[IUser]](scope2.ServiceProvider)
            finally:
                scope2.Dispose()

            # Assert
            assert firstScopeHook1 != firstScopeHook2
            assert firstScopeHook1 != lastScopeHook
        finally:
            provider.Dispose()

    def test_lifetime_buildandcreate_initializersingleton(self):
        try:
            # Arrange
            def subsubclassinitialize(provider: IServiceProvider):
                return get_service(provider,TestContentMappingBuilderLifetimeTests.SubMappingImplementation)
            servicecollection = ServiceCollection()
            ServiceCollectionServiceExtensions.AddSingleton[TestContentMappingBuilderLifetimeTests.SubMappingImplementation](servicecollection)
            provider = ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(servicecollection)
            hookFactory = PyContentMappingBuilder(ContentMappingBuilder()).add(TestContentMappingBuilderLifetimeTests.SubMappingImplementation, IUser, Func[IServiceProvider, TestContentMappingBuilderLifetimeTests.SubMappingImplementation](subsubclassinitialize)).build().get_hooks(IContentMapping[IUser])
        
            assert len(hookFactory) == 1

            # Act
            try:
                scope1 = ServiceProviderServiceExtensions.CreateScope(provider)
                firstScopeHook1 = hookFactory[0].Create[IContentMapping[IUser]](scope1.ServiceProvider)
                firstScopeHook2 = hookFactory[0].Create[IContentMapping[IUser]](scope1.ServiceProvider)
            finally:
                scope1.Dispose()

            try:
                scope2 = ServiceProviderServiceExtensions.CreateScope(provider)
                lastScopeHook = hookFactory[0].Create[IContentMapping[IUser]](scope2.ServiceProvider)
            finally:
                scope2.Dispose()

            # Assert
            assert firstScopeHook1 == firstScopeHook2
            assert firstScopeHook1 is not firstScopeHook2
            assert firstScopeHook1 == lastScopeHook
            assert firstScopeHook1 is not lastScopeHook
        finally:
            provider.Dispose()

    def test_lifetime_buildandcreate_initializerscoped(self):
        try:
            # Arrange
            def classinitialize(provider: IServiceProvider):
                return get_service(provider,TestContentMappingBuilderLifetimeTests.MappingImplementation)
            servicecollection = ServiceCollection()
            ServiceCollectionServiceExtensions.AddScoped[TestContentMappingBuilderLifetimeTests.MappingImplementation](servicecollection)
            provider = ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(servicecollection)
            hookFactory = PyContentMappingBuilder(ContentMappingBuilder()).add(TestContentMappingBuilderLifetimeTests.MappingImplementation, IUser, Func[IServiceProvider, TestContentMappingBuilderLifetimeTests.MappingImplementation](classinitialize)).build().get_hooks(IContentMapping[IUser])
        
            assert len(hookFactory) == 1

            # Act
            try:
                scope1 = ServiceProviderServiceExtensions.CreateScope(provider)
                firstScopeHook1 = hookFactory[0].Create[IContentMapping[IUser]](scope1.ServiceProvider)
                firstScopeHook2 = hookFactory[0].Create[IContentMapping[IUser]](scope1.ServiceProvider)
            finally:
                scope1.Dispose()

            try:
                scope2 = ServiceProviderServiceExtensions.CreateScope(provider)
                lastScopeHook = hookFactory[0].Create[IContentMapping[IUser]](scope2.ServiceProvider)
            finally:
                scope2.Dispose()

            # Assert
            assert firstScopeHook1 == firstScopeHook2
            assert firstScopeHook1 is not firstScopeHook2
            assert firstScopeHook1 != lastScopeHook
        finally:
            provider.Dispose()

    def test_lifetime_buildandcreate_initializertransient(self):
        try:
            # Arrange
            def subclassinitialize(provider: IServiceProvider):
                return get_service(provider,TestContentMappingBuilderLifetimeTests.SubMappingImplementation)
            servicecollection = ServiceCollection()
            ServiceCollectionServiceExtensions.AddTransient[TestContentMappingBuilderLifetimeTests.SubMappingImplementation](servicecollection)
            provider = ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(servicecollection)
            hookFactory = PyContentMappingBuilder(ContentMappingBuilder()).add(TestContentMappingBuilderLifetimeTests.SubMappingImplementation, IUser, Func[IServiceProvider, TestContentMappingBuilderLifetimeTests.SubMappingImplementation](subclassinitialize)).build().get_hooks(IContentMapping[IUser])
        
            assert len(hookFactory) == 1

            # Act
            try:
                scope1 = ServiceProviderServiceExtensions.CreateScope(provider)
                firstScopeHook1 = hookFactory[0].Create[IContentMapping[IUser]](scope1.ServiceProvider)
                firstScopeHook2 = hookFactory[0].Create[IContentMapping[IUser]](scope1.ServiceProvider)
            finally:
                scope1.Dispose()

            try:
                scope2 = ServiceProviderServiceExtensions.CreateScope(provider)
                lastScopeHook = hookFactory[0].Create[IContentMapping[IUser]](scope2.ServiceProvider)
            finally:
                scope2.Dispose()

            # Assert
            assert firstScopeHook1 != firstScopeHook2
            assert firstScopeHook1 != lastScopeHook
        finally:
            provider.Dispose()

    def test_lifetime_buildandcreate_callback(self):
        try:
            # Arrange
            def classcallback(context: ContentMappingContext[IProject]):
                return context
        
            provider = ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(ServiceCollection())
            hookFactory = PyContentMappingBuilder(ContentMappingBuilder()).add(IProject, Func[ContentMappingContext[IProject], ContentMappingContext[IProject]](classcallback)).build().get_hooks(IContentMapping[IProject])
        
            assert len(hookFactory) == 1

            # Act
            try:
                scope1 = ServiceProviderServiceExtensions.CreateScope(provider)
                firstScopeHook1 = hookFactory[0].Create[IMigrationHook[ContentMappingContext[IProject]]](scope1.ServiceProvider)
                firstScopeHook2 = hookFactory[0].Create[IMigrationHook[ContentMappingContext[IProject]]](scope1.ServiceProvider)
            finally:
                scope1.Dispose()

            try:
                scope2 = ServiceProviderServiceExtensions.CreateScope(provider)
                lastScopeHook = hookFactory[0].Create[IMigrationHook[ContentMappingContext[IProject]]](scope2.ServiceProvider)
            finally:
                scope2.Dispose()

            # Assert
            assert firstScopeHook1 != firstScopeHook2
            assert firstScopeHook1 != lastScopeHook
        finally:
            provider.Dispose()
