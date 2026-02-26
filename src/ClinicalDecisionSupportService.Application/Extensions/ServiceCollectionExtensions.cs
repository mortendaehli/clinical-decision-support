using ClinicalDecisionSupportService.Application.Features.NewsScore;
using ClinicalDecisionSupportService.Domain.Scoring;
using ClinicalDecisionSupportService.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicalDecisionSupportService.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IScoringModel, NewsScoringModel>();
        services.AddSingleton<IScoringEngine, ScoringEngine>();
        services.AddScoped<ICalculateNewsScoreQueryHandler, CalculateNewsScoreQueryHandler>();

        return services;
    }
}
