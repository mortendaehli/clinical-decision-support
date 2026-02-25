using System.Diagnostics.CodeAnalysis;

namespace ClinicalDecisionSupportService.Domain.Common;

#pragma warning disable CA1815 // Override equals and operator equals on value types
#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1715 // Identifiers should have correct prefix
/// <summary>
/// Represents a unit type that indicates the absence of a specific value.
/// This is commonly used as a return type for operations that complete successfully
/// but don't return meaningful data (similar to void but in a <see cref="Result{T,E}"/> context).
/// </summary>
/// <remarks>
/// <see cref="Unit"/> is used in functional programming patterns where you need a type
/// to represent "no value" in contexts that require a type parameter.
///
/// Common use cases:
/// - Metadata update operations that return success/failure without data
/// - Delete operations that confirm completion
/// - Validation operations that return pass/fail status
/// </remarks>
/// <example>
/// <code><![CDATA[
/// // Method that returns Unit on success
/// public async Task<Result<Unit, DomainError>> UpdateMetadataAsync()
/// {
///     // Perform update operation
///     return Unit.Default; // Indicates successful completion
/// }
///
/// // Usage
/// var result = await UpdateMetadataAsync();
/// if (result.IsOk())
/// {
///     // Operation completed successfully
/// }
/// ]]></code>
/// </example>
public struct Unit
#pragma warning restore CA1815, CA2225, CA1715
{
    public static readonly Unit Default;

    public override string ToString() => "()";
}

