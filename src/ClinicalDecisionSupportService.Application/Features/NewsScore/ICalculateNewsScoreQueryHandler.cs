using ClinicalDecisionSupportService.Domain.Common;

namespace ClinicalDecisionSupportService.Application.Features.NewsScore;

public interface ICalculateNewsScoreQueryHandler
{
    Task<Result<CalculateNewsScoreResult, DomainError>> Handle(
        CalculateNewsScoreQuery query,
        CancellationToken cancellationToken
    );
}
