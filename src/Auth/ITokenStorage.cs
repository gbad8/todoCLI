namespace TodoCLI.Auth;

/// <summary>
/// Interface for secure token storage operations following DIP.
/// Allows different implementations for different platforms (Windows, Linux, macOS).
/// </summary>
public interface ITokenStorage
{
    /// <summary>
    /// Stores a GitHub token securely.
    /// </summary>
    /// <param name="token">The GitHub personal access token to store</param>
    void StoreToken(string token);

    /// <summary>
    /// Retrieves the stored GitHub token.
    /// </summary>
    /// <returns>The stored token, or null if no token is stored</returns>
    string? GetToken();

    /// <summary>
    /// Clears the stored token.
    /// </summary>
    void ClearToken();
}