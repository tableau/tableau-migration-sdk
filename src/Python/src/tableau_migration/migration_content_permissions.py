# Copyright (c) 2026, Salesforce, Inc.
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

"""Wrapper for classes in Tableau.Migration.Content.Permissions namespace."""

# region _generated

from enum import IntEnum # noqa: E402, F401
from tableau_migration.migration import PyContentReference # noqa: E402, F401
from typing import (  # noqa: E402, F401
    Set,
    Sequence
)
from uuid import UUID # noqa: E402, F401

from System import Guid # noqa: E402, F401
from System.Collections.Generic import (  # noqa: E402, F401
    HashSet as DotnetHashSet,
    List as DotnetList
)
from Tableau.Migration.Content.Permissions import (  # noqa: E402, F401
    ICapability,
    IGranteeCapability,
    IPermissions,
    IPermissionSet
)

class PyCapability():
    """The interface for a grantee's capability."""
    
    _dotnet_base = ICapability
    
    def __init__(self, capability: ICapability) -> None:
        """Creates a new PyCapability object.
        
        Args:
            capability: A ICapability object.
        
        Returns: None.
        """
        self._dotnet = capability
        
    @property
    def name(self) -> str:
        """The capability name from PermissionsCapabilityNames."""
        return self._dotnet.Name
    
    @property
    def mode(self) -> str:
        """The capability mode from PermissionsCapabilityModes."""
        return self._dotnet.Mode
    
class PyGranteeType(IntEnum):
    """Enum of grantee types."""
    
    """The group grantee type."""
    GROUP = 0
    
    """The user grantee type."""
    USER = 1
    
    """The group set grantee type."""
    GROUP_SET = 2
    
class PyGranteeCapability():
    """Interface for the grantee of permissions."""
    
    _dotnet_base = IGranteeCapability
    
    def __init__(self, grantee_capability: IGranteeCapability) -> None:
        """Creates a new PyGranteeCapability object.
        
        Args:
            grantee_capability: A IGranteeCapability object.
        
        Returns: None.
        """
        self._dotnet = grantee_capability
        
    @property
    def grantee_type(self) -> PyGranteeType:
        """Indicates the type of grantee."""
        return None if self._dotnet.GranteeType is None else PyGranteeType(self._dotnet.GranteeType.value__)
    
    @property
    def grantee_id(self) -> UUID:
        """Gets the ID for grantee."""
        return None if self._dotnet.GranteeId is None else UUID(self._dotnet.GranteeId.ToString())
    
    @property
    def grantee(self) -> PyContentReference:
        """Gets the grantee content reference."""
        return None if self._dotnet.Grantee is None else PyContentReference(self._dotnet.Grantee)
    
    @grantee.setter
    def grantee(self, value: PyContentReference) -> None:
        """Gets the grantee content reference."""
        self._dotnet.Grantee = None if value is None else value._dotnet
    
    @property
    def capabilities(self) -> Set[PyCapability]:
        """Gets the collection of capabilities of the grantee."""
        return [] if self._dotnet.Capabilities is None else [PyCapability(x) for x in self._dotnet.Capabilities if x is not None]
    
    def resolve_capability_mode_conflicts(self) -> None:
        """Resolves Deny in case of conflict."""
        self._dotnet.ResolveCapabilityModeConflicts()
    
class PyPermissionSet():
    """Interface for a set of permissions."""
    
    _dotnet_base = IPermissionSet
    
    def __init__(self, permission_set: IPermissionSet) -> None:
        """Creates a new PyPermissionSet object.
        
        Args:
            permission_set: A IPermissionSet object.
        
        Returns: None.
        """
        self._dotnet = permission_set
        
    @property
    def grantee_capabilities(self) -> Sequence[PyGranteeCapability]:
        """Gets or sets the grantee capabilities of the permission set."""
        return [] if self._dotnet.GranteeCapabilities is None else [PyGranteeCapability(x) for x in self._dotnet.GranteeCapabilities if x is not None]
    
    @grantee_capabilities.setter
    def grantee_capabilities(self, value: Sequence[PyGranteeCapability]) -> None:
        """Gets or sets the grantee capabilities of the permission set."""
        if value is None:
            self._dotnet.GranteeCapabilities = DotnetList[IGranteeCapability]()
        else:
            dotnet_collection = DotnetList[IGranteeCapability]()
            for x in filter(None,value):
                dotnet_collection.Add(x._dotnet)
            self._dotnet.GranteeCapabilities = dotnet_collection
    
class PyPermissions(PyPermissionSet):
    """Interface for the permission information of a content item."""
    
    _dotnet_base = IPermissions
    
    def __init__(self, permissions: IPermissions) -> None:
        """Creates a new PyPermissions object.
        
        Args:
            permissions: A IPermissions object.
        
        Returns: None.
        """
        self._dotnet = permissions
        
    @property
    def parent_id(self) -> UUID:
        """The ID of the parent content item that is determining permissions, such as a locked project. The parent content can be one of the types in ParentContentTypeNames, and will be null if the permissions are determined by the content item directly."""
        return None if self._dotnet.ParentId is None else UUID(self._dotnet.ParentId.ToString())
    

# endregion

from typing import Union # noqa: E402, F401

from migration_api_rest_models import PyPermissionsCapabilityModes, PyPermissionsCapabilityNames # noqa: E402, F401
from Tableau.Migration import (  # noqa: E402, F401
    ContentLocation
)
from Tableau.Migration.Content import (  # noqa: E402, F401
    ContentReferenceStub
)
from Tableau.Migration.Content.Permissions import (  # noqa: E402, F401
    Capability,
    GranteeCapability,
    GranteeType
)

def _create_capability(name: PyPermissionsCapabilityNames, mode: PyPermissionsCapabilityModes) -> PyCapability:
    return PyCapability(Capability(str(name), str(mode)))

PyCapability.create = _create_capability

def _create_grantee_capability(grantee_type: PyGranteeType, grantee: Union[PyContentReference, UUID], capabilities: Sequence[PyCapability]) -> PyGranteeCapability:
    
    dotnet_capabilities = DotnetList[ICapability]()
    if capabilities is not None:
        for x in filter(None, capabilities):
            dotnet_capabilities.Add(x._dotnet)

    if isinstance(grantee, UUID):
        dotnet_grantee = ContentReferenceStub(Guid.Parse(str(grantee)), "", ContentLocation(DotnetList[str]()))
    else:
        dotnet_grantee = grantee._dotnet

    return PyGranteeCapability(GranteeCapability(GranteeType(int(grantee_type)), dotnet_grantee, dotnet_capabilities))

PyGranteeCapability.create = _create_grantee_capability