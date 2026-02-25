namespace ClinicalDecisionSupportService.Application.Features.NewsScore;

public sealed record CalculateNewsScoreQuery(
    IReadOnlyList<CalculateNewsScoreMeasurementInput> Measurements
);

public sealed record CalculateNewsScoreMeasurementInput(string Type, int Value);
