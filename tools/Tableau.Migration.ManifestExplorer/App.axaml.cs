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
using System.Threading;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Tableau.Migration.ManifestExplorer.ViewModels;
using Tableau.Migration.ManifestExplorer.Views;

namespace Tableau.Migration.ManifestExplorer
{
    public partial class App : Application
    {
        private static readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        public static CancellationToken AppCancellationToken => _cancellationTokenSource.Token;

        public string[] CommandLineArgs { get; private set; } = Array.Empty<string>();

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void Initialize(string[] args)
        {
            CommandLineArgs = args;
            Initialize();
        }

        private IServiceProvider BuildServices()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddTableauMigrationSdk();

            serviceCollection.AddTransient<MainViewModel>();

            return serviceCollection.BuildServiceProvider();
        }

        public override void OnFrameworkInitializationCompleted()
        {
            var services = BuildServices();

            var mainData = services.GetRequiredService<MainViewModel>();
            mainData.Initialize(CommandLineArgs); // Pass the command line arguments to the MainViewModel

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = mainData
                };
                desktop.Exit += OnExit;
            }
            else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
            {
                singleViewPlatform.MainView = new MainView
                {
                    DataContext = mainData
                };
                singleViewPlatform.MainView.DetachedFromVisualTree += OnExit;
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void OnExit(object? sender, EventArgs e)
        {
            _cancellationTokenSource.Cancel();
        }
    }
}