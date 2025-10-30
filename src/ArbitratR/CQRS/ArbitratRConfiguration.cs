using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ArbitratR.CQRS
{
    /// <summary>
    /// Provides configuration options for registering ArbitratR command and query handlers in the dependency injection container.
    /// </summary>
    public class ArbitratRConfiguration
    {
        private readonly IServiceCollection _services;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArbitratRConfiguration"/> class.
        /// </summary>
        /// <param name="services">The service collection to register handlers with.</param>
        public ArbitratRConfiguration(IServiceCollection services)
        {
            _services = services;
        }

        /// <summary>
        /// Registers all command and query handlers from the specified assemblies.
        /// </summary>
        /// <param name="assemblies">The assemblies to scan for handlers. If none are provided, the calling assembly is used.</param>
        public void AddHandlers(params Assembly[] assemblies)
        {
            AddQueryHandlers(assemblies);
            AddCommandHandlers(assemblies);
        }

        /// <summary>
        /// Registers all query handlers from the specified assemblies.
        /// </summary>
        /// <param name="assemblies">The assemblies to scan for query handlers. If none are provided, the calling assembly is used.</param>
        public void AddQueryHandlers(params Assembly[] assemblies)
        {
            if (assemblies is null || assemblies.Length == 0)
            {
                assemblies = [Assembly.GetCallingAssembly()];
            }

            foreach (var assembly in assemblies)
            {
                AddScoped(assembly, typeof(IQueryHandler<,>));
            }
        }

        /// <summary>
        /// Registers all command handlers from the specified assemblies.
        /// </summary>
        /// <param name="assemblies">The assemblies to scan for command handlers. If none are provided, the calling assembly is used.</param>
        public void AddCommandHandlers(params Assembly[] assemblies)
        {
            if (assemblies is null || assemblies.Length == 0)
            {
                assemblies = [Assembly.GetCallingAssembly()];
            }

            foreach (var assembly in assemblies)
            {
                AddScoped(assembly, typeof(ICommandHandler<>));
                AddScoped(assembly, typeof(ICommandHandler<,>));
            }
        }

        /// <summary>
        /// Registers all concrete types implementing the specified generic interface type as scoped services.
        /// </summary>
        /// <param name="assembly">The assembly to scan for handler implementations.</param>
        /// <param name="type">The generic type definition of the handler interface to register.</param>
        private void AddScoped(Assembly assembly, Type type)
        {
            foreach (var handlerType in assembly.GetTypes()
                    .Where(t => t.IsClass && !t.IsAbstract &&
                        t.GetInterfaces().Any(i =>
                            i.IsGenericType && i.GetGenericTypeDefinition() == type)))
            {
                var interfaceType = handlerType.GetInterfaces()
                    .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == type);

                _services.AddScoped(interfaceType, handlerType);
            }
        }
    }
}
