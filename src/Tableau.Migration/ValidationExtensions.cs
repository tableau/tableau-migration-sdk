using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Tableau.Migration
{
    internal static class ValidationExtensions
    {
        internal static IResult ValidateSimpleProperties(this object o)
        {
            var results = new List<ValidationResult>();

            var success = Validator.TryValidateObject(o, new(o), results, validateAllProperties: true);
            if (success)
            {
                return Result.Succeeded();
            }

            var errors = results.Select(r => new ValidationException(r.ErrorMessage)).ToImmutableArray();

            return Result.Failed(errors);
        }
    }
}
