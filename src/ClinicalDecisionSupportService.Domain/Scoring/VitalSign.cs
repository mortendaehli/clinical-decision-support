using ClinicalDecisionSupportService.Domain.Enums;

namespace ClinicalDecisionSupportService.Domain.Scoring;

public sealed record VitalSign(
    MeasurementType Type,
    string Code,
    string DisplayName,
    RangeRule ValidRange,
    string OutOfRangeErrorCode);
