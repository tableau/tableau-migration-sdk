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

namespace Tableau.Migration.ManifestAnalyzer
{
    internal static class HtmlTemplate
    {
        public const string Template = @"
                    <html>
                    <head>
                        <title>Manifest Analysis Report</title>
                        <style>
                            body {
                                font-family: Arial, sans-serif;
                                margin: 20px;
                                color: #333;
                                padding-top: 110px; /* Add padding to prevent content overlap */
                                overflow-y: scroll; /* Always show vertical scrollbar */
                            }
                            h1 {
                                text-align: center;
                                color: #0056b3;
                            }
                            .header {
                                text-align: center;
                                padding: 10px;
                                color: #0056b3;
                                position: fixed;
                                top: 0;
                                left: 0;
                                width: 100%;
                                background-color: white;
                                z-index: 1000;
                                border-bottom: 1px solid #ddd;
                            }
                            .subheader {
                                text-align: center;
                                font-size: 18px;
                                color: #666;
                            }
                            .summary {
                                margin: 20px 0;
                                padding: 10px;
                                border: 1px solid #ddd;
                                border-radius: 5px;
                                background-color: #f9f9f9;
                            }
                            .summary table {
                                border-collapse: collapse;
                                text-align: left;
                            }
                            .summary th, .summary td {
                                padding: 8px;
                                border: 1px solid #ddd;
                                min-width: 150px; /* Add minimum width to table cells */
                            }
                            .info-icon {
                                display: inline-block;
                                width: 12px;
                                height: 12px;
                                background-color: #0056b3;
                                color: white;
                                text-align: center;
                                border-radius: 50%;
                                font-size: 10px;
                                line-height: 16px;
                                cursor: pointer;
                                position: relative;
                                vertical-align: top;
                                margin-left: 0px;
                            }
                            .tooltip {
                                visibility: hidden;
                                width: 200px;
                                background-color: #555;
                                color: #fff;
                                text-align: center;
                                border-radius: 5px;
                                padding: 5px;
                                position: absolute;
                                z-index: 1;
                                bottom: 125%; /* Position the tooltip above the icon */
                                left: 50%;
                                margin-left: -100px;
                                opacity: 0;
                                transition: opacity 0.3s;
                            }
                            .tooltip::after {
                                content: '';
                                position: absolute;
                                top: 100%;
                                left: 50%;
                                margin-left: -5px;
                                border-width: 5px;
                                border-style: solid;
                                border-color: #555 transparent transparent transparent;
                            }
                            .info-icon:hover .tooltip {
                                visibility: visible;
                                opacity: 1;
                            }
                            .collapsible {
                                background-color: transparent;
                                color: #0056b3;
                                cursor: pointer;
                                padding: 10px;
                                width: 100%;
                                border: 1px solid #0056b3;
                                text-align: left;
                                outline: none;
                                font-size: 15px;
                                margin-bottom: 5px;
                                border-radius: 5px;
                                transition: background-color 0.3s ease, color 0.3s ease;
                            }
                            .collapsible:hover {
                                background-color: #0056b3;
                                color: white;
                            }
                            .content {
                                padding: 0 18px;
                                display: none;
                                overflow: hidden;
                                background-color: #f1f1f1;
                                margin-bottom: 10px;
                                border-radius: 5px;
                                border: 1px solid #ddd;
                            }
                            .error-message {
                                white-space: pre-wrap;
                                background-color: #f4f4f4;
                                border: 1px solid #ddd;
                                padding: 10px;
                                border-radius: 5px;
                                font-family: monospace;
                                color: #d9534f;
                            }
                            .grid-container {
                                display: grid;
                                grid-template-columns: 1fr;
                                gap: 10px;
                            }
                        </style>
                    </head>
                    <body>
                        <div class='header'>
                            <h1>Manifest Analysis Report</h1>
                            <div class='subheader'>{{datetime}}</div>
                        </div>
                        <div class='summary'>
                            <h2>Migration Summary</h2>
                            {{summary}}
                        </div>
                        <div class='grid-container'>
                            {{content}}
                        </div>
                        <script>
                            document.addEventListener('DOMContentLoaded', function() {
                                var coll = document.getElementsByClassName('collapsible');
                                for (var i = 0; i < coll.length; i++) {
                                    coll[i].addEventListener('click', function() {
                                        this.classList.toggle('active');
                                        var content = this.nextElementSibling;
                                        if (content.style.display === 'block') {
                                            content.style.display = 'none';
                                        } else {
                                            content.style.display = 'block';
                                        }
                                    });
                                }
                            });
                        </script>
                    </body>
                    </html>";
    }
}
