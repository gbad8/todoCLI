namespace TodoCLI.Auth;

/// <summary>
/// Result of GitHub token validation operation.
/// </summary>
public class GitHubValidationResult
{
    /// <summary>
    /// Indicates whether the token is valid and has required permissions.
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Type of error if validation failed.
    /// </summary>
    public GitHubErrorType ErrorType { get; set; } = GitHubErrorType.None;

    /// <summary>
    /// Human-readable error message if validation failed.
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;
}

/// <summary>
/// Types of errors that can occur during GitHub operations.
/// </summary>
public enum GitHubErrorType
{
    None,
    InvalidToken,
    NetworkError,
    InsufficientPermissions,
    RateLimited,
    ServerError
}