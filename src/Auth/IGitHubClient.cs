namespace TodoCLI.Auth;

/// <summary>
/// Interface for GitHub API client operations following DIP.
/// Abstracts GitHub API communication for testing and different implementations.
/// </summary>
public interface IGitHubClient
{
    /// <summary>
    /// Validates a GitHub token with the GitHub API.
    /// </summary>
    /// <param name="token">The GitHub personal access token to validate</param>
    /// <returns>Validation result with status and error details</returns>
    Task<GitHubValidationResult> ValidateTokenAsync(string token);
}