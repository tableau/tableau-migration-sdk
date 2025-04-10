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

using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using ReactiveUI;
using Tableau.Migration.Engine.Manifest;

namespace Tableau.Migration.ManifestExplorer.ViewModels
{
    public class ManifestViewModel : ViewModelBase, INotifyPropertyChanged
    {
        #region - Properties - 

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged();
                    _ = UpdateDisplayedContentAsync(App.AppCancellationToken);
                }
            }
        }

        private bool _errorsOnly = false;
        public bool ErrorsOnly
        {
            get => _errorsOnly;
            set
            {
                if (_errorsOnly != value)
                {
                    _errorsOnly = value;
                    OnPropertyChanged();
                    _ = UpdateDisplayedContentAsync(App.AppCancellationToken);
                }
            }
        }



        public ImmutableArray<ManifestPartitionViewModel> Partitions { get; }

        #endregion

        public ManifestViewModel(IShowExceptionListDialogViewModel showExceptionList)
            : this(new MigrationManifest(PipelineProfile.ServerToCloud), showExceptionList) // Defaulting to Server To Cloud
        { }

        public ManifestViewModel(IMigrationManifest manifest, IShowExceptionListDialogViewModel showExceptionList)
        {
            Partitions = manifest.Entries.GetPartitionTypes()
                .Select(t => new ManifestPartitionViewModel(manifest.Entries.ForContentType(t), showExceptionList))
                .ToImmutableArray();
        }

        private async Task UpdateDisplayedContentAsync(CancellationToken cancel)
        {
            var tasks = Partitions.Select(item => item.UpdateEntriesAsync(SearchText, ErrorsOnly, cancel));
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        #region - Event Handlers -

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            this.RaisePropertyChanged(propertyName);
        }

        #endregion
    }
}
