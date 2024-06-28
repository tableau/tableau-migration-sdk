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

# region _generated

from enum import IntEnum # noqa: E402, F401
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
    IPermissions
)

from tableau_migration.migration_content_permissions import (  # noqa: E402, F401
    PyCapability,
    PyGranteeCapability,
    PyGranteeType,
    PyPermissions
)


from Tableau.Migration.Content.Permissions import GranteeType

# Extra imports for tests.
from Tableau.Migration import IContentReference # noqa: E402, F401
from System import Nullable # noqa: E402, F401
from System.Collections.Generic import List as DotnetList # noqa: E402, F401
from tests.helpers.autofixture import AutoFixtureTestBase # noqa: E402, F401

class TestPyCapabilityGenerated(AutoFixtureTestBase):
    
    def test_ctor(self):
        dotnet = self.create(ICapability)
        py = PyCapability(dotnet)
        assert py._dotnet == dotnet
    
    def test_name_getter(self):
        dotnet = self.create(ICapability)
        py = PyCapability(dotnet)
        assert py.name == dotnet.Name
    
    def test_mode_getter(self):
        dotnet = self.create(ICapability)
        py = PyCapability(dotnet)
        assert py.mode == dotnet.Mode
    
class TestPyGranteeCapabilityGenerated(AutoFixtureTestBase):
    
    def test_ctor(self):
        dotnet = self.create(IGranteeCapability)
        py = PyGranteeCapability(dotnet)
        assert py._dotnet == dotnet
    
    def test_grantee_type_getter(self):
        dotnet = self.create(IGranteeCapability)
        py = PyGranteeCapability(dotnet)
        assert py.grantee_type.value == (None if dotnet.GranteeType is None else PyGranteeType(dotnet.GranteeType.value__)).value
    
    def test_grantee_id_getter(self):
        dotnet = self.create(IGranteeCapability)
        py = PyGranteeCapability(dotnet)
        assert py.grantee_id == None if dotnet.GranteeId is None else UUID(dotnet.GranteeId.ToString())
    
    def test_grantee_id_setter(self):
        dotnet = self.create(IGranteeCapability)
        py = PyGranteeCapability(dotnet)
        
        # create test data
        testValue = self.create(Guid)
        
        # set property to new test value
        py.grantee_id = None if testValue is None else UUID(testValue.ToString())
        
        # assert value
        assert py.grantee_id == None if testValue is None else UUID(testValue.ToString())
    
    def test_capabilities_getter(self):
        dotnet = self.create(IGranteeCapability)
        py = PyGranteeCapability(dotnet)
        assert len(dotnet.Capabilities) != 0
        assert len(py.capabilities) == len(dotnet.Capabilities)
    
class TestPyPermissionsGenerated(AutoFixtureTestBase):
    
    def test_ctor(self):
        dotnet = self.create(IPermissions)
        py = PyPermissions(dotnet)
        assert py._dotnet == dotnet
    
    def test_grantee_capabilities_getter(self):
        dotnet = self.create(IPermissions)
        py = PyPermissions(dotnet)
        assert len(dotnet.GranteeCapabilities) != 0
        assert len(py.grantee_capabilities) == len(dotnet.GranteeCapabilities)
    
    def test_grantee_capabilities_setter(self):
        dotnet = self.create(IPermissions)
        py = PyPermissions(dotnet)
        assert len(dotnet.GranteeCapabilities) != 0
        assert len(py.grantee_capabilities) == len(dotnet.GranteeCapabilities)
        
        # create test data
        dotnetCollection = DotnetList[IGranteeCapability]()
        dotnetCollection.Add(self.create(IGranteeCapability))
        dotnetCollection.Add(self.create(IGranteeCapability))
        testCollection = None if dotnetCollection is None else list((None if x is None else PyGranteeCapability(x)) for x in dotnetCollection)
        
        # set property to new test value
        py.grantee_capabilities = testCollection
        
        # assert value
        assert len(py.grantee_capabilities) == len(testCollection)
    
    def test_parent_id_getter(self):
        dotnet = self.create(IPermissions)
        py = PyPermissions(dotnet)
        assert py.parent_id == None if dotnet.ParentId is None else UUID(dotnet.ParentId.ToString())
    
    def test_parent_id_setter(self):
        dotnet = self.create(IPermissions)
        py = PyPermissions(dotnet)
        
        # create test data
        testValue = self.create(Nullable[Guid])
        
        # set property to new test value
        py.parent_id = None if testValue is None else UUID(testValue.ToString())
        
        # assert value
        assert py.parent_id == None if testValue is None else UUID(testValue.ToString())
    

# endregion

