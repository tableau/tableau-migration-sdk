# Copyright (c) 2025, Salesforce, Inc.
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

"""Wrapper for classes in Tableau.Migration.Content namespace."""

from Tableau.Migration import IContentReference # noqa: E402, F401

# region _generated

from enum import IntEnum # noqa: E402, F401
from tableau_migration.migration import (  # noqa: E402, F401
    PyContentReference,
    PyEmptyIdContentReference,
    _generic_wrapper
)
from tableau_migration.migration_api_rest import PyRestIdentifiable # noqa: E402, F401
from tableau_migration.migration_content_schedules import PyWithSchedule # noqa: E402, F401
from tableau_migration.migration_content_schedules_cloud import PyCloudSchedule # noqa: E402, F401
from tableau_migration.migration_content_schedules_server import PyServerSchedule # noqa: E402, F401
from typing import (  # noqa: E402, F401
    Sequence,
    List,
    Generic,
    TypeVar
)
from typing_extensions import Self # noqa: E402, F401
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
    ICloudSubscription,
    IConnection,
    IConnectionsContent,
    IContainerContent,
    ICustomView,
    IDataSource,
    IDataSourceDetails,
    IDescriptionContent,
    IExtractContent,
    IFavorite,
    IGroup,
    IGroupSet,
    IGroupUser,
    ILabel,
    IProject,
    IPublishableCustomView,
    IPublishableDataSource,
    IPublishableGroup,
    IPublishableGroupSet,
    IPublishableWorkbook,
    IPublishedContent,
    IServerSubscription,
    ISubscription,
    ISubscriptionContent,
    ITag,
    IUser,
    IUsernameContent,
    IView,
    IWithDomain,
    IWithOwner,
    IWithTags,
    IWithWorkbook,
    IWorkbook,
    IWorkbookDetails,
    UserAuthenticationType
)

TSchedule = TypeVar("TSchedule")

class PyWithOwner(PyContentReference):
    """Interface to be inherited by content items with owner."""
    
    _dotnet_base = IWithOwner
    
    def __init__(self, with_owner: IWithOwner) -> None:
        """Creates a new PyWithOwner object.
        
        Args:
            with_owner: A IWithOwner object.
        
        Returns: None.
        """
        self._dotnet = with_owner
        
    @property
    def owner(self) -> PyContentReference:
        """Gets or sets the owner for the content item."""
        return None if self._dotnet.Owner is None else PyContentReference(self._dotnet.Owner)
    
    @owner.setter
    def owner(self, value: PyContentReference) -> None:
        """Gets or sets the owner for the content item."""
        self._dotnet.Owner = None if value is None else value._dotnet
    
class PySubscriptionContent():
    """The content of the subscription."""
    
    _dotnet_base = ISubscriptionContent
    
    def __init__(self, subscription_content: ISubscriptionContent) -> None:
        """Creates a new PySubscriptionContent object.
        
        Args:
            subscription_content: A ISubscriptionContent object.
        
        Returns: None.
        """
        self._dotnet = subscription_content
        
    @property
    def id(self) -> UUID:
        """The ID of the content item tied to the subscription."""
        return None if self._dotnet.Id is None else UUID(self._dotnet.Id.ToString())
    
    @id.setter
    def id(self, value: UUID) -> None:
        """The ID of the content item tied to the subscription."""
        self._dotnet.Id = None if value is None else Guid.Parse(str(value))
    
    @property
    def type(self) -> str:
        """The content type of the subscription."""
        return self._dotnet.Type
    
    @type.setter
    def type(self, value: str) -> None:
        """The content type of the subscription."""
        self._dotnet.Type = value
    
    @property
    def send_if_view_empty(self) -> bool:
        """Whether or not send the notification if the view is empty."""
        return self._dotnet.SendIfViewEmpty
    
    @send_if_view_empty.setter
    def send_if_view_empty(self, value: bool) -> None:
        """Whether or not send the notification if the view is empty."""
        self._dotnet.SendIfViewEmpty = value
    
