using ClinicalDecisionSupportService.Domain.Common;
using ClinicalDecisionSupportService.Domain.Enums;
using ClinicalDecisionSupportService.Domain.Scoring;
using ClinicalDecisionSupportService.Domain.Services;
using ClinicalDecisionSupportService.Domain.ValueObjects;

namespace ClinicalDecisionSupportService.Application.Features.NewsScore;

public sealed class CalculateNewsScoreQueryHandler : ICalculateNewsScoreQueryHandler
{
    private const string NewsModelId = "NEWS";

    private readonly IScoringEngine _scoringEngine;

    public CalculateNewsScoreQueryHandler(IScoringEngine scoringEngine)
    {
        _scoringEngine = scoringEngine;
    }

    public Task<Result<CalculateNewsScoreResult, DomainError>> Handle(
        CalculateNewsScoreQuery query,
        CancellationToken cancellationToken
    )
    {
        _ = cancellationToken;

        if (query.Measurements is null || query.Measurements.Count == 0)
        {
            return Error(
                DomainError.Validation(
                    code: "MEASUREMENTS_REQUIRED",
                    message: "At least one measurement is required.",
                    field: "measurements"
                )
            );
        }

        var measurements = new List<Measurement>(query.Measurements.Count);

        for (var i = 0; i < query.Measurements.Count; i++)
        {
            var input = query.Measurements[i];

            var measurementTypeResult = ParseMeasurementType(input.Type, $"measurements[{i}].type");
            if (measurementTypeResult.IsErr())
            {
                return Error(measurementTypeResult.Error!);
            }

            var measurementResult = Measurement.Create(
                measurementTypeResult.Value!,
                input.Value,
                field: $"measurements[{i}].value"
            );

            if (measurementResult.IsErr())
            {
                return Error(measurementResult.Error!);
            }

            measurements.Add(measurementResult.Value!);
        }

        var scoreResult = _scoringEngine.Calculate(NewsModelId, measurements);

        return Task.FromResult(scoreResult.Map(score => new CalculateNewsScoreResult(score)));
    }

    private static Task<Result<CalculateNewsScoreResult, DomainError>> Error(DomainError error) =>
        Task.FromResult<Result<CalculateNewsScoreResult, DomainError>>(error);

    private static Result<MeasurementType, DomainError> ParseMeasurementType(string? value, string field)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return DomainError.Validation(
                code: "MEASUREMENT_TYPE_REQUIRED",
                message: "Measurement type is required.",
                field: field
            );
        }

        if (!VitalSignDefinitions.TryParseType(value, out var measurementType))
        {
            return DomainError.Validation(
                code: "MEASUREMENT_TYPE_INVALID",
                message: $"Measurement type must be one of: {string.Join(", ", VitalSignDefinitions.SupportedCodes)}.",
                field: field
            );
        }

        return measurementType;
    }
}
