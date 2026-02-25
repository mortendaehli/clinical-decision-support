namespace ClinicalDecisionSupportService.Domain.Common;

/// <summary>
/// Represents a minimal domain error model that can be returned from domain/application logic
/// without coupling to transport concerns like HTTP.
/// </summary>
/// <param name="Type">High-level category of the domain error.</param>
/// <param name="Code">Stable, domain-specific error code for mapping and handling.</param>
/// <param name="Message">Human-readable description of the error.</param>
/// <param name="Field">Optional field/property name related to the error.</param>
/// <param name="Exception">Optional underlying exception for diagnostics.</param>
/// <remarks>
/// The API layer can translate <see cref="DomainError"/> to HTTP status codes and ProblemDetails.
/// </remarks>
/// <example>
/// <code><![CDATA[
/// var invalidTemp = DomainError.Validation(
///     code: "MEASUREMENT_OUT_OF_RANGE",
///     message: "TEMP must be > 31 and <= 42",
///     field: "measurements[0].value"
/// );
///
/// var notFound = DomainError.NotFound(
///     code: "SCORING_RULESET_NOT_FOUND",
///     message: "Rule set NEWS2 does not exist"
/// );
///
/// var unexpected = DomainError.Unexpected(
///     code: "UNEXPECTED",
///     message: "An unexpected error occurred"
/// );
/// ]]></code>
/// </example>
public record DomainError(
    DomainErrorType Type,
    string Code,
    string Message,
    string? Field = null,
    Exception? Exception = null
)
{
    public static DomainError Validation(string code, string message, string? field = null) =>
        new(DomainErrorType.Validation, code, message, field);

    public static DomainError NotFound(string code, string message) =>
        new(DomainErrorType.NotFound, code, message);

    public static DomainError Conflict(string code, string message, string? field = null) =>
        new(DomainErrorType.Conflict, code, message, field);

    public static DomainError Unexpected(
        string code,
        string message,
        Exception? exception = null
    ) => new(DomainErrorType.Unexpected, code, message, Exception: exception);

    public static implicit operator DomainError(Exception ex) =>
        Unexpected(
            code: "UNEXPECTED_ERROR",
            message: "An unexpected error occurred.",
            exception: ex
        );

    public DomainError Wrap(string message)
    {
        return this with { Message = $"{message}: {Message}" };
    }

    public override string ToString() => $"{Type} {Code} {Message}";
}

public enum DomainErrorType
{
    Validation = 1,
    NotFound = 2,
    Conflict = 3,
    Unexpected = 4,
}
