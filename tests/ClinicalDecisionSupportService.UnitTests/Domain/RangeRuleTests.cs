using ClinicalDecisionSupportService.Domain.Scoring;

namespace ClinicalDecisionSupportService.UnitTests.Domain;

public sealed class RangeRuleTests
{
    private readonly RangeRule _sut = new(MinExclusive: 10, MaxInclusive: 20);

    [Fact]
    public void contains_returns_false_for_value_at_exclusive_lower_bound()
    {
        Assert.False(_sut.Contains(10));
    }

    [Fact]
    public void contains_returns_true_for_value_just_above_lower_bound()
    {
        Assert.True(_sut.Contains(11));
    }

    [Fact]
    public void contains_returns_true_for_value_at_inclusive_upper_bound()
    {
        Assert.True(_sut.Contains(20));
    }

    [Fact]
    public void contains_returns_false_for_value_just_above_upper_bound()
    {
        Assert.False(_sut.Contains(21));
    }

    [Fact]
    public void contains_returns_true_for_value_in_middle_of_range()
    {
        Assert.True(_sut.Contains(15));
    }

    [Fact]
    public void contains_returns_false_for_value_well_below_range()
    {
        Assert.False(_sut.Contains(0));
    }

    [Fact]
    public void contains_returns_false_for_value_well_above_range()
    {
        Assert.False(_sut.Contains(100));
    }
}
