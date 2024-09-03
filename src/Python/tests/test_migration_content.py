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

from typing import List
import pytest

from tableau_migration.migration_content import PyConnection, PyPublishableWorkbook, PyTag, PyUsernameContent, PyWithTags
from uuid import uuid4

from Tableau.Migration.Content import IConnection, IPublishableWorkbook, IUsernameContent, IWithTags, ITag
import Moq

from tests.helpers.autofixture import AutoFixtureTestBase
from System import String

class TestPyUsernameContent():
    def test_init(self):
        mock = Moq.Mock[IUsernameContent]()
        py = PyUsernameContent(mock.Object)

        d = py.domain

class TestPyConnection():
    def test_query_tagging_enabled(self):
        mock = Moq.Mock[IConnection]()
        py = PyConnection(mock.Object)

        assert py.query_tagging_enabled is None


class TestPyPublishableWorkbook(AutoFixtureTestBase):
    def test_thumbnails_user_id(self):
        mock = Moq.Mock[IPublishableWorkbook]()
        py = PyPublishableWorkbook(mock.Object)

        new_id = uuid4()
        py.thumbnails_user_id = new_id

    def test_hidden_view_names(self):
        dotnet = self.create(IPublishableWorkbook)
        py = PyPublishableWorkbook(dotnet)

        assert len(dotnet.HiddenViewNames) != 0
        assert len(py.hidden_view_names) == len(dotnet.HiddenViewNames)
        assert set(py.hidden_view_names) == set(dotnet.HiddenViewNames)

    def test_hidden_view_names_set(self):
        dotnet = self.create(IPublishableWorkbook)
        py = PyPublishableWorkbook(dotnet)
        
        assert len(dotnet.HiddenViewNames) != 0
        assert len(py.hidden_view_names) == len(dotnet.HiddenViewNames)
        
        # create test data        
        testValue1 = "Hidden View 1"
        testValue2 = "Hidden View 2"
        testCollection = [testValue1,testValue2]
        
        # set property to new test value
        py.hidden_view_names = testCollection
        # assert value
        assert len(py.hidden_view_names) == len(testCollection)    
        assert set(py.hidden_view_names) == set(testCollection)

class TestPyWithTags(AutoFixtureTestBase):

    def test_ctor(self):
        dotnet = self.create(IWithTags)
        py = PyWithTags(dotnet)

        assert len(dotnet.Tags) != 0
        assert len(py.tags) == len(dotnet.Tags)

        # Compare labels
        dotnet_labels = [dotnet_tag.Label for dotnet_tag in dotnet.Tags]
        py_labels = [py_tag.label for py_tag in py.tags]
        assert set(py_labels) == set(dotnet_labels)

    def test_setter(self):
        dotnet = self.create(IWithTags)
        py = PyWithTags(dotnet)

        assert len(dotnet.Tags) != 0
        assert len(py.tags) == len(dotnet.Tags)

        tag1 = PyTag(self.create(ITag))
        tag2 = PyTag(self.create(ITag))
        tags_collection = [tag1, tag2]

        assert len(tags_collection) == 2

        py.tags = tags_collection

        assert len(py.tags) == len(tags_collection)
        
    def test_setter_empty(self):
        dotnet = self.create(IWithTags)
        py = PyWithTags(dotnet)

        assert len(dotnet.Tags) != 0
        assert len(py.tags) == len(dotnet.Tags)

        tags_collection = None

        assert tags_collection is None

        py.tags = tags_collection

        assert len(py.tags) == 0
# region _generated

from tableau_migration.migration import PyContentReference # noqa: E402, F401
from tableau_migration.migration_api_rest import PyRestIdentifiable # noqa: E402, F401
from typing import (  # noqa: E402, F401
    Sequence,
    List
)
from uuid import UUID # noqa: E402, F401

from System import (  # noqa: E402, F401
    Guid,
    String as String
)
from System.Collections.Generic import (  # noqa: E402, F401
    List as DotnetList,
    HashSet as DotnetHashSet
)
from Tableau.Migration.Content import (  # noqa: E402, F401
    IConnection,
    IConnectionsContent,
    IContainerContent,
    ICustomView,
    IDataSource,
    IDataSourceDetails,
    IDescriptionContent,
    IExtractContent,
    IGroup,
    IGroupUser,
    ILabel,
    IProject,
    IPublishableCustomView,
    IPublishableDataSource,
    IPublishableGroup,
    IPublishableWorkbook,
    IPublishedContent,
    ITag,
    IUser,
    IUsernameContent,
    IView,
    IWithDomain,
    IWithOwner,
    IWithTags,
    IWithWorkbook,
    IWorkbook,
    IWorkbookDetails
)