class PySubscription(Generic[TSchedule], PyWithSchedule[TSchedule], PyWithOwner):
    """Interface for a subscription."""
    
    _dotnet_base = ISubscription
    
    def __init__(self, subscription: ISubscription) -> None:
        """Creates a new PySubscription object.
        
        Args:
            subscription: A ISubscription object.
        
        Returns: None.
        """
        self._dotnet = subscription
        
    @property
    def subject(self) -> str:
        """Gets or sets the subject of the subscription."""
        return self._dotnet.Subject
    
    @subject.setter
    def subject(self, value: str) -> None:
        """Gets or sets the subject of the subscription."""
        self._dotnet.Subject = value
    
    @property
    def attach_image(self) -> bool:
        """Gets or sets whether or not an image file should be attached to the notification."""
        return self._dotnet.AttachImage
    
    @attach_image.setter
    def attach_image(self, value: bool) -> None:
        """Gets or sets whether or not an image file should be attached to the notification."""
        self._dotnet.AttachImage = value
    
    @property
    def attach_pdf(self) -> bool:
        """Gets or sets whether or not a pdf file should be attached to the notification."""
        return self._dotnet.AttachPdf
    
    @attach_pdf.setter
    def attach_pdf(self, value: bool) -> None:
        """Gets or sets whether or not a pdf file should be attached to the notification."""
        self._dotnet.AttachPdf = value
    
    @property
    def page_orientation(self) -> str:
        """Gets or set the page orientation of the subscription."""
        return self._dotnet.PageOrientation
    
    @page_orientation.setter
    def page_orientation(self, value: str) -> None:
        """Gets or set the page orientation of the subscription."""
        self._dotnet.PageOrientation = value
    
    @property
    def page_size_option(self) -> str:
        """Gets or set the page page size option of the subscription."""
        return self._dotnet.PageSizeOption
    
    @page_size_option.setter
    def page_size_option(self, value: str) -> None:
        """Gets or set the page page size option of the subscription."""
        self._dotnet.PageSizeOption = value
    
    @property
    def suspended(self) -> bool:
        """Gets or sets whether or not the subscription is suspended."""
        return self._dotnet.Suspended
    
    @suspended.setter
    def suspended(self, value: bool) -> None:
        """Gets or sets whether or not the subscription is suspended."""
        self._dotnet.Suspended = value
    
    @property
    def message(self) -> str:
        """Gets or sets the message of the subscription."""
        return self._dotnet.Message
    
    @message.setter
    def message(self, value: str) -> None:
        """Gets or sets the message of the subscription."""
        self._dotnet.Message = value
    
    @property
    def content(self) -> PySubscriptionContent:
        """Gets or set the content reference of the subscription."""
        return None if self._dotnet.Content is None else PySubscriptionContent(self._dotnet.Content)
    
    @content.setter
    def content(self, value: PySubscriptionContent) -> None:
        """Gets or set the content reference of the subscription."""
        self._dotnet.Content = None if value is None else value._dotnet
    
class PyCloudSubscription(PySubscription[PyCloudSchedule]):
    """The interface for a cloud subscription."""
    
    _dotnet_base = ICloudSubscription
    
    def __init__(self, cloud_subscription: ICloudSubscription) -> None:
        """Creates a new PyCloudSubscription object.
        
        Args:
            cloud_subscription: A ICloudSubscription object.
        
        Returns: None.
        """
        self._dotnet = cloud_subscription
        
class PyConnection():
    """Interface for a content item's embedded connection."""
    
    _dotnet_base = IConnection
    
    def __init__(self, connection: IConnection) -> None:
        """Creates a new PyConnection object.
        
        Args:
            connection: A IConnection object.
        
        Returns: None.
        """
        self._dotnet = connection
        
    @property
    def id(self) -> UUID:
        """Gets the unique identifier."""
        return None if self._dotnet.Id is None else UUID(self._dotnet.Id.ToString())
    
    @property
    def type(self) -> str:
        """Gets the connection type for the response."""
        return self._dotnet.Type
    
    @property
    def server_address(self) -> str:
        """Gets the server address for the response."""
        return self._dotnet.ServerAddress
    
    @property
    def server_port(self) -> str:
        """Gets the server port for the response."""
        return self._dotnet.ServerPort
    
    @property
    def connection_username(self) -> str:
        """Gets the connection username for the response."""
        return self._dotnet.ConnectionUsername
    
    @property
    def query_tagging_enabled(self) -> bool:
        """Gets the query tagging enabled flag for the response. This is returned only for administrator users."""
        return self._dotnet.QueryTaggingEnabled
    
    @property
    def authentication_type(self) -> str:
        """Gets the authentication type for the response."""
        return self._dotnet.AuthenticationType
    
    @property
    def use_o_auth_managed_keychain(self) -> bool:
        """Gets whether to use OAuth managed keychain."""
        return self._dotnet.UseOAuthManagedKeychain
    
    @property
    def embed_password(self) -> bool:
        """Gets whether to embed the password."""
        return self._dotnet.EmbedPassword
    
