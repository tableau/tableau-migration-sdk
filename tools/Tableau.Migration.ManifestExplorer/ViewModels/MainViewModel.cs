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
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using ReactiveUI;
using Tableau.Migration.Engine.Manifest;

namespace Tableau.Migration.ManifestExplorer.ViewModels
{
    public class MainViewModel : ViewModelBase, IShowExceptionListDialogViewModel
    {
        private readonly MigrationManifestSerializer _serializer;

        public ManifestViewModel? ManifestViewModel
        {
            get => _manifestViewModel;
            set => this.RaiseAndSetIfChanged(ref _manifestViewModel, value);
        }
        private ManifestViewModel? _manifestViewModel;

        public Interaction<MainViewModel, IStorageFile?> OpenFileDialog { get; }

        public Interaction<IReadOnlyList<Exception>, Unit> ShowExceptionListDialog { get; }

        public ReactiveCommand<Unit, Unit> LoadManifestCommand { get; }

        public ReactiveCommand<IReadOnlyList<Exception>, Unit> ShowExceptionListDialogCommand { get; }

        private bool _isManifestViewVisible;
        public bool IsManifestViewVisible
        {
            get => _isManifestViewVisible;
            set
            {
                this.RaiseAndSetIfChanged(ref _isManifestViewVisible, value);
                this.RaisePropertyChanged(nameof(IsOverlayVisible));
            }
        }

        // Reverse of IsManifestViewVisible for overlay visibility
        public bool IsOverlayVisible => !IsManifestViewVisible;

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => this.RaiseAndSetIfChanged(ref _isLoading, value);
        }

        public MainViewModel(MigrationManifestSerializer serializer)
        {
            _serializer = serializer;

            OpenFileDialog = new();
            ShowExceptionListDialog = new();

            LoadManifestCommand = ReactiveCommand.CreateFromTask(() => LoadManifestAsync());
            ShowExceptionListDialogCommand = ReactiveCommand.CreateFromTask<IReadOnlyList<Exception>>(ShowExceptionListDialogAsync);

            // Initialize the visibility property
            IsManifestViewVisible = false;
            IsLoading = false;
        }

        public async void Initialize(string[] args)
        {
            if (args.Length > 0 && !string.IsNullOrEmpty(args[0]))
            {
                await LoadManifestAsync(args[0]).ConfigureAwait(false);
            }
        }

        private async Task LoadManifestAsync(string? filePath = null)
        {
            try
            {
                IsLoading = true;

                if (string.IsNullOrEmpty(filePath))
                {
                    var file = await OpenFileDialog.Handle(this);
                    if (file is null)
                    {
                        IsLoading = false;
                        return;
                    }

                    filePath = file.Path.LocalPath;
                }

                using var fileStream = File.OpenRead(filePath);
                var newManifest = await _serializer.LoadAsync(fileStream, CancellationToken.None)
                    .ConfigureAwait(false);

                if (newManifest is null)
                {
                    IsLoading = false;
                    return;
                }

                ManifestViewModel = new(newManifest, this);
                IsManifestViewVisible = true; // Set visibility to true when a new manifest is loaded
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., file not found, invalid format)
                await ShowExceptionListDialog.Handle(new List<Exception> { ex });
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task ShowExceptionListDialogAsync(IReadOnlyList<Exception> exceptionList)
        {
            await ShowExceptionListDialog.Handle(exceptionList);
        }
    }
}