from tableau_migration.migration_content import (  # noqa: E402, F401
    PyConnection,
    PyConnectionsContent,
    PyContainerContent,
    PyCustomView,
    PyDataSource,
    PyDataSourceDetails,
    PyDescriptionContent,
    PyExtractContent,
    PyGroup,
    PyGroupUser,
    PyLabel,
    PyProject,
    PyPublishableCustomView,
    PyPublishableDataSource,
    PyPublishableGroup,
    PyPublishableWorkbook,
    PyPublishedContent,
    PyTag,
    PyUser,
    PyUsernameContent,
    PyView,
    PyWithDomain,
    PyWithOwner,
    PyWithTags,
    PyWithWorkbook,
    PyWorkbook,
    PyWorkbookDetails
)


# Extra imports for tests.
from Tableau.Migration import IContentReference # noqa: E402, F401
from System import (  # noqa: E402, F401
    Boolean,
    Nullable
)
from tests.helpers.autofixture import AutoFixtureTestBase # noqa: E402, F401

class TestPyConnectionGenerated(AutoFixtureTestBase):
    
    def test_ctor(self):
        dotnet = self.create(IConnection)
        py = PyConnection(dotnet)
        assert py._dotnet == dotnet
    
    def test_id_getter(self):
        dotnet = self.create(IConnection)
        py = PyConnection(dotnet)
        assert py.id == None if dotnet.Id is None else UUID(dotnet.Id.ToString())
    
    def test_type_getter(self):
        dotnet = self.create(IConnection)
        py = PyConnection(dotnet)
        assert py.type == dotnet.Type
    
    def test_server_address_getter(self):
        dotnet = self.create(IConnection)
        py = PyConnection(dotnet)
        assert py.server_address == dotnet.ServerAddress
    
    def test_server_port_getter(self):
        dotnet = self.create(IConnection)
        py = PyConnection(dotnet)
        assert py.server_port == dotnet.ServerPort
    
    def test_connection_username_getter(self):
        dotnet = self.create(IConnection)
        py = PyConnection(dotnet)
        assert py.connection_username == dotnet.ConnectionUsername
    
    def test_query_tagging_enabled_getter(self):
        dotnet = self.create(IConnection)
        py = PyConnection(dotnet)
        assert py.query_tagging_enabled == dotnet.QueryTaggingEnabled
    
class TestPyConnectionsContentGenerated(AutoFixtureTestBase):
    
    def test_ctor(self):
        dotnet = self.create(IConnectionsContent)
        py = PyConnectionsContent(dotnet)
        assert py._dotnet == dotnet
    
    def test_connections_getter(self):
        dotnet = self.create(IConnectionsContent)
        py = PyConnectionsContent(dotnet)
        assert len(dotnet.Connections) != 0
        assert len(py.connections) == len(dotnet.Connections)
    
class TestPyContainerContentGenerated(AutoFixtureTestBase):
    
    def test_ctor(self):
        dotnet = self.create(IContainerContent)
        py = PyContainerContent(dotnet)
        assert py._dotnet == dotnet
    
    def test_container_getter(self):
        dotnet = self.create(IContainerContent)
        py = PyContainerContent(dotnet)
        assert py.container == None if dotnet.Container is None else PyContentReference(dotnet.Container)
    
class TestPyCustomViewGenerated(AutoFixtureTestBase):
    
    def test_ctor(self):
        dotnet = self.create(ICustomView)
        py = PyCustomView(dotnet)
        assert py._dotnet == dotnet
    
    def test_created_at_getter(self):
        dotnet = self.create(ICustomView)
        py = PyCustomView(dotnet)
        assert py.created_at == dotnet.CreatedAt
    
    def test_updated_at_getter(self):
        dotnet = self.create(ICustomView)
        py = PyCustomView(dotnet)
        assert py.updated_at == dotnet.UpdatedAt
    
    def test_last_accessed_at_getter(self):
        dotnet = self.create(ICustomView)
        py = PyCustomView(dotnet)
        assert py.last_accessed_at == dotnet.LastAccessedAt
    
    def test_shared_getter(self):
        dotnet = self.create(ICustomView)
        py = PyCustomView(dotnet)
        assert py.shared == dotnet.Shared
    
    def test_shared_setter(self):
        dotnet = self.create(ICustomView)
        py = PyCustomView(dotnet)
        
        # create test data
        testValue = self.create(Boolean)
        
        # set property to new test value
        py.shared = testValue
        
        # assert value
        assert py.shared == testValue
    
    def test_base_view_id_getter(self):
        dotnet = self.create(ICustomView)
        py = PyCustomView(dotnet)
        assert py.base_view_id == None if dotnet.BaseViewId is None else UUID(dotnet.BaseViewId.ToString())
    
    def test_base_view_name_getter(self):
        dotnet = self.create(ICustomView)
        py = PyCustomView(dotnet)
        assert py.base_view_name == dotnet.BaseViewName
    
