namespace ClinicalDecisionSupportService.Domain.Enums;

public static class MeasurementTypeCode
{
    private static readonly IReadOnlyDictionary<MeasurementType, string> CodeByType = new Dictionary<
        MeasurementType,
        string
    >
    {
        [MeasurementType.Temperature] = "TEMP",
        [MeasurementType.HeartRate] = "HR",
        [MeasurementType.RespiratoryRate] = "RR",
    };

    private static readonly IReadOnlyDictionary<string, MeasurementType> TypeByCode = CodeByType
        .ToDictionary(entry => entry.Value, entry => entry.Key, StringComparer.Ordinal);

    public static readonly IReadOnlyList<string> SupportedCodes = CodeByType.Values.ToArray();

    public static string ToCode(MeasurementType type) =>
        CodeByType.TryGetValue(type, out var code)
            ? code
            : throw new ArgumentOutOfRangeException(
                nameof(type),
                type,
                "Unsupported measurement type."
            );

    public static bool TryParseStrict(string code, out MeasurementType type)
        => TypeByCode.TryGetValue(code, out type);
}
