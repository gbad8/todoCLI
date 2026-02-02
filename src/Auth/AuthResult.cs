namespace TodoCLI.Auth;

/// <summary>
/// Result of authentication operations.
/// </summary>
public class AuthResult
{
    /// <summary>
    /// Indicates whether the authentication operation was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Human-readable message about the authentication result.
    /// </summary>
    public string Message { get; set; } = string.Empty;
}