class TestPyDataSourceGenerated(AutoFixtureTestBase):
    
    def test_ctor(self):
        dotnet = self.create(IDataSource)
        py = PyDataSource(dotnet)
        assert py._dotnet == dotnet
    
    def test_has_extracts_getter(self):
        dotnet = self.create(IDataSource)
        py = PyDataSource(dotnet)
        assert py.has_extracts == dotnet.HasExtracts
    
    def test_is_certified_getter(self):
        dotnet = self.create(IDataSource)
        py = PyDataSource(dotnet)
        assert py.is_certified == dotnet.IsCertified
    
    def test_use_remote_query_agent_getter(self):
        dotnet = self.create(IDataSource)
        py = PyDataSource(dotnet)
        assert py.use_remote_query_agent == dotnet.UseRemoteQueryAgent
    
    def test_use_remote_query_agent_setter(self):
        dotnet = self.create(IDataSource)
        py = PyDataSource(dotnet)
        
        # create test data
        testValue = self.create(Boolean)
        
        # set property to new test value
        py.use_remote_query_agent = testValue
        
        # assert value
        assert py.use_remote_query_agent == testValue
    
class TestPyDataSourceDetailsGenerated(AutoFixtureTestBase):
    
    def test_ctor(self):
        dotnet = self.create(IDataSourceDetails)
        py = PyDataSourceDetails(dotnet)
        assert py._dotnet == dotnet
    
    def test_certification_note_getter(self):
        dotnet = self.create(IDataSourceDetails)
        py = PyDataSourceDetails(dotnet)
        assert py.certification_note == dotnet.CertificationNote
    
class TestPyDescriptionContentGenerated(AutoFixtureTestBase):
    
    def test_ctor(self):
        dotnet = self.create(IDescriptionContent)
        py = PyDescriptionContent(dotnet)
        assert py._dotnet == dotnet
    
    def test_description_getter(self):
        dotnet = self.create(IDescriptionContent)
        py = PyDescriptionContent(dotnet)
        assert py.description == dotnet.Description
    
    def test_description_setter(self):
        dotnet = self.create(IDescriptionContent)
        py = PyDescriptionContent(dotnet)
        
        # create test data
        testValue = self.create(String)
        
        # set property to new test value
        py.description = testValue
        
        # assert value
        assert py.description == testValue
    
class TestPyExtractContentGenerated(AutoFixtureTestBase):
    
    def test_ctor(self):
        dotnet = self.create(IExtractContent)
        py = PyExtractContent(dotnet)
        assert py._dotnet == dotnet
    
    def test_encrypt_extracts_getter(self):
        dotnet = self.create(IExtractContent)
        py = PyExtractContent(dotnet)
        assert py.encrypt_extracts == dotnet.EncryptExtracts
    
    def test_encrypt_extracts_setter(self):
        dotnet = self.create(IExtractContent)
        py = PyExtractContent(dotnet)
        
        # create test data
        testValue = self.create(Boolean)
        
        # set property to new test value
        py.encrypt_extracts = testValue
        
        # assert value
        assert py.encrypt_extracts == testValue
    
