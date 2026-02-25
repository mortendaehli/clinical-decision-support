using ClinicalDecisionSupportService.Domain.Common;
using ClinicalDecisionSupportService.Domain.ValueObjects;

namespace ClinicalDecisionSupportService.Domain.Services;

public interface INewsScoreCalculator
{
    Result<int, DomainError> Calculate(IReadOnlyCollection<Measurement> measurements);
}
