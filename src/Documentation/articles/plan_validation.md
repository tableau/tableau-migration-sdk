# Plan Validation


## Plan Builder

The main input for a migration is through a migration plan. A plan builder is provided by the SDK to build a valid migration plan through a [fluent interface](https://en.wikipedia.org/wiki/Fluent_interface). The plan builder is able to find validation errors before the migration plan is built and executed.

### Validation

The migration engine does not enforce plan validation, but users are highly encouraged to validate migration plans and abort execution if validation errors are returned to prevent errors during migration. The **Validate** plan builder method is used to perform plan builder validation:

```C#
    var validationResult = _planBuilder.Validate();
```


### Handling Validation Errors
If a validation error is detected, we recommend aborting the migration in an application appropriate manner. The following example checks for validation errors and logs them to the console:

```C#
if (!validationResult.Success)
    {
        _logger.LogError($"Migration plan validation failed.", validationResult.Errors);
        Console.WriteLine("Press any key to exit");
        Console.ReadKey();
        _appLifetime.StopApplication();
    }
```

Each validation error provides information on how to fix the error detected in the migration plan. Review the validation errors, adjust the plan builder as necessary, and re-run the migration.