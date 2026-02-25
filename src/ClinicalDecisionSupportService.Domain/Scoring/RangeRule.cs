namespace ClinicalDecisionSupportService.Domain.Scoring;

// NOTE: FHIR Observation.referenceRange expresses bounds as low/high (both Quantity),
// and by convention both are inclusive (>= low, <= high). FHIR has no explicit
// inclusive/exclusive flag; use referenceRange.text to clarify non-standard bounds
// (e.g. "> 5.0 mg/L"). This domain model uses MinExclusive by design for NEWS scoring
// rules, which differs from FHIR's default inclusive interpretation.
public readonly record struct RangeRule(int MinExclusive, int MaxInclusive)
{
    public bool Contains(int value) => value > MinExclusive && value <= MaxInclusive;

    public override string ToString() => $"> {MinExclusive} and <= {MaxInclusive}";
}