class TestPyGroupGenerated(AutoFixtureTestBase):
    
    def test_ctor(self):
        dotnet = self.create(IGroup)
        py = PyGroup(dotnet)
        assert py._dotnet == dotnet
    
    def test_grant_license_mode_getter(self):
        dotnet = self.create(IGroup)
        py = PyGroup(dotnet)
        assert py.grant_license_mode == dotnet.GrantLicenseMode
    
    def test_grant_license_mode_setter(self):
        dotnet = self.create(IGroup)
        py = PyGroup(dotnet)
        
        # create test data
        testValue = self.create(String)
        
        # set property to new test value
        py.grant_license_mode = testValue
        
        # assert value
        assert py.grant_license_mode == testValue
    
    def test_site_role_getter(self):
        dotnet = self.create(IGroup)
        py = PyGroup(dotnet)
        assert py.site_role == dotnet.SiteRole
    
    def test_site_role_setter(self):
        dotnet = self.create(IGroup)
        py = PyGroup(dotnet)
        
        # create test data
        testValue = self.create(String)
        
        # set property to new test value
        py.site_role = testValue
        
        # assert value
        assert py.site_role == testValue
    
class TestPyGroupUserGenerated(AutoFixtureTestBase):
    
    def test_ctor(self):
        dotnet = self.create(IGroupUser)
        py = PyGroupUser(dotnet)
        assert py._dotnet == dotnet
    
    def test_user_getter(self):
        dotnet = self.create(IGroupUser)
        py = PyGroupUser(dotnet)
        assert py.user == None if dotnet.User is None else PyContentReference(dotnet.User)
    
    def test_user_setter(self):
        dotnet = self.create(IGroupUser)
        py = PyGroupUser(dotnet)
        
        # create test data
        testValue = self.create(IContentReference)
        
        # set property to new test value
        py.user = None if testValue is None else PyContentReference(testValue)
        
        # assert value
        assert py.user == None if testValue is None else PyContentReference(testValue)
    
class TestPyLabelGenerated(AutoFixtureTestBase):
    
    def test_ctor(self):
        dotnet = self.create(ILabel)
        py = PyLabel(dotnet)
        assert py._dotnet == dotnet
    
    def test_site_id_getter(self):
        dotnet = self.create(ILabel)
        py = PyLabel(dotnet)
        assert py.site_id == None if dotnet.SiteId is None else UUID(dotnet.SiteId.ToString())
    
    def test_owner_id_getter(self):
        dotnet = self.create(ILabel)
        py = PyLabel(dotnet)
        assert py.owner_id == None if dotnet.OwnerId is None else UUID(dotnet.OwnerId.ToString())
    
    def test_user_display_name_getter(self):
        dotnet = self.create(ILabel)
        py = PyLabel(dotnet)
        assert py.user_display_name == dotnet.UserDisplayName
    
    def test_content_id_getter(self):
        dotnet = self.create(ILabel)
        py = PyLabel(dotnet)
        assert py.content_id == None if dotnet.ContentId is None else UUID(dotnet.ContentId.ToString())
    
    def test_content_type_getter(self):
        dotnet = self.create(ILabel)
        py = PyLabel(dotnet)
        assert py.content_type == dotnet.ContentType
    
    def test_message_getter(self):
        dotnet = self.create(ILabel)
        py = PyLabel(dotnet)
        assert py.message == dotnet.Message
    
    def test_value_getter(self):
        dotnet = self.create(ILabel)
        py = PyLabel(dotnet)
        assert py.value == dotnet.Value
    
    def test_category_getter(self):
        dotnet = self.create(ILabel)
        py = PyLabel(dotnet)
        assert py.category == dotnet.Category
    
    def test_active_getter(self):
        dotnet = self.create(ILabel)
        py = PyLabel(dotnet)
        assert py.active == dotnet.Active
    
    def test_elevated_getter(self):
        dotnet = self.create(ILabel)
        py = PyLabel(dotnet)
        assert py.elevated == dotnet.Elevated
    
    def test_created_at_getter(self):
        dotnet = self.create(ILabel)
        py = PyLabel(dotnet)
        assert py.created_at == dotnet.CreatedAt
    
    def test_updated_at_getter(self):
        dotnet = self.create(ILabel)
        py = PyLabel(dotnet)
        assert py.updated_at == dotnet.UpdatedAt
    
class TestPyProjectGenerated(AutoFixtureTestBase):
    
    def test_ctor(self):
        dotnet = self.create(IProject)
        py = PyProject(dotnet)
        assert py._dotnet == dotnet
    
    def test_content_permissions_getter(self):
        dotnet = self.create(IProject)
        py = PyProject(dotnet)
        assert py.content_permissions == dotnet.ContentPermissions
    
    def test_content_permissions_setter(self):
        dotnet = self.create(IProject)
        py = PyProject(dotnet)
        
        # create test data
        testValue = self.create(String)
        
        # set property to new test value
        py.content_permissions = testValue
        
        # assert value
        assert py.content_permissions == testValue
    
    def test_parent_project_getter(self):
        dotnet = self.create(IProject)
        py = PyProject(dotnet)
        assert py.parent_project == None if dotnet.ParentProject is None else PyContentReference(dotnet.ParentProject)
    
