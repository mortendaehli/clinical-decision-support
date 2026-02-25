using ClinicalDecisionSupportService.Domain.Common;
using ClinicalDecisionSupportService.Domain.Enums;
using ClinicalDecisionSupportService.Domain.ValueObjects;

namespace ClinicalDecisionSupportService.UnitTests.Domain;

public sealed class MeasurementTests
{
    [Theory]
    // TEMP valid range: (31, 42]
    [InlineData(MeasurementType.Temperature, 31, false)]  // exclusive lower bound
    [InlineData(MeasurementType.Temperature, 32, true)]   // just above lower bound
    [InlineData(MeasurementType.Temperature, 37, true)]   // normal value
    [InlineData(MeasurementType.Temperature, 42, true)]   // inclusive upper bound
    [InlineData(MeasurementType.Temperature, 43, false)]  // above upper bound
    // HR valid range: (25, 220]
    [InlineData(MeasurementType.HeartRate, 25, false)]     // exclusive lower bound
    [InlineData(MeasurementType.HeartRate, 26, true)]      // just above lower bound
    [InlineData(MeasurementType.HeartRate, 70, true)]      // normal value
    [InlineData(MeasurementType.HeartRate, 220, true)]     // inclusive upper bound
    [InlineData(MeasurementType.HeartRate, 221, false)]    // above upper bound
    // RR valid range: (3, 60]
    [InlineData(MeasurementType.RespiratoryRate, 3, false)]  // exclusive lower bound
    [InlineData(MeasurementType.RespiratoryRate, 4, true)]   // just above lower bound
    [InlineData(MeasurementType.RespiratoryRate, 15, true)]  // normal value
    [InlineData(MeasurementType.RespiratoryRate, 60, true)]  // inclusive upper bound
    [InlineData(MeasurementType.RespiratoryRate, 61, false)] // above upper bound
    public void create_validates_value_within_acceptable_range(
        MeasurementType type, int value, bool shouldSucceed)
    {
        var result = Measurement.Create(type, value);

        if (shouldSucceed)
        {
            Assert.True(result.IsOk(), $"Expected success for {type}={value}");
            Assert.Equal(type, result.Value.MeasurementType);
            Assert.Equal(value, result.Value.Value);
        }
        else
        {
            Assert.True(result.IsErr(), $"Expected failure for {type}={value}");
            Assert.Equal(DomainErrorType.Validation, result.Error.Type);
        }
    }
}
