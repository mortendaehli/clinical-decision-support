using ClinicalDecisionSupportService.Domain.Enums;

namespace ClinicalDecisionSupportService.UnitTests.Domain;

public sealed class MeasurementTypeCodeTests
{
    [Theory]
    [InlineData(MeasurementType.Temperature, "TEMP")]
    [InlineData(MeasurementType.HeartRate, "HR")]
    [InlineData(MeasurementType.RespiratoryRate, "RR")]
    public void to_code_returns_expected_canonical_value(MeasurementType measurementType, string expectedCode)
    {
        var code = MeasurementTypeCode.ToCode(measurementType);

        Assert.Equal(expectedCode, code);
    }

    [Theory]
    [InlineData("TEMP", MeasurementType.Temperature)]
    [InlineData("HR", MeasurementType.HeartRate)]
    [InlineData("RR", MeasurementType.RespiratoryRate)]
    public void try_parse_strict_accepts_supported_canonical_codes(
        string code,
        MeasurementType expectedType
    )
    {
        var success = MeasurementTypeCode.TryParseStrict(code, out var parsedType);

        Assert.True(success);
        Assert.Equal(expectedType, parsedType);
    }

    [Theory]
    [InlineData("temp")]
    [InlineData("Hr")]
    [InlineData("rr")]
    [InlineData("SPO2")]
    public void try_parse_strict_rejects_non_canonical_or_unsupported_codes(string code)
    {
        var success = MeasurementTypeCode.TryParseStrict(code, out _);

        Assert.False(success);
    }

    [Fact]
    public void supported_codes_contains_expected_values()
    {
        Assert.Equal(["TEMP", "HR", "RR"], MeasurementTypeCode.SupportedCodes);
    }
}
