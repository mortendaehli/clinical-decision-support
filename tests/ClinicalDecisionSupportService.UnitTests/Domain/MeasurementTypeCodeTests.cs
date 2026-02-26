using ClinicalDecisionSupportService.Domain.Enums;
using ClinicalDecisionSupportService.Domain.Scoring;

namespace ClinicalDecisionSupportService.UnitTests.Domain;

public sealed class MeasurementTypeParsingTests
{
    [Theory]
    [InlineData("TEMP", MeasurementType.TEMP)]
    [InlineData("HR", MeasurementType.HR)]
    [InlineData("RR", MeasurementType.RR)]
    public void try_parse_type_accepts_canonical_codes(string code, MeasurementType expectedType)
    {
        var success = VitalSignDefinitions.TryParseType(code, out var measurementType);

        Assert.True(success);
        Assert.Equal(expectedType, measurementType);
    }

    [Theory]
    [InlineData("temp")]
    [InlineData("Hr")]
    [InlineData("rr")]
    [InlineData("SPO2")]
    [InlineData("")]
    [InlineData(" ")]
    public void try_parse_type_rejects_invalid_codes(string code)
    {
        var success = VitalSignDefinitions.TryParseType(code, out _);

        Assert.False(success);
    }

    [Fact]
    public void supported_codes_contains_expected_values()
    {
        Assert.Equal(["TEMP", "HR", "RR"], VitalSignDefinitions.SupportedCodes);
    }
}
