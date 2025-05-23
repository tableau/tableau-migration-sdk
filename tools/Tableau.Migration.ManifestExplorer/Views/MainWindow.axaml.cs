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
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using ReactiveUI;
using Tableau.Migration.ManifestExplorer.ViewModels;

namespace Tableau.Migration.ManifestExplorer.Views
{
    public partial class MainWindow : ReactiveWindow<MainViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();

            this.WhenActivated(action => action(ViewModel!.OpenFileDialog.RegisterHandler(ctx => OpenFileAsync((InteractionContext<MainViewModel, IStorageFile?>)ctx))));
            this.WhenActivated(action => action(ViewModel!.ShowExceptionListDialog.RegisterHandler(ctx => ShowExceptionListDialogAsync((InteractionContext<IReadOnlyList<Exception>, Unit>)ctx))));
        }

        private async Task OpenFileAsync(InteractionContext<MainViewModel, IStorageFile?> ctx)
        {
            if (!StorageProvider.CanOpen)
            {
                ctx.SetOutput(null);
                return;
            }

            var result = await StorageProvider.OpenFilePickerAsync(new()
            {
                AllowMultiple = false
            }).ConfigureAwait(false);

            if (result.IsNullOrEmpty())
            {
                ctx.SetOutput(null);
            }

            ctx.SetOutput(result.First());
        }

        private async Task ShowExceptionListDialogAsync(InteractionContext<IReadOnlyList<Exception>, Unit> ctx)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                var dialog = new ExceptionListDialog
                {
                    DataContext = new ExceptionListDialogViewModel(ctx.Input)
                };

                dialog.Show();
            });

            ctx.SetOutput(Unit.Default);
        }
    }
}