class TestPyPublishableCustomViewGenerated(AutoFixtureTestBase):
    
    def test_ctor(self):
        dotnet = self.create(IPublishableCustomView)
        py = PyPublishableCustomView(dotnet)
        assert py._dotnet == dotnet
    
    def test_default_users_getter(self):
        dotnet = self.create(IPublishableCustomView)
        py = PyPublishableCustomView(dotnet)
        assert len(dotnet.DefaultUsers) != 0
        assert len(py.default_users) == len(dotnet.DefaultUsers)
    
    def test_default_users_setter(self):
        dotnet = self.create(IPublishableCustomView)
        py = PyPublishableCustomView(dotnet)
        assert len(dotnet.DefaultUsers) != 0
        assert len(py.default_users) == len(dotnet.DefaultUsers)
        
        # create test data
        dotnetCollection = DotnetList[IContentReference]()
        dotnetCollection.Add(self.create(IContentReference))
        dotnetCollection.Add(self.create(IContentReference))
        testCollection = [] if dotnetCollection is None else [PyContentReference(x) for x in dotnetCollection if x is not None]
        
        # set property to new test value
        py.default_users = testCollection
        
        # assert value
        assert len(py.default_users) == len(testCollection)
    
class TestPyPublishableGroupGenerated(AutoFixtureTestBase):
    
    def test_ctor(self):
        dotnet = self.create(IPublishableGroup)
        py = PyPublishableGroup(dotnet)
        assert py._dotnet == dotnet
    
    def test_users_getter(self):
        dotnet = self.create(IPublishableGroup)
        py = PyPublishableGroup(dotnet)
        assert len(dotnet.Users) != 0
        assert len(py.users) == len(dotnet.Users)
    
    def test_users_setter(self):
        dotnet = self.create(IPublishableGroup)
        py = PyPublishableGroup(dotnet)
        assert len(dotnet.Users) != 0
        assert len(py.users) == len(dotnet.Users)
        
        # create test data
        dotnetCollection = DotnetList[IGroupUser]()
        dotnetCollection.Add(self.create(IGroupUser))
        dotnetCollection.Add(self.create(IGroupUser))
        testCollection = [] if dotnetCollection is None else [PyGroupUser(x) for x in dotnetCollection if x is not None]
        
        # set property to new test value
        py.users = testCollection
        
        # assert value
        assert len(py.users) == len(testCollection)
    
class TestPyPublishableWorkbookGenerated(AutoFixtureTestBase):
    
    def test_ctor(self):
        dotnet = self.create(IPublishableWorkbook)
        py = PyPublishableWorkbook(dotnet)
        assert py._dotnet == dotnet
    
    def test_thumbnails_user_id_getter(self):
        dotnet = self.create(IPublishableWorkbook)
        py = PyPublishableWorkbook(dotnet)
        assert py.thumbnails_user_id == None if dotnet.ThumbnailsUserId is None else UUID(dotnet.ThumbnailsUserId.ToString())
    
    def test_thumbnails_user_id_setter(self):
        dotnet = self.create(IPublishableWorkbook)
        py = PyPublishableWorkbook(dotnet)
        
        # create test data
        testValue = self.create(Nullable[Guid])
        
        # set property to new test value
        py.thumbnails_user_id = None if testValue is None else UUID(testValue.ToString())
        
        # assert value
        assert py.thumbnails_user_id == None if testValue is None else UUID(testValue.ToString())
    
    def test_hidden_view_names_getter(self):
        dotnet = self.create(IPublishableWorkbook)
        py = PyPublishableWorkbook(dotnet)
        assert len(dotnet.HiddenViewNames) != 0
        assert len(py.hidden_view_names) == len(dotnet.HiddenViewNames)
    
    def test_hidden_view_names_setter(self):
        dotnet = self.create(IPublishableWorkbook)
        py = PyPublishableWorkbook(dotnet)
        assert len(dotnet.HiddenViewNames) != 0
        assert len(py.hidden_view_names) == len(dotnet.HiddenViewNames)
        
        # create test data
        dotnetCollection = DotnetList[String]()
        dotnetCollection.Add(self.create(String))
        dotnetCollection.Add(self.create(String))
        testCollection = [] if dotnetCollection is None else [x for x in dotnetCollection if x is not None]
        
        # set property to new test value
        py.hidden_view_names = testCollection
        
        # assert value
        assert len(py.hidden_view_names) == len(testCollection)
    
