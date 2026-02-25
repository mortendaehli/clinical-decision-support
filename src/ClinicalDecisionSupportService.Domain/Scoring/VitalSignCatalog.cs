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
            Code: MeasurementTypeCode.ToCode(MeasurementType.Temperature),
            DisplayName: "Body temperature",
            ValidRange: new RangeRule(31, 42),
            OutOfRangeErrorCode: "TEMP_OUT_OF_RANGE"
        ),

        [MeasurementType.HeartRate] = new VitalSign(
            Type: MeasurementType.HeartRate,
            Code: MeasurementTypeCode.ToCode(MeasurementType.HeartRate),
            DisplayName: "Heart rate",
            ValidRange: new RangeRule(25, 220),
            OutOfRangeErrorCode: "HR_OUT_OF_RANGE"
        ),

        [MeasurementType.RespiratoryRate] = new VitalSign(
            Type: MeasurementType.RespiratoryRate,
            Code: MeasurementTypeCode.ToCode(MeasurementType.RespiratoryRate),
            DisplayName: "Respiratory rate",
            ValidRange: new RangeRule(3, 60),
            OutOfRangeErrorCode: "RR_OUT_OF_RANGE"
        ),
    };

    public static bool TryGetByType(MeasurementType measurementType, out VitalSign rule) =>
        ByType.TryGetValue(measurementType, out rule!);
}