class PyConnectionsContent():
    """Interface for content that has connection metadata."""
    
    _dotnet_base = IConnectionsContent
    
    def __init__(self, connections_content: IConnectionsContent) -> None:
        """Creates a new PyConnectionsContent object.
        
        Args:
            connections_content: A IConnectionsContent object.
        
        Returns: None.
        """
        self._dotnet = connections_content
        
    @property
    def connections(self) -> Sequence[PyConnection]:
        """Gets the connection metadata. Connection metadata is read only because connection metadata should not be transformed directly. Instead, connections should be modified by either: 1) manipulating XML before publishing, or 2) updating connection metadata in a post-publish hook."""
        return None if self._dotnet.Connections is None else list((None if x is None else PyConnection(x)) for x in self._dotnet.Connections)
    
    @property
    def has_embedded_password(self) -> bool:
        """Gets whether any Connections have an embedded password."""
        return self._dotnet.HasEmbeddedPassword
    
    @property
    def has_embedded_o_auth_managed_keychain(self) -> bool:
        """Gets whether any Connections have an embedded password and uses OAuth managed keychain."""
        return self._dotnet.HasEmbeddedOAuthManagedKeychain
    
    @property
    def has_embedded_o_auth_credentials(self) -> bool:
        """Gets whether any Connections have an embedded password and an OAuth authentication type."""
        return self._dotnet.HasEmbeddedOAuthCredentials
    
class PyContainerContent():
    """Interface for a content item that belongs to a container (e.g. project or personal space)."""
    
    _dotnet_base = IContainerContent
    
    def __init__(self, container_content: IContainerContent) -> None:
        """Creates a new PyContainerContent object.
        
        Args:
            container_content: A IContainerContent object.
        
        Returns: None.
        """
        self._dotnet = container_content
        
    @property
    def container(self) -> PyContentReference:
        """Gets the container for the content item. Relocating the content should be done through mapping."""
        return None if self._dotnet.Container is None else PyContentReference(self._dotnet.Container)
    
class PyWithWorkbook(PyContentReference):
    """Interface to be inherited by content items with workbook."""
    
    _dotnet_base = IWithWorkbook
    
    def __init__(self, with_workbook: IWithWorkbook) -> None:
        """Creates a new PyWithWorkbook object.
        
        Args:
            with_workbook: A IWithWorkbook object.
        
        Returns: None.
        """
        self._dotnet = with_workbook
        
    @property
    def workbook(self) -> PyContentReference:
        """Gets or sets the workbook for the content item."""
        return None if self._dotnet.Workbook is None else PyContentReference(self._dotnet.Workbook)
    
    @workbook.setter
    def workbook(self, value: PyContentReference) -> None:
        """Gets or sets the workbook for the content item."""
        self._dotnet.Workbook = None if value is None else value._dotnet
    
class PyCustomView(PyWithOwner, PyWithWorkbook):
    """The interface for a custom view content item."""
    
    _dotnet_base = ICustomView
    
    def __init__(self, custom_view: ICustomView) -> None:
        """Creates a new PyCustomView object.
        
        Args:
            custom_view: A ICustomView object.
        
        Returns: None.
        """
        self._dotnet = custom_view
        
    @property
    def created_at(self) -> str:
        """Gets the created timestamp."""
        return self._dotnet.CreatedAt
    
    @property
    def updated_at(self) -> str:
        """Gets the updated timestamp."""
        return self._dotnet.UpdatedAt
    
    @property
    def last_accessed_at(self) -> str:
        """Gets the last accessed timestamp."""
        return self._dotnet.LastAccessedAt
    
    @property
    def shared(self) -> bool:
        """Gets or sets whether the custom view is shared with all users (true) or private (false)."""
        return self._dotnet.Shared
    
    @shared.setter
    def shared(self, value: bool) -> None:
        """Gets or sets whether the custom view is shared with all users (true) or private (false)."""
        self._dotnet.Shared = value
    
    @property
    def base_view_id(self) -> UUID:
        """Gets the ID of the view that this custom view is based on."""
        return None if self._dotnet.BaseViewId is None else UUID(self._dotnet.BaseViewId.ToString())
    
    @property
    def base_view_name(self) -> str:
        """Gets the name of the view that this custom view is based on."""
        return self._dotnet.BaseViewName
    
class PyPublishedContent():
    """Interface for a content item that has metadata around publishing information."""
    
    _dotnet_base = IPublishedContent
    
    def __init__(self, published_content: IPublishedContent) -> None:
        """Creates a new PyPublishedContent object.
        
        Args:
            published_content: A IPublishedContent object.
        
        Returns: None.
        """
        self._dotnet = published_content
        
    @property
    def created_at(self) -> str:
        """Gets the created timestamp."""
        return self._dotnet.CreatedAt
    
    @property
    def updated_at(self) -> str:
        """Gets the updated timestamp."""
        return self._dotnet.UpdatedAt
    
    @property
    def webpage_url(self) -> str:
        """Gets the webpage URL."""
        return self._dotnet.WebpageUrl
    
class PyDescriptionContent():
    """Interface for a content item that has a description."""
    
    _dotnet_base = IDescriptionContent
    
    def __init__(self, description_content: IDescriptionContent) -> None:
        """Creates a new PyDescriptionContent object.
        
        Args:
            description_content: A IDescriptionContent object.
        
        Returns: None.
        """
        self._dotnet = description_content
        
    @property
    def description(self) -> str:
        """Gets or sets the description."""
        return self._dotnet.Description
    
    @description.setter
    def description(self, value: str) -> None:
        """Gets or sets the description."""
        self._dotnet.Description = value
    
