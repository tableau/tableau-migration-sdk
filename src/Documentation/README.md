# Introduction

This is the main documentation generation project. It utilizes `DocFX` to generate a static HTML as well as a hosted documentation site.

## ℹ️ Sources

1. C# API Reference: XML doc comments on public classes, interfaces and methods from the namespace `Tableau.Migration`.
2. Python API Reference: `~\src\Documentation\api-python` folder.
   - These files are generated our doc generation script using `Sphinx`.
   - `DocFX` renders them into the `Python API Reference` section on the docs site.
3. Code Samples: Markdown files under `~\src\Documentation\samples`.
4. Articles: Markdown files under `~\src\Documentation\articles`.

> Images should be placed in the `images` folder to be referenced in the markdown files.

## 📘 Output

Documentation is rendered into the `docs` in the repo as a static HTML site.

## 🚀️ Generation

### Static HTML

1. Run this command from the root of this repo

    ```powershell
    ./scripts/generate-docs.ps1
    ```

2. Go to `~/docs` and open `index.html`

### Local hosted site

1. Run this command from the root of this repo

    ```powershell
    ./scripts/generate-docs.ps1 -serve
    ```

2. Open `http://localhost:8080/` in your browser

## ✍️ Writing

### Location

| Purpose                                                      | Folder                              | Rendered Site Section |
| ---                                                          | ---                                 | ---                   |
| Explain technical concepts                                   | `~\src\Documentation\articles`      | Articles              |
| Write a how-to guide                                         | `~\src\Documentation\articles`      | Articles              |
| Getting Started code                                         | `~\src\Documentation\samples`       | Code Samples          |
| Manually document python modules (auto-generation fails)     | `~\src\Python\Documentation\manual` | Python Wrapper        |

### Adding

