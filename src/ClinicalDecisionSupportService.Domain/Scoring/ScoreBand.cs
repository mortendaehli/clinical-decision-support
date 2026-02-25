namespace ClinicalDecisionSupportService.Domain.Scoring;

public readonly record struct ScoreBand(RangeRule Range, int Score)
{
    public bool Matches(int value) => Range.Contains(value);
}
