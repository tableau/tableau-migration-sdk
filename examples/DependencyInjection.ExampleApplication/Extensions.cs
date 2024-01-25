using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DependencyInjection.ExampleApplication
{
    public static class Extensions
    {
        public static IServiceCollection AddConsoleLogging(this IServiceCollection services)
        {
            // Configure console logging.
            return services.AddLogging(builder =>
            {
                builder
                    .ClearProviders()
                    .AddSimpleConsole(c =>
                    {
                        c.SingleLine = true;
                    });
            });
        }

        public static IServiceCollection DisplayServices(this IServiceCollection serviceCollection, IEnumerable<Type>? filter = null, bool showFullNames = false)
        {
            serviceCollection.Where(d => filter is null || filter.Contains(d.ServiceType)).DisplayServices(showFullNames);

            return serviceCollection;
        }

        private static void DisplayServices(this IEnumerable<ServiceDescriptor> services, bool showFullNames = false)
        {
            Console.WriteLine("Services:");
            Console.WriteLine();

            foreach (var service in services)
            {
                Console.WriteLine(FormatService(service, showFullNames));
            }

            Console.WriteLine();

            Console.Out.Flush();
        }

        private static string FormatService(this ServiceDescriptor service, bool showFullNames)
        {
            return $"Service Type: {service.ServiceType.FormatType(showFullNames)}, Implementation {service.FormatImplementation(showFullNames)}, Lifetime: {service.Lifetime}";
        }

        private static string? FormatImplementation(this ServiceDescriptor service, bool showFullNames)
        {
            if (service.ImplementationType is not null)
                return $"Type: {service.ImplementationType.FormatType(showFullNames)}";

            if (service.ImplementationInstance is not null)
                return $"Instance: {service.ImplementationInstance.FormatInstance(showFullNames)}";

            if (service.ImplementationFactory is not null)
                return $"Factory: {service.ImplementationFactory.FormatFactory(showFullNames)}";

            return null;
        }

        private static string FormatType(this Type type, bool showFullName)
        {
            if (!type.IsGenericType)
                return FormatTypeName(type, showFullName);

            var genericTypeNames = new List<string>();

            foreach (var genericArgument in type.GetGenericArguments())
            {
                genericTypeNames.Add(FormatTypeName(genericArgument, showFullName));
            }

            return $"{FormatTypeName(type, showFullName)}<{String.Join(", ", genericTypeNames)}>";
        }

        private static string FormatTypeName(this Type t, bool showFullName)
        {
            var name = t.Name;

            if (showFullName && t.FullName is not null)
                name = t.FullName;

            return Regex.Replace(name, @"`\d+", String.Empty);
        }

        private static string FormatInstance<T>(this T obj, bool showFullNames)
        {
            return (obj?.GetType() ?? typeof(T)).FormatType(showFullNames);
        }

        private static string FormatFactory(this Func<IServiceProvider, object?> factory, bool showFullNames)
        {
            return factory.GetType().FormatType(showFullNames);
        }
    }
}
