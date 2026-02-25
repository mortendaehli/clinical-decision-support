using ClinicalDecisionSupportService.Domain.Common;
using ClinicalDecisionSupportService.Domain.Enums;
using ClinicalDecisionSupportService.Domain.Scoring;
using ClinicalDecisionSupportService.Domain.ValueObjects;

namespace ClinicalDecisionSupportService.Domain.Services;

public sealed class NewsScoreCalculator : INewsScoreCalculator
{
    private static readonly IReadOnlyDictionary<
        MeasurementType,
        IReadOnlyList<ScoreBand>
    > ScoreBandsByType = new Dictionary<MeasurementType, IReadOnlyList<ScoreBand>>
    {
        [MeasurementType.Temperature] =
        [
            new ScoreBand(new RangeRule(31, 35), 3),
            new ScoreBand(new RangeRule(35, 36), 1),
            new ScoreBand(new RangeRule(36, 38), 0),
            new ScoreBand(new RangeRule(38, 39), 1),
            new ScoreBand(new RangeRule(39, 42), 2),
        ],
        [MeasurementType.HeartRate] =
        [
            new ScoreBand(new RangeRule(25, 40), 3),
            new ScoreBand(new RangeRule(40, 50), 1),
            new ScoreBand(new RangeRule(50, 90), 0),
            new ScoreBand(new RangeRule(90, 110), 1),
            new ScoreBand(new RangeRule(110, 130), 2),
            new ScoreBand(new RangeRule(130, 220), 3),
        ],
        [MeasurementType.RespiratoryRate] =
        [
            new ScoreBand(new RangeRule(3, 8), 3),
            new ScoreBand(new RangeRule(8, 11), 1),
            new ScoreBand(new RangeRule(11, 20), 0),
            new ScoreBand(new RangeRule(20, 24), 2),
            new ScoreBand(new RangeRule(24, 60), 3),
        ],
    };

    private static readonly List<MeasurementType> RequiredMeasurements =
    [
        MeasurementType.Temperature,
        MeasurementType.HeartRate,
        MeasurementType.RespiratoryRate,
    ];

    public Result<int, DomainError> Calculate(IReadOnlyCollection<Measurement> measurements)
    {
        var providedByType = measurements
            .GroupBy(x => x.MeasurementType)
            .ToDictionary(group => group.Key, group => group.ToList());

        foreach (
            var requiredRule in RequiredMeasurements.Select(requiredType =>
                VitalSignCatalog.ByType[requiredType]
            )
        )
        {
            if (
                !providedByType.TryGetValue(requiredRule.Type, out var entries)
                || entries.Count == 0
            )
            {
                return DomainError.Validation(
                    code: "MEASUREMENT_MISSING",
                    message: $"Missing required measurement type '{requiredRule.Code}'.",
                    field: "measurements"
                );
            }

            if (entries.Count > 1)
            {
                return DomainError.Validation(
                    code: "MEASUREMENT_DUPLICATE",
                    message: $"Measurement type '{requiredRule.Code}' must be provided exactly once.",
                    field: "measurements"
                );
            }
        }

        if (providedByType.Count != VitalSignCatalog.ByType.Count)
        {
            return DomainError.Validation(
                code: "MEASUREMENT_TYPE_UNSUPPORTED",
                message: "Unsupported measurement type provided.",
                field: "measurements"
            );
        }

        var totalScore = 0;

        foreach (var requiredRule in VitalSignCatalog.ByType.Values)
        {
            var measurement = providedByType[requiredRule.Type][0];
            if (!TryResolveScore(requiredRule.Type, measurement.Value, out var score))
            {
                return DomainError.Unexpected(
                    code: "SCORING_RULE_MISMATCH",
                    message: $"No scoring band configured for {requiredRule.Code} value {measurement.Value}."
                );
            }

            totalScore += score;
        }

        return totalScore;
    }

    private static bool TryResolveScore(MeasurementType measurementType, int value, out int score)
    {
        if (!ScoreBandsByType.TryGetValue(measurementType, out var scoreBands))
        {
            score = default;
            return false;
        }

        foreach (var band in scoreBands)
        {
            if (band.Matches(value))
            {
                score = band.Score;
                return true;
            }
        }

        score = default;
        return false;
    }
}
