using ClinicalDecisionSupportService.Domain.Common;
using ClinicalDecisionSupportService.Domain.ValueObjects;

namespace ClinicalDecisionSupportService.Domain.Services;

public interface IScoringEngine
{
    Result<int, DomainError> Calculate(string modelId, IReadOnlyCollection<Measurement> measurements);
}
