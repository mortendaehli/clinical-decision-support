using ClinicalDecisionSupportService.Domain.Common;
using ClinicalDecisionSupportService.Domain.Enums;
using ClinicalDecisionSupportService.Domain.Scoring;

namespace ClinicalDecisionSupportService.Domain.ValueObjects;

public sealed record Measurement
{
    public MeasurementType Type { get; }
    public int Value { get; }

    private Measurement(MeasurementType type, int value)
    {
        Type = type;
        Value = value;
    }

    public static Result<Measurement, DomainError> Create(
        MeasurementType type,
        int value,
        string field = "value"
    )
    {
        var error = Validate(type, value, field);
        if (error is not null)
        {
            return error;
        }

        return new Measurement(type, value);
    }

    private static DomainError? Validate(MeasurementType type, int value, string field)
    {
        if (!VitalSignDefinitions.TryGetByType(type, out var definition))
        {
            return DomainError.Validation(
                code: "MEASUREMENT_TYPE_INVALID",
                message: "Measurement type is invalid.",
                field: nameof(type)
            );
        }

        return definition.PhysiologicalRange.Contains(value)
            ? null
            : DomainError.Validation(
                code: definition.OutOfRangeErrorCode,
                message: $"{definition.Code} must be {definition.PhysiologicalRange}.",
                field: field
            );
    }
}
