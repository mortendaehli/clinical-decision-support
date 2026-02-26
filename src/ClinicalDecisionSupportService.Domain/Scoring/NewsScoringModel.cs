using ClinicalDecisionSupportService.Domain.Common;
using ClinicalDecisionSupportService.Domain.Enums;
using ClinicalDecisionSupportService.Domain.Services;
using ClinicalDecisionSupportService.Domain.ValueObjects;

namespace ClinicalDecisionSupportService.Domain.Scoring;

public sealed class NewsScoringModel : IScoringModel
{
    private static readonly IReadOnlyDictionary<MeasurementType, IReadOnlyList<ScoreBand>> ScoreBandsByType =
        new Dictionary<MeasurementType, IReadOnlyList<ScoreBand>>
        {
            [MeasurementType.TEMP] =
            [
                new ScoreBand(new RangeRule(31, 35), 3),
                new ScoreBand(new RangeRule(35, 36), 1),
                new ScoreBand(new RangeRule(36, 38), 0),
                new ScoreBand(new RangeRule(38, 39), 1),
                new ScoreBand(new RangeRule(39, 42), 2),
            ],
            [MeasurementType.HR] =
            [
                new ScoreBand(new RangeRule(25, 40), 3),
                new ScoreBand(new RangeRule(40, 50), 1),
                new ScoreBand(new RangeRule(50, 90), 0),
                new ScoreBand(new RangeRule(90, 110), 1),
                new ScoreBand(new RangeRule(110, 130), 2),
                new ScoreBand(new RangeRule(130, 220), 3),
            ],
            [MeasurementType.RR] =
            [
                new ScoreBand(new RangeRule(3, 8), 3),
                new ScoreBand(new RangeRule(8, 11), 1),
                new ScoreBand(new RangeRule(11, 20), 0),
                new ScoreBand(new RangeRule(20, 24), 2),
                new ScoreBand(new RangeRule(24, 60), 3),
            ],
        };

    public string ModelId => "NEWS";

    public IReadOnlyCollection<MeasurementType> RequiredVitalSigns => ScoreBandsByType.Keys.ToArray();

    public bool TryScore(MeasurementType measurementType, int value, out int score)
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

    public Result<int, DomainError> Calculate(IReadOnlyDictionary<MeasurementType, Measurement> measurements)
    {
        var totalScore = 0;

        foreach (var requiredType in RequiredVitalSigns)
        {
            var measurement = measurements[requiredType];
            if (!TryScore(requiredType, measurement.Value, out var score))
            {
                return DomainError.Unexpected(
                    code: "SCORING_RULE_MISMATCH",
                    message: $"No scoring band configured for {requiredType} value {measurement.Value}."
                );
            }

            totalScore += score;
        }

        return totalScore;
    }
}
