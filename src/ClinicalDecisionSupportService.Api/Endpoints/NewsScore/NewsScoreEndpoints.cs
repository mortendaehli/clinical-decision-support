using ClinicalDecisionSupportService.Api.Extensions;
using ClinicalDecisionSupportService.Application.Features.NewsScore;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ClinicalDecisionSupportService.Api.Endpoints.NewsScore;

public static class NewsScoreEndpoints
{
    public static IEndpointRouteBuilder MapNewsScoreEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints
            .MapPost("/news-score", Handle)
            .WithName("CalculateNewsScore")
            .WithSummary("Calculate NEWS score")
            .WithDescription(
                """
                Accepts TEMP, HR, and RR measurements and returns NEWS score.
                Validation and scoring logic are handled in the Application layer.
                """
            )
            .WithTags("Scores")
            .Produces<NewsScoreResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        return endpoints;
    }

    private static async Task<Results<Ok<NewsScoreResponse>, ProblemHttpResult>> Handle(
        NewsScoreRequest request,
        ICalculateNewsScoreQueryHandler queryHandler,
        CancellationToken cancellationToken
    )
    {
        var query = new CalculateNewsScoreQuery(
            (request.Measurements ?? [])
                .Select(x => new CalculateNewsScoreMeasurementInput(x.Type, x.Value))
                .ToList()
        );
        var scoreResult = await queryHandler.Handle(query, cancellationToken);

        return scoreResult.ToHttpResult(x => new NewsScoreResponse(x.Score));
    }
}
