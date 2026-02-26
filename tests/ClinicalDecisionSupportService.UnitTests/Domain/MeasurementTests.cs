using ClinicalDecisionSupportService.Domain.Common;
using ClinicalDecisionSupportService.Domain.Enums;
using ClinicalDecisionSupportService.Domain.ValueObjects;

namespace ClinicalDecisionSupportService.UnitTests.Domain;

public sealed class MeasurementTests
{
    [Theory]
    // TEMP valid range: (31, 42]
    [InlineData(MeasurementType.TEMP, 31, false)]
    [InlineData(MeasurementType.TEMP, 32, true)]
    [InlineData(MeasurementType.TEMP, 37, true)]
    [InlineData(MeasurementType.TEMP, 42, true)]
    [InlineData(MeasurementType.TEMP, 43, false)]
    // HR valid range: (25, 220]
    [InlineData(MeasurementType.HR, 25, false)]
    [InlineData(MeasurementType.HR, 26, true)]
    [InlineData(MeasurementType.HR, 70, true)]
    [InlineData(MeasurementType.HR, 220, true)]
    [InlineData(MeasurementType.HR, 221, false)]
    // RR valid range: (3, 60]
    [InlineData(MeasurementType.RR, 3, false)]
    [InlineData(MeasurementType.RR, 4, true)]
    [InlineData(MeasurementType.RR, 15, true)]
    [InlineData(MeasurementType.RR, 60, true)]
    [InlineData(MeasurementType.RR, 61, false)]
    public void create_validates_value_within_acceptable_range(
        MeasurementType measurementType,
        int value,
        bool shouldSucceed
    )
    {
        var result = Measurement.Create(measurementType, value);

        if (shouldSucceed)
        {
            Assert.True(result.IsOk(), $"Expected success for {measurementType}={value}");
            Assert.Equal(measurementType, result.Value.Type);
            Assert.Equal(value, result.Value.Value);
        }
        else
        {
            Assert.True(result.IsErr(), $"Expected failure for {measurementType}={value}");
            Assert.Equal(DomainErrorType.Validation, result.Error.Type);
        }
    }
}
