# Copyright (c) 2023, Salesforce, Inc.
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

"""Wrapper for classes in Tableau.Migration.Engine namespace."""
from typing_extensions import Self
from uuid import UUID

import tableau_migration.migration
from tableau_migration.migration import(
    PyResult)
from tableau_migration.migration_engine_hooks import PyMigrationHookBuilder
from tableau_migration.migration_engine_hooks_filters import PyContentFilterBuilder
from tableau_migration.migration_engine_hooks_mappings import PyContentMappingBuilder
from tableau_migration.migration_engine_hooks_transformers import PyContentTransformerBuilder
from tableau_migration.migration_engine_options import PyMigrationPlanOptionsBuilder

from tableau_migration.migration_engine_options import (
    PyMigrationPlanOptionsCollection,
)

from Tableau.Migration import (
    IMigrationPlanBuilder, 
    IMigrationPlan, 
    IServerToCloudMigrationPlanBuilder)

from Tableau.Migration.Engine.Endpoints import(
    IMigrationPlanEndpointConfiguration)
from System import Uri

class PyMigrationPlan():
    """Default IMigrationPlan implementation."""
    
    _dotnet_base = IMigrationPlan

    def __init__(self, migration_plan: IMigrationPlan) -> None:
        """Default init.

        Args:
            migration_plan: An object that describes how to perform a migration of Tableau data between sites.
        
        Returns: None.
        """
        self._migration_plan = migration_plan

    
    @property
    def plan_id(self) -> UUID:
        """Gets the per-plan options to supply."""
        return UUID(self._migration_plan.PlanId.ToString())

    
    @property
    def pipeline_profile(self):
        """Gets the profile of the pipeline that will be built and executed."""
        return self._migration_plan.PipelineProfile # TODO: PipelineProfile needs python wrapper


    @property
    def options(self):
        """Gets the per-plan options."""
        return PyMigrationPlanOptionsCollection(self._migration_plan.Options)

    @property
    def source(self):
        """Gets the defined source endpoint configuration."""
        return self._migration_plan.Source # TODO: IMigrationPlanEndpointConfiguration needs python wrapper

    @property
    def destination(self):
        """Gets the defined destination endpoint configuration."""
        return self._migration_plan.Destination # TODO: IMigrationPlanEndpointConfiguration needs python wrapper

    @property
    def hooks(self):
        """Gets the collection of registered hooks for each hook type."""
        return PyMigrationPlanOptionsCollection(self._migration_plan.Hooks)
    
    @property
    def mappings(self):
        """Gets the collection of registered mappings for each content type."""
        return PyMigrationPlanOptionsCollection(self._migration_plan.Mappings)
    
    @property
    def filters(self):
        """Gets the collection of registered filters for each content type."""
        return PyMigrationPlanOptionsCollection(self._migration_plan.Filters)
    
    @property
    def transformers(self):
        """Gets the collection of registered transformers for each content type."""
        return PyMigrationPlanOptionsCollection(self._migration_plan.Transformers)
 
