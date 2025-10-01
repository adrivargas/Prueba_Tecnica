using Microsoft.Extensions.DependencyInjection;
using ProductService.Api.Repositories;


namespace ProductService.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IProductRepository, ProductRepository>();
            return services;
        }
    }
}