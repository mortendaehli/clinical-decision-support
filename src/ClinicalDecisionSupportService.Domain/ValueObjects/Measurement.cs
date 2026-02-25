using ClinicalDecisionSupportService.Domain.Common;
using ClinicalDecisionSupportService.Domain.Enums;
using ClinicalDecisionSupportService.Domain.Scoring;

namespace ClinicalDecisionSupportService.Domain.ValueObjects;

public sealed record Measurement
{
    public MeasurementType MeasurementType { get; }
    public int Value { get; }

    private Measurement(MeasurementType measurementType, int value)
    {
        MeasurementType = measurementType;
        Value = value;
    }

    public static Result<Measurement, DomainError> Create(
        MeasurementType measurementType,
        int value,
        string field = "value"
    )
    {
        var error = Validate(measurementType, value, field);
        if (error is not null)
        {
            return error;
        }

        return new Measurement(measurementType, value);
    }

    private static DomainError? Validate(MeasurementType measurementType, int value, string field)
    {
        if (!VitalSignCatalog.TryGetByType(measurementType, out var rule))
        {
            return DomainError.Validation(
                code: "MEASUREMENT_TYPE_INVALID",
                message: "Measurement type is invalid.",
                field: nameof(measurementType)
            );
        }

        return rule.ValidRange.Contains(value)
            ? null
            : DomainError.Validation(
                code: rule.OutOfRangeErrorCode,
                message: $"{rule.Code} must be {rule.ValidRange}.",
                field: field
            );
    }
}
