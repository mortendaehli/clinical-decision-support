using ClinicalDecisionSupportService.Application.Features.NewsScore;
using ClinicalDecisionSupportService.Domain.Common;
using ClinicalDecisionSupportService.Domain.Services;
using ClinicalDecisionSupportService.Domain.ValueObjects;

namespace ClinicalDecisionSupportService.UnitTests;

public sealed class CalculateNewsScoreQueryHandlerTests
{
    [Fact]
    public async Task handle_returns_score_from_injected_calculator()
    {
        var fakeCalculator = new FakeNewsScoreCalculator(expectedScore: 7);
        var sut = new CalculateNewsScoreQueryHandler(fakeCalculator);
        var query = new CalculateNewsScoreQuery([
            new CalculateNewsScoreMeasurementInput("TEMP", 37),
        ]);

        var result = await sut.Handle(query, CancellationToken.None);

        Assert.True(result.IsOk());
        Assert.NotNull(result.Value);
        Assert.Equal(7, result.Value.Score);
        Assert.Equal(1, fakeCalculator.Calls);
    }

    private sealed class FakeNewsScoreCalculator : INewsScoreCalculator
    {
        private readonly int _expectedScore;
        public int Calls { get; private set; }

        public FakeNewsScoreCalculator(int expectedScore)
        {
            _expectedScore = expectedScore;
        }

        public Result<int, DomainError> Calculate(IReadOnlyCollection<Measurement> measurements)
        {
            Calls++;
            return _expectedScore;
        }
    }
}
