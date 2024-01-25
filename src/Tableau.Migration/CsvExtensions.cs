using System.Text;

namespace Tableau.Migration
{
    internal static class CsvExtensions
    {
        /// <summary>
        /// Creates a csv string from an array of strings in the order passed.
        /// </summary>
        /// <param name="builder">A string builder to append to.</param>
        /// <param name="values">The input string values in order.</param>
        internal static StringBuilder AppendCsvLine(this StringBuilder builder, params string?[] values)
        {
            if (!values.IsNullOrEmpty())
            {
                if (values.Length == 1)
                {
                    builder.AppendCsvValue(values[0]);
                }
                else
                {
                    foreach (var value in values[0..^1])
                    {
                        builder.AppendCsvValue(value);
                        builder.Append(',');
                    }

                    builder.AppendCsvValue(values[^1]);
                }
            }

            builder.AppendLine();
            return builder;
        }

        /// <summary>
        /// Escapes CSV unsafe characters.
        /// </summary>
        /// <param name="builder">A string builder to append to.</param>
        /// <param name="value">The string value to be escaped.</param>
        internal static StringBuilder AppendCsvValue(this StringBuilder builder, string? value)
        {
            if (!value.IsNullOrEmpty() &&
                (value.Contains(',') || value.Contains('"') || value.Contains('\r') || value.Contains('\n')))
            {
                builder.Append('"');
                builder.Append(value.Replace("\"", "\"\""));
                builder.Append('"');
            }
            else
            {
                builder.Append(value);
            }

            return builder;
        }
    }
}
