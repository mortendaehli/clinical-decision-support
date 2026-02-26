using ClinicalDecisionSupportService.Domain.Common;
using ClinicalDecisionSupportService.Domain.Enums;
using ClinicalDecisionSupportService.Domain.ValueObjects;

namespace ClinicalDecisionSupportService.Domain.Services;

public interface IScoringModel
{
    string ModelId { get; }

    IReadOnlyCollection<MeasurementType> RequiredVitalSigns { get; }

    bool TryScore(MeasurementType measurementType, int value, out int score);

    Result<int, DomainError> Calculate(IReadOnlyDictionary<MeasurementType, Measurement> measurements);
}
