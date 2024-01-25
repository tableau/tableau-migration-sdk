using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace Tableau.Migration.Tests
{
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Replaces an existing service in the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TService">The type of service to register.</typeparam>
        /// <param name="services">The service collection to register services with.</param>
        /// <param name="service">The service to register.</param>
        /// <returns>The same service collection as the <paramref name="services"/> parameter.</returns>
        public static IServiceCollection Replace<TService>(
            this IServiceCollection services,
            TService service)
            where TService : class
        {
            var serviceType = typeof(TService);
            object instance = service;

            if (serviceType.IsAssignableTo(typeof(Mock)))
            {
                var mockInterface = serviceType.GetInterface(typeof(IMock<>).Name)!;

                serviceType = mockInterface.GenericTypeArguments[0];

                instance = mockInterface.GetProperty("Object")!.GetValue(instance)!;
            }

            var existing = services.SingleOrDefault(s => s.ServiceType == serviceType)
                ?? throw new ArgumentException($"Could not find a registered service of type {serviceType.Name}.", nameof(TService));

            var descriptor = new ServiceDescriptor(existing.ServiceType, _ => instance, existing.Lifetime);

            // Call the extension method explicitly here so the MS overload is called instead of this method.
            return ServiceCollectionDescriptorExtensions.Replace(services, descriptor);
        }
    }
}
