using ClinicalDecisionSupportService.Domain.Enums;

namespace ClinicalDecisionSupportService.Domain.Scoring;

public sealed record VitalSignDefinition(
    MeasurementType Type,
    string DisplayName,
    RangeRule PhysiologicalRange,
    string OutOfRangeErrorCode
)
{
    public string Code => Type.ToString();
}
