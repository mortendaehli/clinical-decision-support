using ClinicalDecisionSupportService.Application.Extensions;
using ClinicalDecisionSupportService.Infrastructure.Extensions;

namespace ClinicalDecisionSupportService.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppConfiguration(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        return services
            .AddApplication()
            .AddInfrastructure(configuration)
            .AddApiCoreServices()
            .AddObservability()
            .AddAuthorizationServices();
    }

    private static IServiceCollection AddApiCoreServices(this IServiceCollection services)
    {
        services.AddOpenApi();
        return services;
    }

    private static IServiceCollection AddObservability(this IServiceCollection services)
    {
        // Placeholder: OpenTelemetry, logging enrichment, tracing, metrics
        return services;
    }

    private static IServiceCollection AddAuthorizationServices(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder();
        return services;
    }
}
