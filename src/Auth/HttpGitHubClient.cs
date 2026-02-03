using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using TodoCLI.Auth;

namespace TodoCLI.Auth;

/// <summary>
/// HTTP client for GitHub API operations
/// </summary>
public class HttpGitHubClient : IGitHubClient
{
    private readonly HttpClient _httpClient;

    public HttpGitHubClient()
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri("https://api.github.com/");
        _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("TodoCLI", "1.0"));
    }

    public async Task<GitHubValidationResult> ValidateTokenAsync(string token)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, "user");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                // Check if token has gist permissions by trying to access gists endpoint
                using var gistRequest = new HttpRequestMessage(HttpMethod.Get, "gists");
                gistRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                
                using var gistResponse = await _httpClient.SendAsync(gistRequest);
                
                if (!gistResponse.IsSuccessStatusCode)
                {
                    return new GitHubValidationResult
                    {
                        IsValid = false,
                        ErrorType = GitHubErrorType.InsufficientPermissions,
                        ErrorMessage = "Token does not have gist permissions. Please ensure your personal access token includes 'gist' scope."
                    };
                }

                return new GitHubValidationResult
                {
                    IsValid = true
                };
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return new GitHubValidationResult
                {
                    IsValid = false,
                    ErrorType = GitHubErrorType.InvalidToken,
                    ErrorMessage = "Invalid GitHub token. Please check your personal access token."
                };
            }
            else
            {
                return new GitHubValidationResult
                {
                    IsValid = false,
                    ErrorType = GitHubErrorType.ServerError,
                    ErrorMessage = $"GitHub API error: {response.StatusCode}"
                };
            }
        }
        catch (HttpRequestException ex)
        {
            return new GitHubValidationResult
            {
                IsValid = false,
                ErrorType = GitHubErrorType.NetworkError,
                ErrorMessage = $"Network error: {ex.Message}"
            };
        }
        catch (Exception ex)
        {
            return new GitHubValidationResult
            {
                IsValid = false,
                ErrorType = GitHubErrorType.ServerError,
                ErrorMessage = $"Unexpected error: {ex.Message}"
            };
        }
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}