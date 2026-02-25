using ClinicalDecisionSupportService.Domain.Enums;
using ClinicalDecisionSupportService.Domain.Services;
using ClinicalDecisionSupportService.Domain.ValueObjects;

namespace ClinicalDecisionSupportService.UnitTests.Domain;

public sealed class NewsScoreCalculatorTests
{
    private readonly NewsScoreCalculator _sut = new();

    private static Measurement M(MeasurementType type, int value)
    {
        var result = Measurement.Create(type, value);
        if (result.IsErr())
            throw new InvalidOperationException($"Test setup failed: {result.Error}");
        return result.Value;
    }

    [Fact]
    public void calculate_returns_3_for_spec_example()
    {
        // Spec example
        // TEMP=37 → 0, HR=60 → 0, RR=5 → 3 ⇒ total = 3
        var measurements = new[]
        {
            M(MeasurementType.Temperature, 37),
            M(MeasurementType.HeartRate, 60),
            M(MeasurementType.RespiratoryRate, 5),
        };

        var result = _sut.Calculate(measurements);

        Assert.True(result.IsOk());
        Assert.Equal(3, result.Value);
    }

    [Fact]
    public void calculate_returns_0_when_all_measurements_in_normal_range()
    {
        // TEMP=37 → 0, HR=60 → 0, RR=15 → 0 ⇒ total = 0
        var measurements = new[]
        {
            M(MeasurementType.Temperature, 37),
            M(MeasurementType.HeartRate, 60),
            M(MeasurementType.RespiratoryRate, 15),
        };

        var result = _sut.Calculate(measurements);

        Assert.True(result.IsOk());
        Assert.Equal(0, result.Value);
    }

    [Fact]
    public void calculate_returns_9_when_all_measurements_at_max_score()
    {
        // TEMP=32 → 3, HR=30 → 3, RR=5 → 3 ⇒ total = 9
        var measurements = new[]
        {
            M(MeasurementType.Temperature, 32),
            M(MeasurementType.HeartRate, 30),
            M(MeasurementType.RespiratoryRate, 5),
        };

        var result = _sut.Calculate(measurements);

        Assert.True(result.IsOk());
        Assert.Equal(9, result.Value);
    }

    [Theory]
    [InlineData(32, 3)] // middle of (31, 35]
    [InlineData(35, 3)] // inclusive end of first band
    [InlineData(36, 1)] // inclusive end of (35, 36]
    [InlineData(37, 0)] // middle of (36, 38]
    [InlineData(38, 0)] // inclusive end of (36, 38]
    [InlineData(39, 1)] // inclusive end of (38, 39]
    [InlineData(40, 2)] // middle of (39, 42]
    [InlineData(42, 2)] // inclusive end of (39, 42]
    public void temp_scores_correctly(int tempValue, int expectedTempScore)
    {
        var measurements = new[]
        {
            M(MeasurementType.Temperature, tempValue),
            M(MeasurementType.HeartRate, 60), // score 0
            M(MeasurementType.RespiratoryRate, 15), // score 0
        };

        var result = _sut.Calculate(measurements);

        Assert.True(result.IsOk());
        Assert.Equal(expectedTempScore, result.Value);
    }

    // ── HR scoring bands: (25,40]→3  (40,50]→1  (50,90]→0  (90,110]→1  (110,130]→2  (130,220]→3

    [Theory]
    [InlineData(26, 3)] // just above exclusive lower bound
    [InlineData(40, 3)] // inclusive end of (25, 40]
    [InlineData(41, 1)] // just into (40, 50]
    [InlineData(50, 1)] // inclusive end of (40, 50]
    [InlineData(51, 0)] // just into (50, 90]
    [InlineData(90, 0)] // inclusive end of (50, 90]
    [InlineData(91, 1)] // just into (90, 110]
    [InlineData(110, 1)] // inclusive end of (90, 110]
    [InlineData(111, 2)] // just into (110, 130]
    [InlineData(130, 2)] // inclusive end of (110, 130]
    [InlineData(131, 3)] // just into (130, 220]
    [InlineData(220, 3)] // inclusive end of (130, 220]
    public void hr_scores_correctly(int hrValue, int expectedHrScore)
    {
        var measurements = new[]
        {
            M(MeasurementType.Temperature, 37), // score 0
            M(MeasurementType.HeartRate, hrValue),
            M(MeasurementType.RespiratoryRate, 15), // score 0
        };

        var result = _sut.Calculate(measurements);

        Assert.True(result.IsOk());
        Assert.Equal(expectedHrScore, result.Value);
    }

    // ── RR scoring bands: (3,8]→3  (8,11]→1  (11,20]→0  (20,24]→2  (24,60]→3

    [Theory]
    [InlineData(4, 3)] // just above exclusive lower bound
    [InlineData(8, 3)] // inclusive end of (3, 8]
    [InlineData(9, 1)] // just into (8, 11]
    [InlineData(11, 1)] // inclusive end of (8, 11]
    [InlineData(12, 0)] // just into (11, 20]
    [InlineData(20, 0)] // inclusive end of (11, 20]
    [InlineData(21, 2)] // just into (20, 24]
    [InlineData(24, 2)] // inclusive end of (20, 24]
    [InlineData(25, 3)] // just into (24, 60]
    [InlineData(60, 3)] // inclusive end of (24, 60]
    public void rr_scores_correctly(int rrValue, int expectedRrScore)
    {
        var measurements = new[]
        {
            M(MeasurementType.Temperature, 37), // score 0
            M(MeasurementType.HeartRate, 60), // score 0
            M(MeasurementType.RespiratoryRate, rrValue),
        };

        var result = _sut.Calculate(measurements);

        Assert.True(result.IsOk());
        Assert.Equal(expectedRrScore, result.Value);
    }

    // ── Validation ───────────────────────────────────────────────────

    [Fact]
    public void calculate_fails_when_measurement_is_missing()
    {
        var measurements = new[]
        {
            M(MeasurementType.Temperature, 37),
            M(MeasurementType.HeartRate, 60),
            // Missing RR
        };

        var result = _sut.Calculate(measurements);

        Assert.True(result.IsErr());
        Assert.Equal("MEASUREMENT_MISSING", result.Error.Code);
    }

    [Fact]
    public void calculate_fails_when_measurement_is_duplicated()
    {
        var measurements = new[]
        {
            M(MeasurementType.Temperature, 37),
            M(MeasurementType.Temperature, 38),
            M(MeasurementType.HeartRate, 60),
            M(MeasurementType.RespiratoryRate, 15),
        };

        var result = _sut.Calculate(measurements);

        Assert.True(result.IsErr());
        Assert.Equal("MEASUREMENT_DUPLICATE", result.Error.Code);
    }
}