class TestPyPublishedContentGenerated(AutoFixtureTestBase):
    
    def test_ctor(self):
        dotnet = self.create(IPublishedContent)
        py = PyPublishedContent(dotnet)
        assert py._dotnet == dotnet
    
    def test_created_at_getter(self):
        dotnet = self.create(IPublishedContent)
        py = PyPublishedContent(dotnet)
        assert py.created_at == dotnet.CreatedAt
    
    def test_updated_at_getter(self):
        dotnet = self.create(IPublishedContent)
        py = PyPublishedContent(dotnet)
        assert py.updated_at == dotnet.UpdatedAt
    
    def test_webpage_url_getter(self):
        dotnet = self.create(IPublishedContent)
        py = PyPublishedContent(dotnet)
        assert py.webpage_url == dotnet.WebpageUrl
    
class TestPyTagGenerated(AutoFixtureTestBase):
    
    def test_ctor(self):
        dotnet = self.create(ITag)
        py = PyTag(dotnet)
        assert py._dotnet == dotnet
    
    def test_label_getter(self):
        dotnet = self.create(ITag)
        py = PyTag(dotnet)
        assert py.label == dotnet.Label
    
    def test_label_setter(self):
        dotnet = self.create(ITag)
        py = PyTag(dotnet)
        
        # create test data
        testValue = self.create(String)
        
        # set property to new test value
        py.label = testValue
        
        # assert value
        assert py.label == testValue
    
class TestPyUserGenerated(AutoFixtureTestBase):
    
    def test_ctor(self):
        dotnet = self.create(IUser)
        py = PyUser(dotnet)
        assert py._dotnet == dotnet
    
    def test_full_name_getter(self):
        dotnet = self.create(IUser)
        py = PyUser(dotnet)
        assert py.full_name == dotnet.FullName
    
    def test_full_name_setter(self):
        dotnet = self.create(IUser)
        py = PyUser(dotnet)
        
        # create test data
        testValue = self.create(String)
        
        # set property to new test value
        py.full_name = testValue
        
        # assert value
        assert py.full_name == testValue
    
    def test_email_getter(self):
        dotnet = self.create(IUser)
        py = PyUser(dotnet)
        assert py.email == dotnet.Email
    
    def test_email_setter(self):
        dotnet = self.create(IUser)
        py = PyUser(dotnet)
        
        # create test data
        testValue = self.create(String)
        
        # set property to new test value
        py.email = testValue
        
        # assert value
        assert py.email == testValue
    
    def test_site_role_getter(self):
        dotnet = self.create(IUser)
        py = PyUser(dotnet)
        assert py.site_role == dotnet.SiteRole
    
    def test_site_role_setter(self):
        dotnet = self.create(IUser)
        py = PyUser(dotnet)
        
        # create test data
        testValue = self.create(String)
        
        # set property to new test value
        py.site_role = testValue
        
        # assert value
        assert py.site_role == testValue
    
    def test_authentication_type_getter(self):
        dotnet = self.create(IUser)
        py = PyUser(dotnet)
        assert py.authentication_type == dotnet.AuthenticationType
    
    def test_authentication_type_setter(self):
        dotnet = self.create(IUser)
        py = PyUser(dotnet)
        
        # create test data
        testValue = self.create(String)
        
        # set property to new test value
        py.authentication_type = testValue
        
        # assert value
        assert py.authentication_type == testValue
    
    def test_administrator_level_getter(self):
        dotnet = self.create(IUser)
        py = PyUser(dotnet)
        assert py.administrator_level == dotnet.AdministratorLevel
    
    def test_license_level_getter(self):
        dotnet = self.create(IUser)
        py = PyUser(dotnet)
        assert py.license_level == dotnet.LicenseLevel
    
    def test_can_publish_getter(self):
        dotnet = self.create(IUser)
        py = PyUser(dotnet)
        assert py.can_publish == dotnet.CanPublish
    
