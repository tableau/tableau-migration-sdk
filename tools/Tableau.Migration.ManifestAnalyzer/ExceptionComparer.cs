//
//  Copyright (c) 2025, Salesforce, Inc.
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

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Tableau.Migration.ManifestAnalyzer
{
    public class ExceptionComparer : IEqualityComparer<Exception>
    {
        private static readonly List<Regex> RegexPatterns = new List<Regex>
        {
            // Matches GUIDs in the format 8-4-4-4-12 (e.g., 123e4567-e89b-12d3-a456-426614174000)
            new Regex(@"\b[0-9a-fA-F]{8}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{12}\b", RegexOptions.Compiled),
            
            // Matches upload session IDs in the format uploadSessionId= followed by alphanumeric characters, %, :, or -
            new Regex(@"uploadSessionId=[0-9a-fA-F%:-]+", RegexOptions.Compiled),
            
            // Matches identifiers in the format digits:hexadecimal-0:0 (e.g., 4629:4448bd27de134820bf46a12fd09d4df5-0:0)
            new Regex(@"\b\d+:[0-9a-fA-F]{32}-0:0\b", RegexOptions.Compiled),
            
            // Matches datasource names in the format 'DatasourceName' with start anchor 'Datasource' and end anchor ' not found'
            // Message example: There was a problem publishing the file '15575:b3b652dc9b544784b0896b26deecf3f9-0:0'.. (0x9F4C1551 : com.tableausoftware.domain.exception.PublishingException: Datasource 'AssetsSnapshotUSL' not found for workbook 'Updatable License : Phase 1 Mer Edits'.)
            new Regex(@"(?i)(?<=Datasource\s').*?(?='\snot\sfound)", RegexOptions.Compiled),
            
            // Matches workbook names in the format 'WorkbookName' with start anchor 'workbook' and end anchor '.)'
            // Message example: There was a problem publishing the file '15575:b3b652dc9b544784b0896b26deecf3f9-0:0'.. (0x9F4C1551 : com.tableausoftware.domain.exception.PublishingException: Datasource 'AssetsSnapshotUSL' not found for workbook 'Updatable License : Phase 1 Mer Edits'.)
            new Regex(@"(?i)(?<=workbook\s').*?(?='\.\))", RegexOptions.Compiled),
            
            // Matches the repeated message about .tde files being deprecated
            new Regex(@"Live and extract connection with \.tde files have been deprecated\. To upgrade \.tde files to \.hyper, please follow instructions at https:\/\/community\.tableau\.com\/s\/feed\/0D58b0000CQKAUbCQP\.\s*", RegexOptions.Compiled),

            // Matches file paths in PublishingException messages
            // Message example: There was a problem publishing the file '29315:4c2461aa36eb4f5bb491bf88e8913741-0:0'.. (0xD30BACC6 : com.tableausoftware.domain.exception.PublishingException: File Data/Feb2020_ImpactAnalysis/TABLEAU MODELS CRM LEAD SCORES_2022-02-03.csv is too large. Files larger than 1,024 MB decompressed size are not permitted. Please create an extract to proceed with publishing.)
            new Regex(@"(?<=com\.tableausoftware\.domain\.exception\.PublishingException:).*?(?=\sis\stoo\slarge\.)", RegexOptions.Compiled),

            // Matches view names in the format 'ViewName' with start anchor 'View with name ' and end anchor ', ownerId'
            // Message example: Detail: Customized View with name 'My Account Engagement', ownerId 'f35f7db7-e18e-4a47-b137-e3a0e0276b27', and viewId '8a2098ed-ff6c-4e04-8f00-f6e56e3dc4b7' already exists.
            new Regex(@"(?<=View\swith\sname\s').*?(?=',\sownerId)", RegexOptions.Compiled)
        };

        public bool Equals(Exception? x, Exception? y)
        {
            if (x is null || y is null)
                return x == y;

            return x.GetType() == y.GetType() &&
                   RemoveIdentifiers(x.Message) == RemoveIdentifiers(y.Message);
        }

        public int GetHashCode(Exception obj)
        {
            if (obj is null)
                return 0;

            return obj.GetType().GetHashCode() ^ RemoveIdentifiers(obj.Message).GetHashCode();
        }

        private string RemoveIdentifiers(string message)
        {
            foreach (var regex in RegexPatterns)
            {
                message = regex.Replace(message, string.Empty);
            }
            return message;
        }
    }
}

