using ClinicalDecisionSupportService.Domain.Common;
using ClinicalDecisionSupportService.Domain.ValueObjects;

namespace ClinicalDecisionSupportService.Domain.Services;

public sealed class ScoringEngine : IScoringEngine
{
    private readonly IReadOnlyDictionary<string, IScoringModel> _modelsById;

    public ScoringEngine(IEnumerable<IScoringModel> models)
    {
        _modelsById = models.ToDictionary(x => x.ModelId, StringComparer.Ordinal);
    }

    public Result<int, DomainError> Calculate(string modelId, IReadOnlyCollection<Measurement> measurements)
    {
        if (!_modelsById.TryGetValue(modelId, out var model))
        {
            return DomainError.NotFound(
                code: "SCORING_MODEL_NOT_FOUND",
                message: $"Scoring model '{modelId}' was not found."
            );
        }

        var providedByType = measurements
            .GroupBy(x => x.Type)
            .ToDictionary(group => group.Key, group => group.ToList());

        foreach (var requiredType in model.RequiredVitalSigns)
        {
            if (!providedByType.TryGetValue(requiredType, out var entries) || entries.Count == 0)
            {
                return DomainError.Validation(
                    code: "MEASUREMENT_MISSING",
                    message: $"Missing required measurement type '{requiredType}'.",
                    field: "measurements"
                );
            }

            if (entries.Count > 1)
            {
                return DomainError.Validation(
                    code: "MEASUREMENT_DUPLICATE",
                    message: $"Measurement type '{requiredType}' must be provided exactly once.",
                    field: "measurements"
                );
            }
        }

        if (providedByType.Count != model.RequiredVitalSigns.Count)
        {
            return DomainError.Validation(
                code: "MEASUREMENT_TYPE_UNSUPPORTED",
                message: "Unsupported measurement type provided.",
                field: "measurements"
            );
        }

        var measurementsByType = providedByType.ToDictionary(x => x.Key, x => x.Value[0]);
        return model.Calculate(measurementsByType);
    }
}
