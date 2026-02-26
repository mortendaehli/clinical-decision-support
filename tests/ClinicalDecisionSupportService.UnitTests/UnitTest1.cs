using ClinicalDecisionSupportService.Application.Features.NewsScore;
using ClinicalDecisionSupportService.Domain.Common;
using ClinicalDecisionSupportService.Domain.Services;
using ClinicalDecisionSupportService.Domain.ValueObjects;

namespace ClinicalDecisionSupportService.UnitTests;

public sealed class CalculateNewsScoreQueryHandlerTests
{
    [Fact]
    public async Task handle_returns_score_from_injected_scoring_engine()
    {
        var fakeScoringEngine = new FakeScoringEngine(expectedScore: 7);
        var sut = new CalculateNewsScoreQueryHandler(fakeScoringEngine);
        var query = new CalculateNewsScoreQuery([
            new CalculateNewsScoreMeasurementInput("TEMP", 37),
            new CalculateNewsScoreMeasurementInput("HR", 60),
            new CalculateNewsScoreMeasurementInput("RR", 15),
        ]);

        var result = await sut.Handle(query, CancellationToken.None);

        Assert.True(result.IsOk());
        Assert.NotNull(result.Value);
        Assert.Equal(7, result.Value.Score);
        Assert.Equal(1, fakeScoringEngine.Calls);
        Assert.Equal("NEWS", fakeScoringEngine.LastModelId);
    }

    [Fact]
    public async Task handle_returns_validation_error_for_lowercase_measurement_type()
    {
        var fakeScoringEngine = new FakeScoringEngine(expectedScore: 7);
        var sut = new CalculateNewsScoreQueryHandler(fakeScoringEngine);
        var query = new CalculateNewsScoreQuery([
            new CalculateNewsScoreMeasurementInput("temp", 37),
        ]);

        var result = await sut.Handle(query, CancellationToken.None);

        Assert.True(result.IsErr());
        Assert.NotNull(result.Error);
        Assert.Equal("MEASUREMENT_TYPE_INVALID", result.Error.Code);
        Assert.Equal(0, fakeScoringEngine.Calls);
    }

    private sealed class FakeScoringEngine : IScoringEngine
    {
        private readonly int _expectedScore;
        public int Calls { get; private set; }
        public string? LastModelId { get; private set; }

        public FakeScoringEngine(int expectedScore)
        {
            _expectedScore = expectedScore;
        }

        public Result<int, DomainError> Calculate(string modelId, IReadOnlyCollection<Measurement> measurements)
        {
            Calls++;
            LastModelId = modelId;
            return _expectedScore;
        }
    }
}