class PyExtractContent():
    """Interface for a content item that has an extract."""
    
    _dotnet_base = IExtractContent
    
    def __init__(self, extract_content: IExtractContent) -> None:
        """Creates a new PyExtractContent object.
        
        Args:
            extract_content: A IExtractContent object.
        
        Returns: None.
        """
        self._dotnet = extract_content
        
    @property
    def encrypt_extracts(self) -> bool:
        """Gets or sets whether or not extracts are encrypted."""
        return self._dotnet.EncryptExtracts
    
    @encrypt_extracts.setter
    def encrypt_extracts(self, value: bool) -> None:
        """Gets or sets whether or not extracts are encrypted."""
        self._dotnet.EncryptExtracts = value
    
class PyTag():
    """Interface for tags associated with content items."""
    
    _dotnet_base = ITag
    
    def __init__(self, tag: ITag) -> None:
        """Creates a new PyTag object.
        
        Args:
            tag: A ITag object.
        
        Returns: None.
        """
        self._dotnet = tag
        
    @property
    def label(self) -> str:
        """Gets or sets label for the tag."""
        return self._dotnet.Label
    
    @label.setter
    def label(self, value: str) -> None:
        """Gets or sets label for the tag."""
        self._dotnet.Label = value
    
class PyWithTags():
    """Interface to be inherited by content items with tags."""
    
    _dotnet_base = IWithTags
    
    def __init__(self, with_tags: IWithTags) -> None:
        """Creates a new PyWithTags object.
        
        Args:
            with_tags: A IWithTags object.
        
        Returns: None.
        """
        self._dotnet = with_tags
        
    @property
    def tags(self) -> List[PyTag]:
        """Gets or sets the tags for the content item."""
        return [] if self._dotnet.Tags is None else [PyTag(x) for x in self._dotnet.Tags if x is not None]
    
    @tags.setter
    def tags(self, value: List[PyTag]) -> None:
        """Gets or sets the tags for the content item."""
        if value is None:
            self._dotnet.Tags = DotnetList[ITag]()
        else:
            dotnet_collection = DotnetList[ITag]()
            for x in filter(None,value):
                dotnet_collection.Add(x._dotnet)
            self._dotnet.Tags = dotnet_collection
    
class PyDataSource(PyPublishedContent, PyDescriptionContent, PyExtractContent, PyWithTags, PyContainerContent, PyWithOwner):
    """Interface for a data source content item."""
    
    _dotnet_base = IDataSource
    
    def __init__(self, data_source: IDataSource) -> None:
        """Creates a new PyDataSource object.
        
        Args:
            data_source: A IDataSource object.
        
        Returns: None.
        """
        self._dotnet = data_source
        
    @property
    def has_extracts(self) -> bool:
        """Gets whether or not the data source has extracts."""
        return self._dotnet.HasExtracts
    
    @property
    def is_certified(self) -> bool:
        """Gets the IsCertified flag for the data source. Should be updated through a post-publish hook."""
        return self._dotnet.IsCertified
    
    @property
    def use_remote_query_agent(self) -> bool:
        """Gets or sets the UseRemoteQueryAgent flag for the data source."""
        return self._dotnet.UseRemoteQueryAgent
    
    @use_remote_query_agent.setter
    def use_remote_query_agent(self, value: bool) -> None:
        """Gets or sets the UseRemoteQueryAgent flag for the data source."""
        self._dotnet.UseRemoteQueryAgent = value
    
class PyDataSourceDetails(PyDataSource):
    """Interface for a data source object with extended information, from a GET query for example."""
    
    _dotnet_base = IDataSourceDetails
    
    def __init__(self, data_source_details: IDataSourceDetails) -> None:
        """Creates a new PyDataSourceDetails object.
        
        Args:
            data_source_details: A IDataSourceDetails object.
        
        Returns: None.
        """
        self._dotnet = data_source_details
        
    @property
    def certification_note(self) -> str:
        """Gets the certification note."""
        return self._dotnet.CertificationNote
    
class PyFavoriteContentType(IntEnum):
    """Enum of content types for favorites."""
    
    """Unknown content type."""
    UNKNOWN = 0
    
    """Workbook content type."""
    PROJECT = 1
    
    """Workbook content type."""
    WORKBOOK = 2
    
    """Workbook content type."""
    VIEW = 3
    
    """Data source content type."""
    DATA_SOURCE = 4
    
    """Flow content type."""
    FLOW = 5
    
    """Collection content type."""
    COLLECTION = 6
    
