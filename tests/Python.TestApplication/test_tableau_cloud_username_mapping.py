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

from configparser import ConfigParser
from Tableau.Migration.Interop.Hooks.Mappings import ISyncContentMapping
from Tableau.Migration.Content import IUser

class PyTestTableauCloudUsernameMapping(ISyncContentMapping[IUser]):
    __namespace__ = "MyNamespace"

    def __init__(self, config: ConfigParser) -> None:
        self._config: ConfigParser = config
        None

    def Execute(self, ctx):
        domain = ctx.MappedLocation.Parent()

        # Re-use an existing email if it already exists unless always override is set
        if self._config['TEST_TABLEAU_CLOUD_USERNAME_OPTIONS']['ALWAYS_OVERRIDE_ADDRESS'] == False and ctx.ContentItem.Email:
            return ctx.MapTo(domain.Append(ctx.ContentItem.Email))    
            
        username, domain = ctx.ContentItem.Email.split('@')
        test_email = username + ctx.ContentItem.Name.Replace(" ", "") + '@' + domain

        return ctx.MapTo(domain.Append(test_email))
    