class PyServerToCloudMigrationPlanBuilder():
    """Default IServerToCloudMigrationPlanBuilder implementation."""

    _dotnet_base = IServerToCloudMigrationPlanBuilder

    def __init__(self, _plan_builder: IServerToCloudMigrationPlanBuilder) -> None:
        """Default init.

        Args:
            _plan_builder: An object that can build IMigrationPlan objects that migrate content from Tableau Server to Tableau Cloud.
        
        Returns: None.
        """
        self._plan_builder = _plan_builder
        self._hooks = PyMigrationHookBuilder(_plan_builder.Hooks)
        self._filters = PyContentFilterBuilder(_plan_builder.Filters)
        self._mappings = PyContentMappingBuilder(_plan_builder.Mappings)
        self._transformers = PyContentTransformerBuilder(_plan_builder.Transformers)
        self._options = PyMigrationPlanOptionsBuilder(_plan_builder.Options)

    @property
    def filters(self) -> PyContentFilterBuilder:
        """Gets the filters to execute at various points during the migration."""
        return self._filters

    @property
    def transformers(self) -> PyContentTransformerBuilder:
        """Gets the transformations to execute at various points during the migration."""
        return self._transformers

    @property
    def options(self) -> PyMigrationPlanOptionsBuilder:
        """Gets the per-plan options to supply."""
        return self._options

    @property
    def hooks(self) -> PyMigrationHookBuilder:
        """Gets the hooks to execute at various points during the migration, determined by hook type."""
        return self._hooks

    @property
    def mappings(self) -> PyContentMappingBuilder:
        """Gets the mappings to execute at various points during the migration."""
        return self._mappings

    def from_source_tableau_server(self, server_url: str, site_content_url: str, access_token_name: str, access_token: str, create_api_simulator: bool = False) -> Self:
        """Sets or overwrites the configuration for the source Tableau Server site to migrate content from.

        Args:
            server_url: The base URL of the Tableau Server to connect to.
            site_content_url: The URL namespace of the site to connect to.
            access_token_name: The name of the personal access token to use to sign into the site.
            access_token: The personal access token to use to sign into the site.
            create_api_simulator: Whether or not to create an API simulator for the server_url.
    
        Returns: The same plan builder object for fluent API calls.
        """
        self._plan_builder.FromSourceTableauServer(Uri(server_url), site_content_url, access_token_name, access_token, create_api_simulator)
        return self

    def to_destination_tableau_cloud(self, pod_url: str, site_content_url: str, access_token_name: str, access_token: str, create_api_simulator: bool = False) -> Self:
        """Sets or overwrites the configuration for the destination Tableau Cloud site to migrate content to.

        Args:
            pod_url: The base URL of Tableau Cloud pod to connect to.
            site_content_url: The URL namespace of the site to connect to.
            access_token_name: The name of the personal access token to use to sign into the site.
            access_token: The personal access token to use to sign into the site.
            create_api_simulator: Whether or not to create an API simulator for the server_url.
        
        Returns: The same plan builder object for fluent API calls.
        """
        self._plan_builder.ToDestinationTableauCloud(Uri(pod_url), site_content_url, access_token_name, access_token, create_api_simulator)
        return self

    def for_server_to_cloud(self) -> Self:
        """Initializes the plan to perform a migration of content between a Tableau Server and Tableau Cloud site.

        Returns: The same plan builder object for fluent API calls.
        """
        return self._plan_builder.ForServerToCloud()

    def build(self) -> PyMigrationPlan:
        """Finalizes the IMigrationPlan based on the current state.

        Returns: The created IMigrationPlan.
        """
        return PyMigrationPlan(self._plan_builder.Build())

    def clear_extensions(self) -> Self:
        """Clears all hooks, filters, mappings, and transformations.

        Returns: The same plan builder object for fluent API calls.
        """
        self._plan_builder.ClearExtensions()
        return self

    def append_default_extensions(self) -> Self:
        """Adds default hooks, filters, etc. that are common between all migration scenarios.

        Returns: The same plan builder object for fluent API calls.
        """
        self._plan_builder.AppendDefaultExtensions()
        return self

    def from_source(self, config: IMigrationPlanEndpointConfiguration) -> Self:
        """Sets or overwrites the configuration for the source endpoint to migrate content from.

        Args:
            config: The endpoint configuration.

        Returns: The same plan builder object for fluent API calls.
        """
        self._plan_builder.FromSource(config)
        return self

    def to_destination(self, config: IMigrationPlanEndpointConfiguration) -> Self:
        """Sets or overwrites the configuration for the destination endpoint to migrate content to.

        Args:
           config: The endpoint configuration.

        Returns: The same plan builder object for fluent API calls.
        """
        self._plan_builder.ToDestination(config)
        return self

    def validate(self) -> PyResult:
        """Validates that the plan that would be built has enough information to execute.

        Returns: The validation result.
        """
        return PyResult(self._plan_builder.Validate())

    def append_default_server_to_cloud_extensions(self) -> Self:
        """Appends default hooks, filters, mappings, and transformations for server-to-cloud migrations.
        
        This method is intended for upgrading existing plan builders - 
        new plan builders should use <see cref="IMigrationPlanBuilder.ForServerToCloud"/> instead.

        Returns: The same plan builder object for fluent API calls.
        """
        self._plan_builder.AppendDefaultServerToCloudExtensions()
        return self

    def with_saml_authentication_type(self, domain: str) -> Self:
        """Adds an object to map user and group domains based on the SAML authentication type.

        Args:
            domain: The domain to map users and groups to.

        Returns: The same plan builder object for fluent API calls.
        """
        self._plan_builder.WithSamlAuthenticationType(domain)
        return self

    def with_tableau_id_authentication_type(self, mfa: bool = True) -> Self:
        """Adds an object to map user and group domains based on the Tableau ID authentication type.

        Args:
            mfa: Whether or not MFA is used, defaults to true.

        Returns: The same plan builder object for fluent API calls.
        """
        self._plan_builder.WithTableauIdAuthenticationType(mfa)
        return self
    
    def with_authentication_type(self, authentication_type: str, input_1, group_domain) -> Self:
        """Adds an object to map user and group domains based on the destination authentication type.

        Args:
            authentication_type: An authentication type to assign to users.
            input_1: Either 1) the domain to map users to, or 2) the mapping to execute.
            group_domain: The domain to map users to. Not used with input_1 in a mapping object.

        Returns: The same plan builder object for fluent API calls.
        """
        if isinstance(input_1, str):
            self._plan_builder.WithAuthenticationType(authentication_type, input_1, group_domain)
        else:
            self._plan_builder.WithAuthenticationType(authentication_type, input_1)
        return self

    def with_tableau_cloud_usernames(self, input_0, use_existing_email: bool = True) -> Self:
        """Adds an object to map usernames to be in the form of an email.

        Args:
            input_0: Either 1) A domain name to use to build email usernames for users that lack emails, generated as "{username}@{input_0}", or 2) the mapping to execute.
            use_existing_email: Whether or not existing user emails should be used when available, defaults to true. Not used when input_0 is a mapping object.

        Returns: The same plan builder object for fluent API calls.
        """
        if isinstance(input_0, str):
            self._plan_builder.WithTableauCloudUsernames(input_0, use_existing_email)
        else:
            self._plan_builder.WithTableauCloudUsernames(input_0)
        return self

