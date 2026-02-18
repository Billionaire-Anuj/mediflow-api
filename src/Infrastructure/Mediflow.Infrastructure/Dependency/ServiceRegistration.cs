using Mediflow.Application.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Mediflow.Infrastructure.Dependency;

public static class ServiceRegistration
{
    extension(IServiceCollection services)
    {
        public void AddDependencyServices()
        {
            services
                .AddServices(typeof(IScopedService), ServiceLifetime.Scoped)
                .AddServices(typeof(ISingletonService), ServiceLifetime.Singleton)
                .AddServices(typeof(ITransientService), ServiceLifetime.Transient);
        }

        private IServiceCollection AddServices(Type interfaceType, ServiceLifetime lifetime)
        {
            var interfaceTypes =
                AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(t => interfaceType.IsAssignableFrom(t) && t is { IsClass: true, IsAbstract: false })
                    .Select(t => new
                    {
                        Service = t.GetInterfaces().FirstOrDefault(),
                        Implementation = t
                    })
                    .Where(t => t.Service is not null && interfaceType.IsAssignableFrom(t.Service));

            foreach (var type in interfaceTypes)
            {
                services.AddService(type.Service!, type.Implementation, lifetime);
            }

            return services;
        }

        private IServiceCollection AddService(Type serviceType,
            Type implementationType,
            ServiceLifetime lifetime) =>
            lifetime switch
            {
                ServiceLifetime.Singleton => services.AddSingleton(serviceType, implementationType),
                ServiceLifetime.Scoped => services.AddScoped(serviceType, implementationType),
                ServiceLifetime.Transient => services.AddTransient(serviceType, implementationType),
                _ => throw new ArgumentException("Invalid lifeTime", nameof(lifetime))
            };
    }
}