You can just create a new markdown file and write content in it. See [Editing](#editing) for more guidance.

> Don't forget to add a reference to the new file in `toc.yml`. It then appears in the navigation menu.

### Editing

#### The basics

`Articles` and `Code Samples` are in markdown format. It is pretty simple to use for our purpose. You can edit them as you do text files. `DocFX` documentation has a [guide](https://dotnet.github.io/docfx/docs/markdown.html?tabs=linux%2Cdotnet) you can refer to for formatting.

#### Links

##### Auto-generated content

If you wanted to link to, say, `Tableau.Migration.Api.IApiClient`, just do this `[Tableau.Migration.Api.IApiClient](xref: Tableau.Migration.Api.IApiClient)`. The part after `xref:` is a uid from the `yml` files in the `api` folder. Here is a snippet of this example

```yml
items:
- uid: Tableau.Migration.Api.IApiClient
  commentId: T:Tableau.Migration.Api.IApiClient
  id: IApiClient
  
```

> **Important**
> xref links work with anchors. Example: `[NetworkOptions.ExceptionsLoggingEnabled](xref:Tableau.Migration.Config.NetworkOptions#Tableau_Migration_Config_NetworkOptions_ExceptionsLoggingEnabled)`

##### Other `md` files

The easiest way is like this `[API Reference](~/api-csharp/index.md)`. The preferred method is to use a relative URL. However, most doc do not use it because levels of  `../` are less intuitive. `DocFX` will show warnings for invalid links during doc generation.

#### Comments

Comments are not natively supported in Markdown. The workaround is to us an empty link. Credit: https://stackoverflow.com/a/20885980

Best way to add links is like this:
`[//]: <> (This is a comment.)`

#### Alerts

Alerts should be written as below.
> Github and VS Code previews may not render them properly but `DocFX` does in the HTML output.

```markdown
> [!NOTE]
> This is a car.
```

```markdown
> [!TIP]
> If the car does not move, put in in drive or park.
```

```markdown
> [!IMPORTANT]
> Always change the engine oil on time. A new engine costs a lot more than a few oil changes.
```

```markdown
> [!CAUTION]
> Check your surroundings when backing up.
```

```markdown
> [!WARNING]
> Do not operate brake and accelerator pedals with two feet.
```

### Tabs

DocFX can also render content in [tabs](https://dotnet.github.io/docfx/docs/markdown.html?tabs=linux%2Cdotnet#tabs). Here is an example tab group.

```markdown

### [Tab 1 Name](#tab/tab1-id)

Content in Tab 1

### [Tab 2](#tab/tab2-id)

Content in Tab 2

### [Tab 3](#tab/tab3-id)

Content in Tab 3

---
```

#### Code snippets from example apps

`DocFX` can make [code snippets](https://dotnet.github.io/docfx/docs/markdown.html?tabs=linux%2Cdotnet#code-snippet) out of actual code files. Here are some examples of markdown to do that.

##### Using regions (Preferred method)

```markdown
[!code-csharp[](../../../../examples/Csharp.ExampleApplication/MyMigrationApplication.cs#DefaultProjectsFilter-Registration)]
```

##### Using line numbers

> [!WARNING]
> Use these very sparingly for C#. When the code is edited, it can change line numbers and mess up code snippets.

```markdown
[!code-csharp[](../../../../examples/Csharp.ExampleApplication/Hooks/Mappings/EmailDomainMapping.cs#L8-)]
```

> [!IMPORTANT]
> Regions do not work for Python code snippets. So, the options are to include the whole file or using line numbers.

```markdown
[!code-python[](../../../examples/Python.ExampleApplication/Python.ExampleApplication.py#L3-)]
```

#### Code blocks

Some examples

##### `C#`

````markdown
```C#
public sealed class MyMigrationApplicationOptions
{
        public EndpointOptions Source { get; set; } = new();
    
        public EndpointOptions Destination { get; set; } = new();
}
```
````

##### `Python`

````markdown

```python
import configparser         # configuration parser

from tableau_migration import MigrationPlanBuilder, Migrator

if __name__ == '__main__':
    config = configparser.ConfigParser()
    config.read('config.DEV.ini')

    # Build the plan
    plan_builder = MigrationPlanBuilder()
    plan_builder = plan_builder \
                .from_source_tableau_server(config['SOURCE']['URL'], config['SOURCE']['SITE_CONTENT_URL'], config['SOURCE']['ACCESS_TOKEN_NAME'], config['SOURCE']['ACCESS_TOKEN']) \
                .to_destination_tableau_cloud(config['DESTINATION']['URL'], config['DESTINATION']['SITE_CONTENT_URL'], config['DESTINATION']['ACCESS_TOKEN_NAME'], config['DESTINATION']['ACCESS_TOKEN']) \
                .for_server_to_cloud() \
                    .with_tableau_id_authentication_type() \
                    .with_tableau_cloud_usernames(config['USERS']['EMAIL_DOMAIN'])       
```

````

## Tools

Here are some helpful tools and extensions

- [Visual Studio Code](https://code.visualstudio.com/)
  - Extensions
    - [markdownlint](https://marketplace.visualstudio.com/items?itemName=DavidAnson.vscode-markdownlint)
    - [Markdown Editor](https://marketplace.visualstudio.com/items?itemName=zaaack.markdown-editor)
    - [Markdown All in One](https://marketplace.visualstudio.com/items?itemName=yzhang.markdown-all-in-one)
    - [Code Spell Checker](https://marketplace.visualstudio.com/items?itemName=streetsidesoftware.code-spell-checker)
      - Check the `Problems` panel in VS Code for spelling warnings.
      - False positives: Add to the `ignoreWords` array in `~/src/Documentation/cspell.json`.
    - [LanguageTool for Visual Studio Code](https://marketplace.visualstudio.com/items?itemName=adamvoss.vscode-languagetool)
    - [Grammarly Plugin](https://marketplace.visualstudio.com/items?itemName=znck.grammarly)
