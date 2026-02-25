using ClinicalDecisionSupportService.Application.Features.NewsScore;
using ClinicalDecisionSupportService.Domain.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicalDecisionSupportService.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<INewsScoreCalculator, NewsScoreCalculator>();
        services.AddScoped<ICalculateNewsScoreQueryHandler, CalculateNewsScoreQueryHandler>();

        return services;
    }
}
