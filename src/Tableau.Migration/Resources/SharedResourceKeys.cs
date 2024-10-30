//
//  Copyright (c) 2024, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the "License") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

namespace Tableau.Migration.Resources
{
    internal static class SharedResourceKeys
    {
        public const string ApiClientInputNotInitializedError = "ApiClientInputNotInitializedError";

        public const string ProductName = "ProductName";

        public const string EndpointInitializationError = "EndpointInitializationError";

        public const string PreviousManifestPlanMismatchWarning = "PreviousManifestPlanMismatchWarning";

        public const string NetworkTraceLogMessage = "NetworkTraceLogMessage";

        public const string NetworkTraceExceptionLogMessage = "NetworkTraceExceptionLogMessage";

        public const string NetworkTraceTooLargeDetails = "NetworkTraceTooLargeDetails";

        public const string SectionRequestHeaders = "SectionRequestHeaders";

        public const string SectionResponseHeaders = "SectionResponseHeaders";

        public const string SectionRequestContent = "SectionRequestContent";

        public const string SectionResponseContent = "SectionResponseContent";

        public const string SectionException = "SectionException";

        public const string NetworkTraceNotDisplayedDetails = "NetworkTraceNotDisplayedDetails";

        public const string AuthenticationTypeDomainMappingValidationMessage = "AuthenticationTypeDomainMappingValidationMessage";

        public const string TableauCloudUsernameMappingValidationMessage = "TableauCloudUsernameMappingValidationMessage";

        public const string RestExceptionContent = "RestExceptionContent";

        public const string NullValue = "NullValue";

        public const string UnknownFilterContentTypeValidationMessage = "UnknownFilterContentTypeValidationMessage";

        public const string UnknownMappingContentTypeValidationMessage = "UnknownMappingContentTypeValidationMessage";

        public const string UnknownTransformerContentTypeValidationMessage = "UnknownTransformerContentTypeValidationMessage";

        public const string InvalidTransformerPublishTypeValidationMessage = "InvalidTransformerPublishTypeValidationMessage";

        public const string UnknownTransformerPublishTypeValidationMessage = "UnknownTransformerPublishTypeValidationMessage";

        public const string FileEncryptionDisabledLogMessage = "FileEncryptionDisabledLogMessage";

        public const string MigrationErrorLogMessage = "MigrationErrorLogMessage";

        public const string MigrationItemErrorLogMessage = "MigrationItemErrorLogMessage";

        public const string SourceUserNotFoundLogMessage = "SourceUserNotFoundLogMessage";

        public const string SourceWorkbookNotFoundLogMessage = "SourceWorkbookNotFoundLogMessage";

        public const string PublishedDataSourceReferenceNotFoundLogMessage = "PublishedDataSourceReferenceNotFoundLogMessage";

        public const string FailedJobExceptionContent = "FailedJobExceptionContent";

        public const string TimeoutJobExceptionMessage = "TimeoutJobExceptionMessage";

        public const string ContentFilterBaseDebugMessage = "ContentFilterBaseDebugMessage";

        public const string ContentMappingBaseDebugMessage = "ContentMappingBaseDebugMessage";

        public const string ContentTransformerBaseDebugMessage = "ContentTransformerBaseDebugMessage";

        public const string GroupUsersTransformerCannotAddUserWarning = "GroupUsersTransformerCannotAddUserWarning";

        public const string CustomViewDefaultUsersTransformerNoUserRefsDebugMessage = "CustomViewDefaultUsersTransformerNoUserRefsDebugMessage";

        public const string PermissionsTransformerGranteeNotFoundWarning = "PermissionsTransformerGranteeNotFoundWarning";

        public const string SiteSettingsSkippedDisabledLogMessage = "SiteSettingsSkippedDisabledLogMessage";

        public const string SiteSettingsSkippedNoAccessLogMessage = "SiteSettingsSkippedNoAccessLogMessage";

        public const string ApiClientDoesnotImplementIReadApiClientError = "ApiClientDoesnotImplementIReadApiClientError";

        public const string ApiEndpointNotInitializedError = "ApiEndpointNotInitializedError";

        public const string ApiEndpointDoesnotHaveValidSiteError = "ApiEndpointDoesnotHaveValidSiteError";

        public const string ProjectReferenceNotFoundMessage = "ProjectReferenceNotFoundMessage";

        public const string ProjectReferenceNotFoundException = "ProjectReferenceNotFoundException";

        public const string OwnerNotFoundMessage = "OwnerNotFoundMessage";

        public const string OwnerNotFoundException = "OwnerNotFoundException";

        public const string WorkbookReferenceNotFoundMessage = "WorkbookReferenceNotFoundMessage";

        public const string WorkbookReferenceNotFoundException = "WorkbookReferenceNotFoundException";

        public const string ViewReferenceNotFoundMessage = "ViewReferenceNotFoundMessage";

        public const string ViewReferenceNotFoundException = "ViewReferenceNotFoundException";

        public const string UserReferenceNotFoundMessage = "UserReferenceNotFoundMessage";

