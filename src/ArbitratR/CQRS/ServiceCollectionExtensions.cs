using Microsoft.Extensions.DependencyInjection;

namespace ArbitratR.CQRS
{
    /// <summary>
    /// Provides extension methods for registering ArbitratR services with the dependency injection container.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds ArbitratR services to the specified service collection.
        /// </summary>
        /// <param name="services">The service collection to add ArbitratR services to.</param>
        /// <param name="configure">An action to configure ArbitratR handler registration options.</param>
        /// <returns>The service collection for method chaining.</returns>
        public static IServiceCollection AddArbitratR(this IServiceCollection services, Action<ArbitratRConfiguration> configure)
        {
            var configuration = new ArbitratRConfiguration(services);
            configure.Invoke(configuration);

            return services;
        }
    }
}
