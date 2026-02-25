using ClinicalDecisionSupportService.Domain.Common;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ClinicalDecisionSupportService.Api.Mappings;

public static class DomainErrorHttpMapper
{
    public static ProblemHttpResult ToProblem(this DomainError error)
    {
        var statusCode = error.Type switch
        {
            DomainErrorType.Validation => StatusCodes.Status400BadRequest,
            DomainErrorType.NotFound => StatusCodes.Status404NotFound,
            DomainErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        var extensions = new Dictionary<string, object?>
        {
            ["code"] = error.Code
        };

        if (!string.IsNullOrWhiteSpace(error.Field))
        {
            extensions["field"] = error.Field;
        }

        return TypedResults.Problem(
            title: error.Code,
            detail: error.Message,
            statusCode: statusCode,
            extensions: extensions
        );
    }
}