/// <summary>
/// A functional programming result type that represents either a successful value of type <typeparamref name="T"/>
/// or an error of type <typeparamref name="E"/>. This eliminates the need for exception-based error handling
/// in healthcare API operations where explicit error handling is critical.
/// </summary>
/// <typeparam name="T">The type of the successful result value (e.g., <see cref="HealthRecordDocument{TData}"/>, <see cref="Unit"/>).</typeparam>
/// <typeparam name="E">The type of the error value (typically <see cref="DomainError"/>).</typeparam>
/// <remarks>
/// The <see cref="Result{T,E}"/> type enforces explicit error handling in healthcare applications where
/// data integrity and error visibility are paramount. Instead of throwing exceptions, methods return
/// a <see cref="Result{T,E}"/> that must be checked for success or failure.
///
/// Key benefits:
/// - **Explicit Error Handling**: Callers must check for errors, preventing silent failures
/// - **Type Safety**: Compile-time guarantee that errors are handled
/// - **Performance**: No exception throwing/catching overhead
/// - **Composability**: Results can be chained and transformed functionally
/// - **Audit Trail**: Clear success/failure paths for healthcare compliance
///
/// Common patterns:
/// - Use <see cref="IsOk()"/> and <see cref="IsErr()"/> for simple success/failure checks
/// - Use <see cref="Match{U}(Func{T,U}, Func{E,U})"/> for functional transformations
/// - Use <see cref="Map{K}(Func{T,K})"/> to transform successful values
/// - Use implicit conversions for easy result creation
/// </remarks>
/// <example>
/// <code><![CDATA[
/// // Creating results
/// Result<Patient, DomainError> success = new Patient { Id = Guid.NewGuid() };
/// Result<Patient, DomainError> failure = DomainError.NotFound("PATIENT_NOT_FOUND", "Patient was not found");
///
/// // Checking results
/// var patientResult = await GetPatientAsync(patientId);
/// if (patientResult.IsOk())
/// {
///     var patient = patientResult.Value;
///     // Use patient data
/// }
/// else
/// {
///     var error = patientResult.Error;
///     // Handle error (log, return error response, etc.)
/// }
///
/// // Functional pattern matching
/// var response = patientResult.Match(
///     patient => $"Found patient: {patient.Name}",
///     error => $"Error: {error.Message}"
/// );
///
/// // Transforming successful results
/// var nameResult = patientResult.Map(patient => patient.Name);
///
/// // Pattern matching with actions
/// patientResult.Match(
///     patient => logger.LogInformation("Patient retrieved: {PatientId}", patient.Id),
///     error => logger.LogError("Failed to retrieve patient: {Error}", error)
/// );
/// ]]></code>
/// </example>
public readonly record struct Result<T, E>
{
    public readonly T? Value { get; private init; }

    public readonly E? Error { get; private init; }

    private readonly bool _isOk { get; init; }

    public Result(T value)
    {
        Value = value;
        Error = default(E);
        _isOk = true;
    }

    public Result(E error)
    {
        Value = default(T);
        Error = error;
        _isOk = false;
    }

    /// <summary>
    /// Creates a successful <see cref="Result{T,E}"/> containing the specified value.
    /// </summary>
    /// <param name="value">The successful result value.</param>
    /// <returns>A <see cref="Result{T,E}"/> in the success state.</returns>
    public static Result<T, E> Ok(T value) => new Result<T, E>(value);

    /// <summary>
    /// Creates a failed <see cref="Result{T,E}"/> containing the specified error.
    /// </summary>
    /// <param name="error">The error value.</param>
    /// <returns>A <see cref="Result{T,E}"/> in the error state.</returns>
    public static Result<T, E> Err(E error) => new Result<T, E>(error);

    public static implicit operator Result<T, E>(T result) => new Result<T, E>(result);

    public static implicit operator Result<T, E>(E error) => new Result<T, E>(error);

    /// <summary>
    /// Determines whether this <see cref="Result{T,E}"/> represents a successful operation.
    /// </summary>
    /// <returns><c>true</c> if the result contains a successful value; otherwise, <c>false</c>.</returns>
    /// <remarks>
    /// When this method returns <c>true</c>, the <see cref="Value"/> property is guaranteed to be non-null.
    /// This is the primary method for checking result success in healthcare operations.
    /// </remarks>
    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsOk() => _isOk;

    /// <summary>
    /// Determines whether this <see cref="Result{T,E}"/> represents a failed operation.
    /// </summary>
    /// <returns><c>true</c> if the result contains an error; otherwise, <c>false</c>.</returns>
    /// <remarks>
    /// When this method returns <c>true</c>, the <see cref="Error"/> property is guaranteed to be non-null.
    /// Use this method to check for errors in healthcare operations before accessing error details.
    /// </remarks>
    [MemberNotNullWhen(false, nameof(Value))]
    [MemberNotNullWhen(true, nameof(Error))]
    public bool IsErr() => !_isOk;

    public T Unwrap()
    {
        if (IsErr())
        {
            throw new Exception(Error!.ToString());
        }
        return Value!;
    }

    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(false, nameof(Error))]
    public bool Ok([NotNullWhen(true)] out T? value)
    {
        value = Value;
        return _isOk;
    }

    [MemberNotNullWhen(true, nameof(Error))]
    [MemberNotNullWhen(false, nameof(Value))]
    public bool Err([NotNullWhen(true)] out E? err)
    {
        err = Error;
        return !_isOk;
    }

    /// <summary>
    /// Transforms this <see cref="Result{T,E}"/> into a value of type <typeparamref name="U"/> by applying
    /// the appropriate function based on whether the result is successful or contains an error.
    /// </summary>
    /// <typeparam name="U">The type of the transformed result.</typeparam>
    /// <param name="ok">Function to apply if the result is successful. Receives the <see cref="Value"/>.</param>
    /// <param name="err">Function to apply if the result contains an error. Receives the <see cref="Error"/>.</param>
    /// <returns>The result of applying either <paramref name="ok"/> or <paramref name="err"/>.</returns>
    /// <remarks>
    /// This is the primary functional programming pattern for handling <see cref="Result{T,E}"/> values.
    /// It ensures that both success and error cases are explicitly handled.
    /// </remarks>
    /// <example>
    /// <code><![CDATA[
    /// var message = result.Match(
    ///     document => $"Created document {document.Id}",
    ///     error => $"Failed: {error.Message}"
    /// );
    /// ]]></code>
    /// </example>
    public U Match<U>(Func<T, U> ok, Func<E, U> err) => IsOk() ? ok(Value!) : err(Error!);

    /// <summary>
    /// Executes the appropriate action based on whether this <see cref="Result{T,E}"/> is successful or contains an error.
    /// </summary>
    /// <param name="ok">Action to execute if the result is successful. Receives the <see cref="Value"/>.</param>
    /// <param name="err">Action to execute if the result contains an error. Receives the <see cref="Error"/>.</param>
    /// <remarks>
    /// Use this overload when you need to perform side effects (like logging) based on the result state,
    /// rather than transforming the result into another value.
    /// </remarks>
    /// <example>
    /// <code><![CDATA[
    /// result.Match(
    ///     document => logger.LogInformation("Document created: {Id}", document.Id),
    ///     error => logger.LogError("Document creation failed: {Error}", error.Message)
    /// );
    /// ]]></code>
    /// </example>
    public void Match(Action<T> ok, Action<E> err)
    {
        if (IsErr())
        {
            err(Error!);
            return;
        }
        ok(Value!);
    }

    /// <summary>
    /// Transforms the successful value of this <see cref="Result{T,E}"/> using the specified mapper function.
    /// If the result contains an error, the error is passed through unchanged.
    /// </summary>
    /// <typeparam name="K">The type of the transformed successful value.</typeparam>
    /// <param name="mapper">Function to transform the successful value.</param>
    /// <returns>A new <see cref="Result{T,E}"/> with the transformed value or the original error.</returns>
    /// <remarks>
    /// This method enables functional composition of operations on successful results while preserving
    /// error information. The mapper function is only called if the result is successful.
    /// </remarks>
    /// <example>
    /// <code><![CDATA[
    /// // Transform a document result to extract just the ID
    /// Result<Guid, DomainError> documentIdResult = documentResult.Map(doc => doc.Id);
    ///
    /// // Chain transformations
    /// var summaryResult = documentResult
    ///     .Map(doc => doc.Title)
    ///     .Map(title => $"Document: {title}");
    /// ]]></code>
    /// </example>
    public Result<K, E> Map<K>(Func<T, K> mapper)
    {
        return IsOk() ? Result<K, E>.Ok(mapper(Value!)) : Result<K, E>.Err(Error!);
    }

    /// <summary>
    /// Transforms the error value of this <see cref="Result{T,E}"/> using the specified mapper function.
    /// If the result contains a successful value, the success value is passed through unchanged.
    /// </summary>
    /// <typeparam name="K">The type of the transformed error value.</typeparam>
    /// <param name="mapper">Function to transform the error value.</param>
    /// <returns>A new <see cref="Result{T,E}"/> with the original value or the transformed error.</returns>
    /// <remarks>
    /// This method enables error transformation and conversion in functional composition patterns.
    /// The mapper function is only called if the result contains an error. Common use cases include
    /// converting between different error types or enriching error information.
    /// </remarks>
    /// <example>
    /// <code><![CDATA[
    /// // Convert DomainError to a more specific error type
    /// Result<Document, ValidationError> validationResult = documentResult.MapErr(httpError =>
    ///     new ValidationError($"Document validation failed: {domainError.Message}")
    /// );
    ///
    /// // Enrich error with additional context
    /// var enrichedResult = patientResult.MapErr(error =>
    ///     error with { Message = $"Patient lookup failed: {error.Message}" }
    /// );
    /// ]]></code>
    /// </example>
    public Result<T, K> MapErr<K>(Func<E, K> mapper)
    {
        return IsOk() ? Result<T, K>.Ok(Value!) : Result<T, K>.Err(mapper(Error!));
    }
}
