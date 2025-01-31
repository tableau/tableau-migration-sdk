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

"""Wrapper for classes in Tableau.Migration.Api.Rest namespace."""

# region _generated

from uuid import UUID # noqa: E402, F401

from System import Guid # noqa: E402, F401
from Tableau.Migration.Api.Rest import IRestIdentifiable # noqa: E402, F401

class PyRestIdentifiable():
    """Interface for an object that uses a REST API-style LUID identifier."""
    
    _dotnet_base = IRestIdentifiable
    
    def __init__(self, rest_identifiable: IRestIdentifiable) -> None:
        """Creates a new PyRestIdentifiable object.
        
        Args:
            rest_identifiable: A IRestIdentifiable object.
        
        Returns: None.
        """
        self._dotnet = rest_identifiable
        
    @property
    def id(self) -> UUID:
        """Gets the unique identifier."""
        return None if self._dotnet.Id is None else UUID(self._dotnet.Id.ToString())
    

# endregion

