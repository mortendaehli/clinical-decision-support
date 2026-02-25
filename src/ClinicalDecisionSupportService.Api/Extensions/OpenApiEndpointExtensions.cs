using Scalar.AspNetCore;

namespace ClinicalDecisionSupportService.Api.Extensions;

public static class OpenApiEndpointExtensions
{
    public static WebApplication MapOpenApiEndpoints(this WebApplication app)
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
        return app;
    }
}
