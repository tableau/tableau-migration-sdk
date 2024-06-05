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

"""Transformers for the Python.TestApplication."""

from tableau_migration import (
    ContentTransformerBase,
    IPublishableGroup,
    IUser
)

class RemoveMissingDestinationUsersFromGroupsTransformer(ContentTransformerBase[IPublishableGroup]):
    """A class to transform groups to not try to link skipped users to the group."""
    
    def transform(self, item_to_transform: IPublishableGroup) -> IPublishableGroup:
        destination_user_finder = self.services.get_destination_finder(IUser)
        users_list = []
        for usergroup in item_to_transform.users:
            destination_reference = destination_user_finder.find_by_id(usergroup.user.id)

            if destination_reference is not None:
                users_list.append(usergroup)
            
        item_to_transform.users = users_list
        
        return item_to_transform