class PyFavorite(PyEmptyIdContentReference):
    """Interface for a content item named favorite."""
    
    _dotnet_base = IFavorite
    
    def __init__(self, favorite: IFavorite) -> None:
        """Creates a new PyFavorite object.
        
        Args:
            favorite: A IFavorite object.
        
        Returns: None.
        """
        self._dotnet = favorite
        
    @property
    def label(self) -> str:
        """Gets or sets the label for the favorite."""
        return self._dotnet.Label
    
    @label.setter
    def label(self, value: str) -> None:
        """Gets or sets the label for the favorite."""
        self._dotnet.Label = value
    
    @property
    def user(self) -> PyContentReference:
        """Gets the user IContentReference for the favorite."""
        return None if self._dotnet.User is None else PyContentReference(self._dotnet.User)
    
    @user.setter
    def user(self, value: PyContentReference) -> None:
        """Gets the user IContentReference for the favorite."""
        self._dotnet.User = None if value is None else value._dotnet
    
    @property
    def content(self) -> PyContentReference:
        """Gets or sets the IContentReference for the favorite."""
        return None if self._dotnet.Content is None else PyContentReference(self._dotnet.Content)
    
    @content.setter
    def content(self, value: PyContentReference) -> None:
        """Gets or sets the IContentReference for the favorite."""
        self._dotnet.Content = None if value is None else value._dotnet
    
    @property
    def content_type(self) -> PyFavoriteContentType:
        """Gets or sets the content type for the favorite."""
        return None if self._dotnet.ContentType is None else PyFavoriteContentType(self._dotnet.ContentType.value__)
    
class PyWithDomain():
    """Interface for content items with a domain."""
    
    _dotnet_base = IWithDomain
    
    def __init__(self, with_domain: IWithDomain) -> None:
        """Creates a new PyWithDomain object.
        
        Args:
            with_domain: A IWithDomain object.
        
        Returns: None.
        """
        self._dotnet = with_domain
        
    @property
    def domain(self) -> str:
        """Gets the domain this item belongs to. Changes should be made through mapping."""
        return self._dotnet.Domain
    
class PyUsernameContent(PyContentReference, PyWithDomain):
    """Interface for a content item that uses a domain qualified username."""
    
    _dotnet_base = IUsernameContent
    
    def __init__(self, username_content: IUsernameContent) -> None:
        """Creates a new PyUsernameContent object.
        
        Args:
            username_content: A IUsernameContent object.
        
        Returns: None.
        """
        self._dotnet = username_content
        
class PyGroup(PyUsernameContent):
    """Interface for a group content item."""
    
    _dotnet_base = IGroup
    
    def __init__(self, group: IGroup) -> None:
        """Creates a new PyGroup object.
        
        Args:
            group: A IGroup object.
        
        Returns: None.
        """
        self._dotnet = group
        
    @property
    def grant_license_mode(self) -> str:
        """Gets the grant license mode of the group."""
        return self._dotnet.GrantLicenseMode
    
    @grant_license_mode.setter
    def grant_license_mode(self, value: str) -> None:
        """Gets the grant license mode of the group."""
        self._dotnet.GrantLicenseMode = value
    
    @property
    def site_role(self) -> str:
        """Gets the site role of the group."""
        return self._dotnet.SiteRole
    
    @site_role.setter
    def site_role(self, value: str) -> None:
        """Gets the site role of the group."""
        self._dotnet.SiteRole = value
    
class PyGroupSet(PyContentReference):
    """Interface for a group set content item."""
    
    _dotnet_base = IGroupSet
    
    def __init__(self, group_set: IGroupSet) -> None:
        """Creates a new PyGroupSet object.
        
        Args:
            group_set: A IGroupSet object.
        
        Returns: None.
        """
        self._dotnet = group_set
        
class PyGroupUser():
    """Interface for a user linked to a group content item."""
    
    _dotnet_base = IGroupUser
    
    def __init__(self, group_user: IGroupUser) -> None:
        """Creates a new PyGroupUser object.
        
        Args:
            group_user: A IGroupUser object.
        
        Returns: None.
        """
        self._dotnet = group_user
        
    @property
    def user(self) -> PyContentReference:
        """Gets the user that belongs to a group."""
        return None if self._dotnet.User is None else PyContentReference(self._dotnet.User)
    
    @user.setter
    def user(self, value: PyContentReference) -> None:
        """Gets the user that belongs to a group."""
        self._dotnet.User = None if value is None else value._dotnet
    
