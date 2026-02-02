namespace TodoCLI.Auth;

/// <summary>
/// Interface for authentication operations following Dependency Inversion Principle.
/// This abstraction allows other modules (like CLI, Sync) to depend on this interface
/// rather than the concrete AuthManager implementation.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Checks if the user is currently authenticated with valid credentials.
    /// </summary>
    /// <returns>True if authenticated with valid token, false otherwise</returns>
    bool IsAuthenticated();

    /// <summary>
    /// Authenticates the user with a GitHub personal access token.
    /// </summary>
    /// <param name="token">GitHub personal access token</param>
    /// <returns>Result indicating success/failure with message</returns>
    /// <exception cref="ArgumentException">Thrown when token is null or empty</exception>
    Task<AuthResult> AuthenticateAsync(string token);

    /// <summary>
    /// Retrieves the current authentication token if authenticated.
    /// </summary>
    /// <returns>The stored token if authenticated, null otherwise</returns>
    Task<string?> GetTokenAsync();

    /// <summary>
    /// Clears the current authentication, logging out the user.
    /// </summary>
    void ClearAuthentication();
}