        public const string UserReferenceNotFoundException = "UserReferenceNotFoundException";

        public const string FailedToGetDefaultPermissionsMessage = "FailedToGetDefaultPermissionsMessage";

        public const string TableauInstanceTypeNotSupportedMessage = "TableauInstanceTypeNotSupportedMessage";

        public const string MappedReferenceExtractRefreshTaskTransformerCannotFindReferenceWarning = "MappedReferenceExtractRefreshTaskTransformerCannotFindReferenceWarning";

        public const string IntervalsChangedWarning = "IntervalsChangedWarning";

        public const string IntervalNotChangedDebugMessage = "IntervalNotChangedDebugMessage";

        public const string Found = "Found";

        public const string NotFound = "NotFound";

        public const string CustomViewSkippedMissingReferenceWarning = "CustomViewSkippedMissingReferenceWarning";

        public const string DataSourceSkippedMissingReferenceWarning = "DataSourceSkippedMissingReferenceWarning";

        public const string WorkbookSkippedMissingReferenceWarning = "WorkbookSkippedMissingReferenceWarning";

        public const string UserWithCustomViewDefaultSkippedMissingReferenceWarning = "UserWithCustomViewDefaultSkippedMissingReferenceWarning";

        public const string DuplicateContentTypeConfigurationMessage = "DuplicateContentTypeConfigurationMessage";

        public const string UnknownExtractRefreshContentTypeWarning = "UnknownExtractRefreshContentTypeWarning";

        public const string FrequencyNotSetError = "FrequencyNotSetError";

        public const string FrequencyNotSupportedError = "FrequencyNotSupportedError";

        public const string InvalidScheduleError = "InvalidScheduleError";

        public const string AtLeastOneIntervalError = "AtLeastOneIntervalError";

        public const string AtLeastOneIntervalWithHourOrMinutesError = "AtLeastOneIntervalWithHourOrMinutesError";

        public const string BothHoursAndMinutesIntervalError = "BothHoursAndMinutesIntervalError";

        public const string InvalidHourlyIntervalForServerError = "InvalidHourlyIntervalForServerError";

        public const string InvalidHourlyIntervalForCloudError = "InvalidHourlyIntervalForCloudError";

        public const string InvalidMinuteIntervalError = "InvalidMinuteIntervalError";

        public const string InvalidMinuteIntervalWarning = "InvalidMinuteIntervalWarning";

        public const string IntervalsIgnoredWarning = "IntervalsIgnoredWarning";

        public const string WeeklyScheduleIntervalError = "WeeklyScheduleIntervalError";

        public const string InvalidWeekdayError = "InvalidWeekdayError";

        public const string ScheduleMustHaveStartAtTimeError = "ScheduleMustHaveStartAtTimeError";

        public const string ScheduleMustHaveEndAtTimeError = "ScheduleMustHaveEndAtTimeError";

        public const string ScheduleMustNotHaveEndAtTimeError = "ScheduleMustNotHaveEndAtTimeError";

        public const string InvalidMonthDayError = "InvalidMonthDayError";

        public const string FrequencyNotExpectedError = "FrequencyNotExpectedError";

        public const string AtLeastOneValidWeekdayError = "AtLeastOneValidWeekdayError";

        public const string AtLeastOneValidMonthDayError = "AtLeastOneValidMonthDayError";

        public const string ScheduleShouldOnlyHaveOneHoursIntervalWarning = "ScheduleShouldOnlyHaveOneHoursIntervalWarning";

        public const string ScheduleMustHaveExactlyOneWeekdayIntervalError = "ScheduleMustHaveExactlyOneWeekdayIntervalError";

        public const string ScheduleMustOnlyHaveOneIntervalWithLastDayError = "ScheduleMustOnlyHaveOneIntervalWithLastDayError";

        public const string InvalidScheduleForMonthlyError = "InvalidScheduleForMonthlyError";

        public const string ExactlyOneHourOrMinutesError = "ExactlyOneHourOrMinutesError";

        public const string IntervalMustBe1HourOr60MinError = "IntervalMustBe1HourOr60MinError";

        public const string StartEndTimeDifferenceError = "StartEndTimeDifferenceError";

        public const string ReplacingHourlyIntervalMessage = "ReplacingHourlyIntevalMessage";

        public const string ScheduleUpdateFailedError = "ScheduleUpdateFailedError";

        public const string ScheduleUpdatedMessage = "ScheduleUpdatedMessage";

        public const string ScheduleUpdatedFrequencyToDailyMessage = "ScheduleUpdatedFrequencyToDailyMessage";

        public const string ScheduleUpdatedAddedWeekdayMessage = "ScheduleUpdatedAddedWeekdayMessage";

        public const string ScheduleUpdatedAddedEndAtMessage = "ScheduleUpdatedAddedEndAtMessage";

        public const string ScheduleUpdatedRemovedEndAtMessage = "ScheduleUpdatedRemovedEndAtMessage";

        public const string ScheduleUpdatedHoursMessage = "ScheduleUpdatedHoursMessage";
    }
}
