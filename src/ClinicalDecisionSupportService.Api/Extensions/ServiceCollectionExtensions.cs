using ClinicalDecisionSupportService.Application.Extensions;
using ClinicalDecisionSupportService.Api.Endpoints.NewsScore;
using ClinicalDecisionSupportService.Domain.Enums;
using ClinicalDecisionSupportService.Infrastructure.Extensions;
using Microsoft.OpenApi;
using System.Text.Json.Nodes;

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
        services.AddOpenApi(options =>
        {
            options.AddSchemaTransformer((schema, context, _) =>
            {
                if (context.JsonTypeInfo.Type != typeof(MeasurementInput))
                {
                    return Task.CompletedTask;
                }

                if (
                    schema.Properties is null
                    || !schema.Properties.TryGetValue("type", out var measurementTypeSchema)
                    || measurementTypeSchema is not OpenApiSchema openApiMeasurementTypeSchema
                )
                {
                    return Task.CompletedTask;
                }

                openApiMeasurementTypeSchema.Type = JsonSchemaType.String;
                openApiMeasurementTypeSchema.Enum = MeasurementTypeCode
                    .SupportedCodes.Select(code => (JsonNode)JsonValue.Create(code)!)
                    .ToList();

                return Task.CompletedTask;
            });
        });
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