class TestPyWithDomainGenerated(AutoFixtureTestBase):
    
    def test_ctor(self):
        dotnet = self.create(IWithDomain)
        py = PyWithDomain(dotnet)
        assert py._dotnet == dotnet
    
    def test_domain_getter(self):
        dotnet = self.create(IWithDomain)
        py = PyWithDomain(dotnet)
        assert py.domain == dotnet.Domain
    
class TestPyWithOwnerGenerated(AutoFixtureTestBase):
    
    def test_ctor(self):
        dotnet = self.create(IWithOwner)
        py = PyWithOwner(dotnet)
        assert py._dotnet == dotnet
    
    def test_owner_getter(self):
        dotnet = self.create(IWithOwner)
        py = PyWithOwner(dotnet)
        assert py.owner == None if dotnet.Owner is None else PyContentReference(dotnet.Owner)
    
    def test_owner_setter(self):
        dotnet = self.create(IWithOwner)
        py = PyWithOwner(dotnet)
        
        # create test data
        testValue = self.create(IContentReference)
        
        # set property to new test value
        py.owner = None if testValue is None else PyContentReference(testValue)
        
        # assert value
        assert py.owner == None if testValue is None else PyContentReference(testValue)
    
class TestPyWithTagsGenerated(AutoFixtureTestBase):
    
    def test_ctor(self):
        dotnet = self.create(IWithTags)
        py = PyWithTags(dotnet)
        assert py._dotnet == dotnet
    
    def test_tags_getter(self):
        dotnet = self.create(IWithTags)
        py = PyWithTags(dotnet)
        assert len(dotnet.Tags) != 0
        assert len(py.tags) == len(dotnet.Tags)
    
    def test_tags_setter(self):
        dotnet = self.create(IWithTags)
        py = PyWithTags(dotnet)
        assert len(dotnet.Tags) != 0
        assert len(py.tags) == len(dotnet.Tags)
        
        # create test data
        dotnetCollection = DotnetList[ITag]()
        dotnetCollection.Add(self.create(ITag))
        dotnetCollection.Add(self.create(ITag))
        testCollection = [] if dotnetCollection is None else [PyTag(x) for x in dotnetCollection if x is not None]
        
        # set property to new test value
        py.tags = testCollection
        
        # assert value
        assert len(py.tags) == len(testCollection)
    
class TestPyWithWorkbookGenerated(AutoFixtureTestBase):
    
    def test_ctor(self):
        dotnet = self.create(IWithWorkbook)
        py = PyWithWorkbook(dotnet)
        assert py._dotnet == dotnet
    
    def test_workbook_getter(self):
        dotnet = self.create(IWithWorkbook)
        py = PyWithWorkbook(dotnet)
        assert py.workbook == None if dotnet.Workbook is None else PyContentReference(dotnet.Workbook)
    
    def test_workbook_setter(self):
        dotnet = self.create(IWithWorkbook)
        py = PyWithWorkbook(dotnet)
        
        # create test data
        testValue = self.create(IContentReference)
        
        # set property to new test value
        py.workbook = None if testValue is None else PyContentReference(testValue)
        
        # assert value
        assert py.workbook == None if testValue is None else PyContentReference(testValue)
    
class TestPyWorkbookGenerated(AutoFixtureTestBase):
    
    def test_ctor(self):
        dotnet = self.create(IWorkbook)
        py = PyWorkbook(dotnet)
        assert py._dotnet == dotnet
    
    def test_show_tabs_getter(self):
        dotnet = self.create(IWorkbook)
        py = PyWorkbook(dotnet)
        assert py.show_tabs == dotnet.ShowTabs
    
    def test_show_tabs_setter(self):
        dotnet = self.create(IWorkbook)
        py = PyWorkbook(dotnet)
        
        # create test data
        testValue = self.create(Boolean)
        
        # set property to new test value
        py.show_tabs = testValue
        
        # assert value
        assert py.show_tabs == testValue
    
    def test_size_getter(self):
        dotnet = self.create(IWorkbook)
        py = PyWorkbook(dotnet)
        assert py.size == dotnet.Size
    
class TestPyWorkbookDetailsGenerated(AutoFixtureTestBase):
    
    def test_ctor(self):
        dotnet = self.create(IWorkbookDetails)
        py = PyWorkbookDetails(dotnet)
        assert py._dotnet == dotnet
    
    def test_views_getter(self):
        dotnet = self.create(IWorkbookDetails)
        py = PyWorkbookDetails(dotnet)
        assert len(dotnet.Views) != 0
        assert len(py.views) == len(dotnet.Views)
    

# endregion

