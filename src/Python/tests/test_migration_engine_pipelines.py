# region _generated

from tableau_migration.migration import PyPipelineProfile # noqa: E402, F401
from typing import Sequence # noqa: E402, F401
from typing_extensions import Self # noqa: E402, F401

import System # noqa: E402

from Tableau.Migration.Engine.Pipelines import (  # noqa: E402, F401
    MigrationPipelineContentType,
    ServerToCloudMigrationPipeline
)

from tableau_migration.migration_engine_pipelines import (  # noqa: E402, F401
    PyMigrationPipelineContentType,
    PyServerToCloudMigrationPipeline
)


# Extra imports for tests.
from tests.helpers.autofixture import AutoFixtureTestBase # noqa: E402, F401

class TestPyMigrationPipelineContentTypeGenerated(AutoFixtureTestBase):
    
    def test_ctor(self):
        dotnet = self.create(MigrationPipelineContentType)
        py = PyMigrationPipelineContentType(dotnet)
        assert py._dotnet == dotnet
    
    def test_users_getter(self):
        dotnet = self.create(MigrationPipelineContentType)
        py = PyMigrationPipelineContentType(dotnet)
        assert py.get_users() == None if MigrationPipelineContentType.Users is None else PyMigrationPipelineContentType(MigrationPipelineContentType.Users)
    
    def test_groups_getter(self):
        dotnet = self.create(MigrationPipelineContentType)
        py = PyMigrationPipelineContentType(dotnet)
        assert py.get_groups() == None if MigrationPipelineContentType.Groups is None else PyMigrationPipelineContentType(MigrationPipelineContentType.Groups)
    
    def test_group_sets_getter(self):
        dotnet = self.create(MigrationPipelineContentType)
        py = PyMigrationPipelineContentType(dotnet)
        assert py.get_group_sets() == None if MigrationPipelineContentType.GroupSets is None else PyMigrationPipelineContentType(MigrationPipelineContentType.GroupSets)
    
    def test_projects_getter(self):
        dotnet = self.create(MigrationPipelineContentType)
        py = PyMigrationPipelineContentType(dotnet)
        assert py.get_projects() == None if MigrationPipelineContentType.Projects is None else PyMigrationPipelineContentType(MigrationPipelineContentType.Projects)
    
    def test_data_sources_getter(self):
        dotnet = self.create(MigrationPipelineContentType)
        py = PyMigrationPipelineContentType(dotnet)
        assert py.get_data_sources() == None if MigrationPipelineContentType.DataSources is None else PyMigrationPipelineContentType(MigrationPipelineContentType.DataSources)
    
    def test_workbooks_getter(self):
        dotnet = self.create(MigrationPipelineContentType)
        py = PyMigrationPipelineContentType(dotnet)
        assert py.get_workbooks() == None if MigrationPipelineContentType.Workbooks is None else PyMigrationPipelineContentType(MigrationPipelineContentType.Workbooks)
    
    def test_views_getter(self):
        dotnet = self.create(MigrationPipelineContentType)
        py = PyMigrationPipelineContentType(dotnet)
        assert py.get_views() == None if MigrationPipelineContentType.Views is None else PyMigrationPipelineContentType(MigrationPipelineContentType.Views)
    
    def test_server_to_server_extract_refresh_tasks_getter(self):
        dotnet = self.create(MigrationPipelineContentType)
        py = PyMigrationPipelineContentType(dotnet)
        assert py.get_server_to_server_extract_refresh_tasks() == None if MigrationPipelineContentType.ServerToServerExtractRefreshTasks is None else PyMigrationPipelineContentType(MigrationPipelineContentType.ServerToServerExtractRefreshTasks)
    
    def test_server_to_cloud_extract_refresh_tasks_getter(self):
        dotnet = self.create(MigrationPipelineContentType)
        py = PyMigrationPipelineContentType(dotnet)
        assert py.get_server_to_cloud_extract_refresh_tasks() == None if MigrationPipelineContentType.ServerToCloudExtractRefreshTasks is None else PyMigrationPipelineContentType(MigrationPipelineContentType.ServerToCloudExtractRefreshTasks)
    
    def test_cloud_to_cloud_extract_refresh_tasks_getter(self):
        dotnet = self.create(MigrationPipelineContentType)
        py = PyMigrationPipelineContentType(dotnet)
        assert py.get_cloud_to_cloud_extract_refresh_tasks() == None if MigrationPipelineContentType.CloudToCloudExtractRefreshTasks is None else PyMigrationPipelineContentType(MigrationPipelineContentType.CloudToCloudExtractRefreshTasks)
    
    def test_custom_views_getter(self):
        dotnet = self.create(MigrationPipelineContentType)
        py = PyMigrationPipelineContentType(dotnet)
        assert py.get_custom_views() == None if MigrationPipelineContentType.CustomViews is None else PyMigrationPipelineContentType(MigrationPipelineContentType.CustomViews)
    
    def test_server_to_server_subscriptions_getter(self):
        dotnet = self.create(MigrationPipelineContentType)
        py = PyMigrationPipelineContentType(dotnet)
        assert py.get_server_to_server_subscriptions() == None if MigrationPipelineContentType.ServerToServerSubscriptions is None else PyMigrationPipelineContentType(MigrationPipelineContentType.ServerToServerSubscriptions)
    
    def test_server_to_cloud_subscriptions_getter(self):
        dotnet = self.create(MigrationPipelineContentType)
        py = PyMigrationPipelineContentType(dotnet)
        assert py.get_server_to_cloud_subscriptions() == None if MigrationPipelineContentType.ServerToCloudSubscriptions is None else PyMigrationPipelineContentType(MigrationPipelineContentType.ServerToCloudSubscriptions)
    
    def test_cloud_to_cloud_subscriptions_getter(self):
        dotnet = self.create(MigrationPipelineContentType)
        py = PyMigrationPipelineContentType(dotnet)
        assert py.get_cloud_to_cloud_subscriptions() == None if MigrationPipelineContentType.CloudToCloudSubscriptions is None else PyMigrationPipelineContentType(MigrationPipelineContentType.CloudToCloudSubscriptions)
    
    def test_favorites_getter(self):
        dotnet = self.create(MigrationPipelineContentType)
        py = PyMigrationPipelineContentType(dotnet)
        assert py.get_favorites() == None if MigrationPipelineContentType.Favorites is None else PyMigrationPipelineContentType(MigrationPipelineContentType.Favorites)
    
    def test_content_type_getter(self):
        dotnet = self.create(MigrationPipelineContentType)
        py = PyMigrationPipelineContentType(dotnet)
        assert py.content_type == dotnet.ContentType
    
    def test_prepare_type_getter(self):
        dotnet = self.create(MigrationPipelineContentType)
        py = PyMigrationPipelineContentType(dotnet)
        assert py.prepare_type == dotnet.PrepareType
    
    def test_publish_type_getter(self):
        dotnet = self.create(MigrationPipelineContentType)
        py = PyMigrationPipelineContentType(dotnet)
        assert py.publish_type == dotnet.PublishType
    
    def test_result_type_getter(self):
        dotnet = self.create(MigrationPipelineContentType)
        py = PyMigrationPipelineContentType(dotnet)
        assert py.result_type == dotnet.ResultType
    
    def test_types_getter(self):
        dotnet = self.create(MigrationPipelineContentType)
        py = PyMigrationPipelineContentType(dotnet)
        assert len(dotnet.Types) != 0
        assert len(py.types) == len(dotnet.Types)
    
class TestPyServerToCloudMigrationPipelineGenerated(AutoFixtureTestBase):
    
    def test_ctor(self):
        dotnet = self.create(ServerToCloudMigrationPipeline)
        py = PyServerToCloudMigrationPipeline(dotnet)
        assert py._dotnet == dotnet
    
    def test_content_types_getter(self):
        dotnet = self.create(ServerToCloudMigrationPipeline)
        py = PyServerToCloudMigrationPipeline(dotnet)
        assert len(ServerToCloudMigrationPipeline.ContentTypes) != 0
        assert len(py.get_content_types()) == len(ServerToCloudMigrationPipeline.ContentTypes)
    

# endregion

