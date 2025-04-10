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

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using Tableau.Migration;
using Tableau.Migration.Api;
using Tableau.Migration.Content;

var config = new ConfigurationBuilder()
    .AddInMemoryCollection(new Dictionary<string, string?>
    {
        { "Files:RootPath", @"C:\Temp\filestore"},
        { "Network:HeadersLoggingEnabled", "true" },
        { "Network:ContentLoggingEnabled", "true" },
        { "Network:BinaryContentLoggingEnabled", "true" }
    })
    // Copy the clean-site-settings.json to clean-site-settings.dev.json and fill in.
    .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "clean-site-settings.dev.json"))
    .Build();

var serviceCollection = new ServiceCollection()
    //.AddLogging(b => b.AddConsole())
    .AddTableauMigrationSdk(config);

var services = serviceCollection.BuildServiceProvider();

var connectionConfig = config.GetSection("ConnectionConfig").Get<TableauSiteConnectionConfiguration>();

var apiClient = services.GetRequiredService<IScopedApiClientFactory>()
                    .Initialize(connectionConfig);

var signIn = await apiClient.SignInAsync(default);

if (!signIn.Success)
{
    foreach (var e in signIn.Errors)
        Console.WriteLine(e);

    // If the above error isn't enough, uncomment the .AddLogging during when newing the ServiceCollection
}

var siteClient = signIn.Value!;

// Asks for confirmation. Site name is in purple.
Console.WriteLine($"Are you sure you want to delete all projects, groups, and users on the site \u001b[35m{connectionConfig.SiteContentUrl}\u001b[0m? (y/N)");
ConsoleKeyInfo response = Console.ReadKey();

if (!response.KeyChar.ToString().Equals("y", StringComparison.CurrentCultureIgnoreCase))
{
    Console.WriteLine("\nOperation canceled.\n");
    return;
}

Console.WriteLine("\nPreparing to delete projects...");

await DeleteProjectsAsync();
await DeleteGroupsAsync();
await DeleteUsersAsync();

#region - Functions - 

async Task DeleteProjectsAsync()
{
    var projects = await siteClient.Projects.GetAllAsync(100, default).ConfigureAwait(false);

    foreach (var proj in projects.Value!)
    {
        Console.WriteLine($"About to delete project: {proj.Name}");
        await siteClient.Projects.DeleteProjectAsync(proj.Id, default).ConfigureAwait(false);
    }
}

async Task DeleteGroupsAsync()
{
    var groups = await siteClient.Groups.GetAllAsync(100, default).ConfigureAwait(false);

    foreach (var group in groups.Value!)
    {
        Console.WriteLine($"About to delete group: {group.Name}");
        await siteClient.Groups.DeleteGroupAsync(group.Id, default).ConfigureAwait(false);
    }
}

async Task DeleteUsersAsync()
{
    var users = await siteClient.Users.GetAllAsync(1000, default);

    var parallelUsers = new ConcurrentBag<IUser>(users.Value!);

    ParallelOptions parallelOptions = new()
    {
        MaxDegreeOfParallelism = 10
    };

    await Parallel.ForEachAsync(parallelUsers, parallelOptions, async (user, cancel) =>
    {
        if (user.AdministratorLevel.Contains("None"))
        {
            Console.WriteLine($"About to delete user: {user.Name}");
            await siteClient.Users.DeleteUserAsync(user.Id, cancel);
        }
    });
}

#endregion