class PyLabel(PyRestIdentifiable):
    """Interface for a content item's label."""
    
    _dotnet_base = ILabel
    
    def __init__(self, label: ILabel) -> None:
        """Creates a new PyLabel object.
        
        Args:
            label: A ILabel object.
        
        Returns: None.
        """
        self._dotnet = label
        
    @property
    def site_id(self) -> UUID:
        """Gets the site ID."""
        return None if self._dotnet.SiteId is None else UUID(self._dotnet.SiteId.ToString())
    
    @property
    def owner_id(self) -> UUID:
        """Gets the owner ID."""
        return None if self._dotnet.OwnerId is None else UUID(self._dotnet.OwnerId.ToString())
    
    @property
    def user_display_name(self) -> str:
        """Gets the user display name."""
        return self._dotnet.UserDisplayName
    
    @property
    def content_id(self) -> UUID:
        """Gets the ID for the label's content item."""
        return None if self._dotnet.ContentId is None else UUID(self._dotnet.ContentId.ToString())
    
    @property
    def content_type(self) -> str:
        """Gets the type for the label's content item."""
        return self._dotnet.ContentType
    
    @property
    def message(self) -> str:
        """Gets the message."""
        return self._dotnet.Message
    
    @property
    def value(self) -> str:
        """Gets the value."""
        return self._dotnet.Value
    
    @property
    def category(self) -> str:
        """Gets the category."""
        return self._dotnet.Category
    
    @property
    def active(self) -> bool:
        """Gets the active flag."""
        return self._dotnet.Active
    
    @property
    def elevated(self) -> bool:
        """Gets the active flag."""
        return self._dotnet.Elevated
    
    @property
    def created_at(self) -> str:
        """Gets the create timestamp."""
        return self._dotnet.CreatedAt
    
    @property
    def updated_at(self) -> str:
        """Gets the update timestamp."""
        return self._dotnet.UpdatedAt
    
class PyProject(PyDescriptionContent, PyWithOwner):
    """Interface for a project content item."""
    
    _dotnet_base = IProject
    
    def __init__(self, project: IProject) -> None:
        """Creates a new PyProject object.
        
        Args:
            project: A IProject object.
        
        Returns: None.
        """
        self._dotnet = project
        
    @property
    def content_permissions(self) -> str:
        """Gets or sets the content permission mode of the project."""
        return self._dotnet.ContentPermissions
    
    @content_permissions.setter
    def content_permissions(self, value: str) -> None:
        """Gets or sets the content permission mode of the project."""
        self._dotnet.ContentPermissions = value
    
    @property
    def parent_project(self) -> PyContentReference:
        """Gets the parent project reference, or null if the project is a top-level project. Should be changed through mapping."""
        return None if self._dotnet.ParentProject is None else PyContentReference(self._dotnet.ParentProject)
    
class PyPublishableCustomView(PyCustomView):
    """Interface for the publishable version of ICustomView."""
    
    _dotnet_base = IPublishableCustomView
    
    def __init__(self, publishable_custom_view: IPublishableCustomView) -> None:
        """Creates a new PyPublishableCustomView object.
        
        Args:
            publishable_custom_view: A IPublishableCustomView object.
        
        Returns: None.
        """
        self._dotnet = publishable_custom_view
        
    @property
    def default_users(self) -> List[PyContentReference]:
        """The list of users for whom the Custom View is the default."""
        return [] if self._dotnet.DefaultUsers is None else [PyContentReference(x) for x in self._dotnet.DefaultUsers if x is not None]
    
    @default_users.setter
    def default_users(self, value: List[PyContentReference]) -> None:
        """The list of users for whom the Custom View is the default."""
        if value is None:
            self._dotnet.DefaultUsers = DotnetList[IContentReference]()
        else:
            dotnet_collection = DotnetList[IContentReference]()
            for x in filter(None,value):
                dotnet_collection.Add(x._dotnet)
            self._dotnet.DefaultUsers = dotnet_collection
    
class PyPublishableDataSource(PyDataSourceDetails, PyConnectionsContent):
    """Interface for a IDataSource that has been downloaded and has full information necessary for re-publishing."""
    
    _dotnet_base = IPublishableDataSource
    
    def __init__(self, publishable_data_source: IPublishableDataSource) -> None:
        """Creates a new PyPublishableDataSource object.
        
        Args:
            publishable_data_source: A IPublishableDataSource object.
        
        Returns: None.
        """
        self._dotnet = publishable_data_source
        
class PyPublishableGroup(PyGroup):
    """Interface for a group content item with users."""
    
    _dotnet_base = IPublishableGroup
    
    def __init__(self, publishable_group: IPublishableGroup) -> None:
        """Creates a new PyPublishableGroup object.
        
        Args:
            publishable_group: A IPublishableGroup object.
        
        Returns: None.
        """
        self._dotnet = publishable_group
        
    @property
    def users(self) -> List[PyGroupUser]:
        """Gets or sets the users assigned to the group."""
        return [] if self._dotnet.Users is None else [PyGroupUser(x) for x in self._dotnet.Users if x is not None]
    
    @users.setter
    def users(self, value: List[PyGroupUser]) -> None:
        """Gets or sets the users assigned to the group."""
        if value is None:
            self._dotnet.Users = DotnetList[IGroupUser]()
        else:
            dotnet_collection = DotnetList[IGroupUser]()
            for x in filter(None,value):
                dotnet_collection.Add(x._dotnet)
            self._dotnet.Users = dotnet_collection
    
