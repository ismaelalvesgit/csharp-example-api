using Example.Application.Services;
using Example.Data.Repositorys;
using Example.Domain.Interfaces.Repository;
using Example.Domain.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Example.Application.Modules;

public static class ApplicationModule
{
    public static void Register(IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
    {

        Dictionary<Type, Type> container = new()
        {
            // Domain
            { typeof(IServiceBase<>), typeof(ServiceBase<>) },
            { typeof(IProductService), typeof(ProductService) },
            { typeof(ICategoryService), typeof(CategoryService) },
            { typeof(IProducerService), typeof(ProducerService) },

            // Data
            { typeof(IRepositoryBase<>), typeof(RepositoryBase<>) },
            { typeof(IProductRepository), typeof(ProductRepository) },
            { typeof(ICategoryRepository), typeof(CategoryRepository) }
        };

        foreach (var (serviceType, implementationType) in container)
        {
            switch (serviceLifetime)
            {
                case ServiceLifetime.Scoped:
                    services.AddScoped(serviceType, implementationType); break;
                case ServiceLifetime.Singleton:
                    services.AddSingleton(serviceType, implementationType); break;
                case ServiceLifetime.Transient:
                    services.AddTransient(serviceType, implementationType); break;
            }
        }
    }
}
