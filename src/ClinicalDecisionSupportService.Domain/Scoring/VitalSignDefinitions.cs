using ClinicalDecisionSupportService.Domain.Enums;

namespace ClinicalDecisionSupportService.Domain.Scoring;

public static class VitalSignDefinitions
{
    private static readonly IReadOnlyDictionary<
        MeasurementType,
        VitalSignDefinition
    > DefinitionsByType = new Dictionary<MeasurementType, VitalSignDefinition>
    {
        [MeasurementType.TEMP] = new VitalSignDefinition(
            Type: MeasurementType.TEMP,
            DisplayName: "Body temperature",
            PhysiologicalRange: new RangeRule(31, 42),
            OutOfRangeErrorCode: "TEMP_OUT_OF_RANGE"
        ),
        [MeasurementType.HR] = new VitalSignDefinition(
            Type: MeasurementType.HR,
            DisplayName: "Heart rate",
            PhysiologicalRange: new RangeRule(25, 220),
            OutOfRangeErrorCode: "HR_OUT_OF_RANGE"
        ),
        [MeasurementType.RR] = new VitalSignDefinition(
            Type: MeasurementType.RR,
            DisplayName: "Respiratory rate",
            PhysiologicalRange: new RangeRule(3, 60),
            OutOfRangeErrorCode: "RR_OUT_OF_RANGE"
        ),
    };

    public static IReadOnlyList<string> SupportedCodes => Enum.GetNames<MeasurementType>();

    public static bool TryParseType(string? value, out MeasurementType measurementType)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            measurementType = default;
            return false;
        }

        return Enum.TryParse(value.Trim(), ignoreCase: false, out measurementType)
            && Enum.IsDefined(measurementType);
    }

    public static bool TryGetByType(
        MeasurementType measurementType,
        out VitalSignDefinition definition
    ) => DefinitionsByType.TryGetValue(measurementType, out definition!);
}