class PyMigrationPlanBuilder():
    """Default IMigrationPlanBuilder implementation."""
    
    _dotnet_base = IMigrationPlanBuilder

    def __init__(self) -> None:
        """Default init.
        
        Returns: None.
        """
        self._services = tableau_migration.migration.get_service_provider()
        self._plan_builder = tableau_migration.migration.get_service(self._services, IMigrationPlanBuilder)
        self._hooks = PyMigrationHookBuilder(self._plan_builder.Hooks)
        self._filters = PyContentFilterBuilder(self._plan_builder.Filters)
        self._mappings = PyContentMappingBuilder(self._plan_builder.Mappings)
        self._transformers = PyContentTransformerBuilder(self._plan_builder.Transformers)
        self._options = PyMigrationPlanOptionsBuilder(self._plan_builder.Options)

    @property
    def filters(self) -> PyContentFilterBuilder:
        """Gets the filters to execute at various points during the migration."""
        return self._filters

    @property
    def transformers(self) -> PyContentTransformerBuilder:
        """Gets the transformations to execute at various points during the migration."""
        return self._transformers

    @property
    def options(self) -> PyMigrationPlanOptionsBuilder:
        """Gets the per-plan options to supply."""
        return self._options

    @property
    def hooks(self) -> PyMigrationHookBuilder:
        """Gets the hooks to execute at various points during the migration, determined by hook type."""
        return self._hooks

    @property
    def mappings(self) -> PyContentMappingBuilder:
        """Gets the mappings to execute at various points during the migration."""
        return self._mappings

    def from_source_tableau_server(self, server_url: str, site_content_url: str, access_token_name: str, access_token: str, create_api_simulator: bool = False) -> Self:
        """Sets or overwrites the configuration for the source Tableau Server site to migrate content from.

        Args:
            server_url: The base URL of the Tableau Server to connect to.
            site_content_url: The URL namespace of the site to connect to.
            access_token_name: The name of the personal access token to use to sign into the site.
            access_token: The personal access token to use to sign into the site.
            create_api_simulator: Whether or not to create an API simulator for the server_url.
    
        Returns: The same plan builder object for fluent API calls.
        """
        self._plan_builder.FromSourceTableauServer(Uri(server_url), site_content_url, access_token_name, access_token, create_api_simulator)
        return self

    def to_destination_tableau_cloud(self, pod_url: str, site_content_url: str, access_token_name: str, access_token: str, create_api_simulator: bool = False) -> Self:
        """Sets or overwrites the configuration for the destination Tableau Cloud site to migrate content to.

        Args:
            pod_url: The base URL of Tableau Cloud pod to connect to.
            site_content_url: The URL namespace of the site to connect to.
            access_token_name: The name of the personal access token to use to sign into the site.
            access_token: The personal access token to use to sign into the site.
            create_api_simulator: Whether or not to create an API simulator for the server_url.
        
        Returns: The same plan builder object for fluent API calls.
        """
        self._plan_builder.ToDestinationTableauCloud(Uri(pod_url), site_content_url, access_token_name, access_token, create_api_simulator)
        return self

    def for_server_to_cloud(self) -> PyServerToCloudMigrationPlanBuilder:
        """Initializes the plan to perform a migration of content between a Tableau Server and Tableau Cloud site.

        Returns: The same plan builder object for fluent API calls.
        """
        return PyServerToCloudMigrationPlanBuilder(self._plan_builder.ForServerToCloud())

    def build(self) -> PyMigrationPlan:
        """Finalizes the IMigrationPlan based on the current state.

        Returns: The created IMigrationPlan.
        """
        return PyMigrationPlan(self._plan_builder.Build())

    def clear_extensions(self) -> Self:
        """Clears all hooks, filters, mappings, and transformations.

        Returns: The same plan builder object for fluent API calls.
        """
        self._plan_builder.ClearExtensions()
        return self

    def append_default_extensions(self) -> Self:
        """Adds default hooks, filters, etc. that are common between all migration scenarios.

        Returns: The same plan builder object for fluent API calls.
        """
        self._plan_builder.AppendDefaultExtensions()
        return self

    def from_source(self, config: IMigrationPlanEndpointConfiguration) -> Self:
        """Sets or overwrites the configuration for the source endpoint to migrate content from.

        Args:
            config: The endpoint configuration.

        Returns: The same plan builder object for fluent API calls.
        """
        self._plan_builder.FromSource(config)
        return self

    def to_destination(self, config: IMigrationPlanEndpointConfiguration) -> Self:
        """Sets or overwrites the configuration for the destination endpoint to migrate content to.

        Args:
           config: The endpoint configuration.

        Returns: The same plan builder object for fluent API calls.
        """
        self._plan_builder.ToDestination(config)
        return self

    def validate(self) -> PyResult:
        """Validates that the plan that would be built has enough information to execute.

        Returns: The validation result.
        """
        return PyResult(self._plan_builder.Validate())