class PyPublishableGroupSet(PyGroupSet):
    """Interface for a group set content item with groups."""
    
    _dotnet_base = IPublishableGroupSet
    
    def __init__(self, publishable_group_set: IPublishableGroupSet) -> None:
        """Creates a new PyPublishableGroupSet object.
        
        Args:
            publishable_group_set: A IPublishableGroupSet object.
        
        Returns: None.
        """
        self._dotnet = publishable_group_set
        
    @property
    def groups(self) -> List[PyContentReference]:
        """Gets or sets the groups belonging to the group set."""
        return [] if self._dotnet.Groups is None else [PyContentReference(x) for x in self._dotnet.Groups if x is not None]
    
    @groups.setter
    def groups(self, value: List[PyContentReference]) -> None:
        """Gets or sets the groups belonging to the group set."""
        if value is None:
            self._dotnet.Groups = DotnetList[IContentReference]()
        else:
            dotnet_collection = DotnetList[IContentReference]()
            for x in filter(None,value):
                dotnet_collection.Add(x._dotnet)
            self._dotnet.Groups = dotnet_collection
    
class PyWorkbook(PyPublishedContent, PyDescriptionContent, PyExtractContent, PyWithTags, PyContainerContent, PyWithOwner):
    """Interface for a workbook content item."""
    
    _dotnet_base = IWorkbook
    
    def __init__(self, workbook: IWorkbook) -> None:
        """Creates a new PyWorkbook object.
        
        Args:
            workbook: A IWorkbook object.
        
        Returns: None.
        """
        self._dotnet = workbook
        
    @property
    def show_tabs(self) -> bool:
        """Gets or sets whether tabs are shown."""
        return self._dotnet.ShowTabs
    
    @show_tabs.setter
    def show_tabs(self, value: bool) -> None:
        """Gets or sets whether tabs are shown."""
        self._dotnet.ShowTabs = value
    
    @property
    def size(self) -> int:
        """Gets the file size."""
        return self._dotnet.Size
    
class PyView(PyWithTags, PyContentReference, PyContainerContent):
    """Interface for view associated with the content item."""
    
    _dotnet_base = IView
    
    def __init__(self, view: IView) -> None:
        """Creates a new PyView object.
        
        Args:
            view: A IView object.
        
        Returns: None.
        """
        self._dotnet = view
        
    @property
    def parent_workbook(self) -> PyContentReference:
        """Gets the parent workbook of the view."""
        return None if self._dotnet.ParentWorkbook is None else PyContentReference(self._dotnet.ParentWorkbook)
    
class PyWorkbookDetails(PyWorkbook):
    """Interface for a workbook object with extended information, from a GET query for example."""
    
    _dotnet_base = IWorkbookDetails
    
    def __init__(self, workbook_details: IWorkbookDetails) -> None:
        """Creates a new PyWorkbookDetails object.
        
        Args:
            workbook_details: A IWorkbookDetails object.
        
        Returns: None.
        """
        self._dotnet = workbook_details
        
    @property
    def views(self) -> Sequence[PyView]:
        """Gets the view metadata."""
        return None if self._dotnet.Views is None else list((None if x is None else PyView(x)) for x in self._dotnet.Views)
    
class PyPublishableWorkbook(PyWorkbookDetails, PyConnectionsContent):
    """Interface for an IWorkbook that has been downloaded and has full information necessary for re-publishing."""
    
    _dotnet_base = IPublishableWorkbook
    
    def __init__(self, publishable_workbook: IPublishableWorkbook) -> None:
        """Creates a new PyPublishableWorkbook object.
        
        Args:
            publishable_workbook: A IPublishableWorkbook object.
        
        Returns: None.
        """
        self._dotnet = publishable_workbook
        
    @property
    def thumbnails_user_id(self) -> UUID:
        """Gets the ID of the user to generate thumbnails as."""
        return None if self._dotnet.ThumbnailsUserId is None else UUID(self._dotnet.ThumbnailsUserId.ToString())
    
    @thumbnails_user_id.setter
    def thumbnails_user_id(self, value: UUID) -> None:
        """Gets the ID of the user to generate thumbnails as."""
        self._dotnet.ThumbnailsUserId = None if value is None else Guid.Parse(str(value))
    
    @property
    def hidden_view_names(self) -> Sequence[str]:
        """Gets the names of the views that should be hidden."""
        return [] if self._dotnet.HiddenViewNames is None else [x for x in self._dotnet.HiddenViewNames if x is not None]
    
    @hidden_view_names.setter
    def hidden_view_names(self, value: Sequence[str]) -> None:
        """Gets the names of the views that should be hidden."""
        if value is None:
            self._dotnet.HiddenViewNames = DotnetHashSet[String]()
        else:
            dotnet_collection = DotnetHashSet[String]()
            for x in filter(None,value):
                dotnet_collection.Add(x)
            self._dotnet.HiddenViewNames = dotnet_collection
    
