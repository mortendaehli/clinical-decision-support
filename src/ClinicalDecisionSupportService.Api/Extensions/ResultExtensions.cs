using ClinicalDecisionSupportService.Api.Mappings;
using ClinicalDecisionSupportService.Domain.Common;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ClinicalDecisionSupportService.Api.Extensions;

public static class ResultExtensions
{
    public static Results<Ok<TResponse>, ProblemHttpResult> ToHttpResult<TData, TResponse>(
        this Result<TData, DomainError> result,
        Func<TData, TResponse> mapOk
    ) =>
        result.Match<Results<Ok<TResponse>, ProblemHttpResult>>(
            ok => TypedResults.Ok(mapOk(ok)),
            err => err.ToProblem()
        );
}
