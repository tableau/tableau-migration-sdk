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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using ReactiveUI;
using Tableau.Migration.Engine.Manifest;

namespace Tableau.Migration.ManifestExplorer.ViewModels
{
    public class ManifestEntryPageViewModel : ViewModelBase, INotifyPropertyChanged
    {
        #region - Properties -
        public IShowExceptionListDialogViewModel ShowExceptionList { get; }

        private readonly IReadOnlyCollection<IMigrationManifestEntry> _entries;
        private ObservableCollection<ManifestEntryViewModel> _shownEntries;

        private CancellationTokenSource _searchCancellationTokenSource = new();
        public ObservableCollection<ManifestEntryViewModel> ShownEntries
        {
            get => _shownEntries;
            set
            {
                _shownEntries = value;
                OnPropertyChanged(nameof(ShownEntries));
            }
        }

        //public int TotalCount { get; }
        #endregion

        public ManifestEntryPageViewModel(IReadOnlyCollection<IMigrationManifestEntry> entries, IShowExceptionListDialogViewModel showExceptionList)
        {
            _entries = entries;
            ShowExceptionList = showExceptionList;

            _shownEntries = new ObservableCollection<ManifestEntryViewModel>(_entries.Select(e => new ManifestEntryViewModel(e)));
        }

        public async Task SearchAsync(string searchText, bool errorsOnly, CancellationToken cancel)
        {
            // Cancel the previous search if it is still running
            _searchCancellationTokenSource.Cancel();
            _searchCancellationTokenSource = new CancellationTokenSource();
            var searchToken = _searchCancellationTokenSource.Token;

            // Create a linked token that combines the provided token and the search-specific token
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancel, searchToken);
            var linkedToken = linkedCts.Token;

            if (string.IsNullOrWhiteSpace(searchText) && !errorsOnly)
            {
                ShownEntries = new ObservableCollection<ManifestEntryViewModel>(_entries.Select(e => new ManifestEntryViewModel(e)));
                return;
            }

            var filteredEntries = await Task.Run(() =>
            {
                return _entries.Where(e =>
                    (!errorsOnly || e.Errors.Any()) &&

                    (string.IsNullOrWhiteSpace(searchText) ||
                        e.Source.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                        e.Source.Id.ToString().Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                        e.Source.Location.Path.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||

                        (e.Destination != null &&
                            (e.Destination.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                            e.Destination.Id.ToString().Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                            e.Destination.Location.Path.Contains(searchText, StringComparison.OrdinalIgnoreCase))) ||

                        e.MappedLocation.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                        e.MappedLocation.Path.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||

                        e.Errors.Any(err => err.Message.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                    )
                ).Select(e => new ManifestEntryViewModel(e)).ToList();
            }, linkedToken).ConfigureAwait(false);

            if (!linkedToken.IsCancellationRequested)
            {
                ShownEntries = new ObservableCollection<ManifestEntryViewModel>(filteredEntries);
            }
        }

        #region - Event Handlers -

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            this.RaisePropertyChanged(propertyName);
        }

        #endregion
    }

}
