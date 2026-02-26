using ClinicalDecisionSupportService.Domain.Enums;
using ClinicalDecisionSupportService.Domain.Scoring;
using ClinicalDecisionSupportService.Domain.Services;
using ClinicalDecisionSupportService.Domain.ValueObjects;

namespace ClinicalDecisionSupportService.UnitTests.Domain;

public sealed class ScoringEngineTests
{
    private readonly IScoringEngine _sut = new ScoringEngine([new NewsScoringModel()]);

    private static Measurement M(MeasurementType measurementType, int value)
    {
        var result = Measurement.Create(measurementType, value);
        if (result.IsErr())
            throw new InvalidOperationException($"Test setup failed: {result.Error}");
        return result.Value;
    }

    [Fact]
    public void calculate_returns_3_for_spec_example()
    {
        var measurements = new[] { M(MeasurementType.TEMP, 37), M(MeasurementType.HR, 60), M(MeasurementType.RR, 5) };

        var result = _sut.Calculate("NEWS", measurements);

        Assert.True(result.IsOk());
        Assert.Equal(3, result.Value);
    }

    [Fact]
    public void calculate_returns_0_when_all_measurements_in_normal_range()
    {
        var measurements = new[] { M(MeasurementType.TEMP, 37), M(MeasurementType.HR, 60), M(MeasurementType.RR, 15) };

        var result = _sut.Calculate("NEWS", measurements);

        Assert.True(result.IsOk());
        Assert.Equal(0, result.Value);
    }

    [Fact]
    public void calculate_returns_9_when_all_measurements_at_max_score()
    {
        var measurements = new[] { M(MeasurementType.TEMP, 32), M(MeasurementType.HR, 30), M(MeasurementType.RR, 5) };

        var result = _sut.Calculate("NEWS", measurements);

        Assert.True(result.IsOk());
        Assert.Equal(9, result.Value);
    }

    [Theory]
    [InlineData(32, 3)]
    [InlineData(35, 3)]
    [InlineData(36, 1)]
    [InlineData(37, 0)]
    [InlineData(38, 0)]
    [InlineData(39, 1)]
    [InlineData(40, 2)]
    [InlineData(42, 2)]
    public void temp_scores_correctly(int tempValue, int expectedTempScore)
    {
        var measurements = new[] { M(MeasurementType.TEMP, tempValue), M(MeasurementType.HR, 60), M(MeasurementType.RR, 15) };

        var result = _sut.Calculate("NEWS", measurements);

        Assert.True(result.IsOk());
        Assert.Equal(expectedTempScore, result.Value);
    }

    [Theory]
    [InlineData(26, 3)]
    [InlineData(40, 3)]
    [InlineData(41, 1)]
    [InlineData(50, 1)]
    [InlineData(51, 0)]
    [InlineData(90, 0)]
    [InlineData(91, 1)]
    [InlineData(110, 1)]
    [InlineData(111, 2)]
    [InlineData(130, 2)]
    [InlineData(131, 3)]
    [InlineData(220, 3)]
    public void hr_scores_correctly(int hrValue, int expectedHrScore)
    {
        var measurements = new[] { M(MeasurementType.TEMP, 37), M(MeasurementType.HR, hrValue), M(MeasurementType.RR, 15) };

        var result = _sut.Calculate("NEWS", measurements);

        Assert.True(result.IsOk());
        Assert.Equal(expectedHrScore, result.Value);
    }

    [Theory]
    [InlineData(4, 3)]
    [InlineData(8, 3)]
    [InlineData(9, 1)]
    [InlineData(11, 1)]
    [InlineData(12, 0)]
    [InlineData(20, 0)]
    [InlineData(21, 2)]
    [InlineData(24, 2)]
    [InlineData(25, 3)]
    [InlineData(60, 3)]
    public void rr_scores_correctly(int rrValue, int expectedRrScore)
    {
        var measurements = new[] { M(MeasurementType.TEMP, 37), M(MeasurementType.HR, 60), M(MeasurementType.RR, rrValue) };

        var result = _sut.Calculate("NEWS", measurements);

        Assert.True(result.IsOk());
        Assert.Equal(expectedRrScore, result.Value);
    }

    [Fact]
    public void calculate_fails_when_measurement_is_missing()
    {
        var measurements = new[]
        {
            M(MeasurementType.TEMP, 37),
            M(MeasurementType.HR, 60),
        };

        var result = _sut.Calculate("NEWS", measurements);

        Assert.True(result.IsErr());
        Assert.Equal("MEASUREMENT_MISSING", result.Error.Code);
    }

    [Fact]
    public void calculate_fails_when_measurement_is_duplicated()
    {
        var measurements = new[] { M(MeasurementType.TEMP, 37), M(MeasurementType.TEMP, 38), M(MeasurementType.HR, 60), M(MeasurementType.RR, 15) };

        var result = _sut.Calculate("NEWS", measurements);

        Assert.True(result.IsErr());
        Assert.Equal("MEASUREMENT_DUPLICATE", result.Error.Code);
    }
}
