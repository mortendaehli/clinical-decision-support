using ClinicalDecisionSupportService.Domain.Enums;

namespace ClinicalDecisionSupportService.Domain.Scoring;

public static class VitalSignCatalog
{
    public static readonly IReadOnlyDictionary<MeasurementType, VitalSign> ByType = new Dictionary<
        MeasurementType,
        VitalSign
    >
    {
        [MeasurementType.Temperature] = new VitalSign(
            Type: MeasurementType.Temperature,
            Code: "TEMP",
            DisplayName: "Body temperature",
            ValidRange: new RangeRule(31, 42),
            OutOfRangeErrorCode: "TEMP_OUT_OF_RANGE"
        ),

        [MeasurementType.HeartRate] = new VitalSign(
            Type: MeasurementType.HeartRate,
            Code: "HR",
            DisplayName: "Heart rate",
            ValidRange: new RangeRule(25, 220),
            OutOfRangeErrorCode: "HR_OUT_OF_RANGE"
        ),

        [MeasurementType.RespiratoryRate] = new VitalSign(
            Type: MeasurementType.RespiratoryRate,
            Code: "RR",
            DisplayName: "Respiratory rate",
            ValidRange: new RangeRule(3, 60),
            OutOfRangeErrorCode: "RR_OUT_OF_RANGE"
        ),
    };

    private static readonly IReadOnlyDictionary<string, VitalSign> ByCode =
        ByType.Values.ToDictionary(rule => rule.Code, StringComparer.OrdinalIgnoreCase);

    public static bool TryGetByType(MeasurementType measurementType, out VitalSign rule) =>
        ByType.TryGetValue(measurementType, out rule!);

    public static bool TryGetByCode(string code, out VitalSign rule) =>
        ByCode.TryGetValue(code, out rule!);
}
