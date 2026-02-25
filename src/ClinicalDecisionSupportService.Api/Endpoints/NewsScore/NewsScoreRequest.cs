namespace ClinicalDecisionSupportService.Api.Endpoints.NewsScore;

public sealed record NewsScoreRequest(IReadOnlyList<MeasurementInput> Measurements);

public sealed record MeasurementInput(string Type, int Value);
