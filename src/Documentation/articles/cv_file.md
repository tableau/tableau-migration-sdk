# Anatomy of a Custom View File

The Migration SDK downloads Custom View definition files during the migration process. There files are in JSON format. This is what a Custom View File looks like:

```json
[
    .....
    {
        "isSourceView": true,
        "viewName": "View 1",
        "tcv": "{base-64 custom view xml content with + signs replaced by -}"
    }
    .....
]

```

## Getting the Custom View Definition from the Custom View File

In the JSON file, the field `tcv` contains the XML definition of a custom view. This field is encoded as a Base64 string, with `+` characters replaced by `-`.

To decode the `tcv` value

1. Replace all the `-` characters in the string with `+`.
2. Decode the resulting Base64 string.
3. Get the XML Custom View Definition.

To encode the `tcv` value

1. Make any changes to the Custom View Definition.
2. Encode the XML into a Base64 string.
3. Replace all the `+` characters in the string with `-`.