class PyServerSubscription(PySubscription[PyServerSchedule]):
    """The interface for a server subscription."""
    
    _dotnet_base = IServerSubscription
    
    def __init__(self, server_subscription: IServerSubscription) -> None:
        """Creates a new PyServerSubscription object.
        
        Args:
            server_subscription: A IServerSubscription object.
        
        Returns: None.
        """
        self._dotnet = server_subscription
        
class PyUserAuthenticationType():
    """Structure representing the authentication type of a user."""
    
    _dotnet_base = UserAuthenticationType
    
    def __init__(self, user_authentication_type: UserAuthenticationType) -> None:
        """Creates a new PyUserAuthenticationType object.
        
        Args:
            user_authentication_type: A UserAuthenticationType object.
        
        Returns: None.
        """
        self._dotnet = user_authentication_type
        
    @classmethod
    def get_default(cls) -> Self:
        """Gets a value representing the site default authentication type."""
        return None if UserAuthenticationType.Default is None else PyUserAuthenticationType(UserAuthenticationType.Default)
    
    @property
    def authentication_type(self) -> str:
        """Gets the authentication type, or null if the site uses IdpConfigurationIds."""
        return self._dotnet.AuthenticationType
    
    @property
    def idp_configuration_id(self) -> UUID:
        """Gets the IdP configuration ID, or null if the site uses AuthenticationTypes."""
        return None if self._dotnet.IdpConfigurationId is None else UUID(self._dotnet.IdpConfigurationId.ToString())
    
    @classmethod
    def for_authentication_type(cls, authentication_type: str) -> Self:
        """Creates a new UserAuthenticationType value.
        
        Args:
            authentication_type: The authentication type.
        
        Returns: The created UserAuthenticationType value.
        """
        result = UserAuthenticationType.ForAuthenticationType(authentication_type)
        return None if result is None else PyUserAuthenticationType(result)
    
    @classmethod
    def for_configuration_id(cls, idp_configuration_id: UUID) -> Self:
        """Creates a new UserAuthenticationType value.
        
        Args:
            idp_configuration_id: The IdP configuration ID.
        
        Returns: The created UserAuthenticationType value.
        """
        result = UserAuthenticationType.ForConfigurationId(None if idp_configuration_id is None else Guid.Parse(str(idp_configuration_id)))
        return None if result is None else PyUserAuthenticationType(result)
    
class PyUser(PyUsernameContent):
    """Interface for a user content item."""
    
    _dotnet_base = IUser
    
    def __init__(self, user: IUser) -> None:
        """Creates a new PyUser object.
        
        Args:
            user: A IUser object.
        
        Returns: None.
        """
        self._dotnet = user
        
    @property
    def full_name(self) -> str:
        """Gets or sets the full name of the user."""
        return self._dotnet.FullName
    
    @full_name.setter
    def full_name(self, value: str) -> None:
        """Gets or sets the full name of the user."""
        self._dotnet.FullName = value
    
    @property
    def email(self) -> str:
        """Gets or sets the email of the user."""
        return self._dotnet.Email
    
    @email.setter
    def email(self, value: str) -> None:
        """Gets or sets the email of the user."""
        self._dotnet.Email = value
    
    @property
    def site_role(self) -> str:
        """Gets or sets the site role of the user."""
        return self._dotnet.SiteRole
    
    @site_role.setter
    def site_role(self, value: str) -> None:
        """Gets or sets the site role of the user."""
        self._dotnet.SiteRole = value
    
    @property
    def authentication_type(self) -> str:
        """Gets or sets the authentication type of the user, or null to not send an explicit authentication type for the user during migration."""
        return self._dotnet.AuthenticationType
    
    @authentication_type.setter
    def authentication_type(self, value: str) -> None:
        """Gets or sets the authentication type of the user, or null to not send an explicit authentication type for the user during migration."""
        self._dotnet.AuthenticationType = value
    
    @property
    def authentication(self) -> PyUserAuthenticationType:
        """Gets or sets the authentication type of the user. Use Default to use either the default authentication type of the site."""
        return None if self._dotnet.Authentication is None else PyUserAuthenticationType(self._dotnet.Authentication)
    
    @authentication.setter
    def authentication(self, value: PyUserAuthenticationType) -> None:
        """Gets or sets the authentication type of the user. Use Default to use either the default authentication type of the site."""
        self._dotnet.Authentication = None if value is None else value._dotnet
    
    @property
    def administrator_level(self) -> str:
        """Gets the user's administrator level derived from SiteRole."""
        return self._dotnet.AdministratorLevel
    
    @property
    def license_level(self) -> str:
        """Gets the user's license level derived from SiteRole."""
        return self._dotnet.LicenseLevel
    
    @property
    def can_publish(self) -> bool:
        """Gets the user's publish capability derived from SiteRole."""
        return self._dotnet.CanPublish
    

# endregion

