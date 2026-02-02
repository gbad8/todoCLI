namespace TodoCLI.Auth;

public class AuthManager : IAuthService
{
    private readonly ITokenStorage _tokenStorage;
    private readonly IGitHubClient _gitHubClient;

    public AuthManager(ITokenStorage tokenStorage, IGitHubClient gitHubClient)
    {
        _tokenStorage = tokenStorage ?? throw new ArgumentNullException(nameof(tokenStorage));
        _gitHubClient = gitHubClient ?? throw new ArgumentNullException(nameof(gitHubClient));
    }

    public bool IsAuthenticated()
    {
        var token = _tokenStorage.GetToken();
        if (string.IsNullOrEmpty(token))
            return false;

        // For this simple implementation, we'll do a quick sync validation
        // In production, we might want to cache validation results to avoid hitting the API every time
        try
        {
            var validationTask = _gitHubClient.ValidateTokenAsync(token);
            validationTask.Wait(); // Convert async to sync for this method
            return validationTask.Result.IsValid;
        }
        catch
        {
            // If validation fails for any reason, consider it not authenticated
            return false;
        }
    }

    public async Task<AuthResult> AuthenticateAsync(string token)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token cannot be empty.", nameof(token));

        try
        {
            // Validate token with GitHub API
            var validationResult = await _gitHubClient.ValidateTokenAsync(token);

            if (validationResult.IsValid)
            {
                // Store the valid token
                _tokenStorage.StoreToken(token);
                return new AuthResult 
                { 
                    Success = true, 
                    Message = "Authentication successful." 
                };
            }
            else
            {
                // Handle different types of validation failures
                return validationResult.ErrorType switch
                {
                    GitHubErrorType.NetworkError => new AuthResult 
                    { 
                        Success = false, 
                        Message = "Network connection failed. Please check your internet connection." 
                    },
                    GitHubErrorType.InsufficientPermissions => new AuthResult 
                    { 
                        Success = false, 
                        Message = "Token lacks required permissions. Ensure 'gist' scope is enabled." 
                    },
                    GitHubErrorType.InvalidToken => new AuthResult 
                    { 
                        Success = false, 
                        Message = "Invalid token. Please check your GitHub token." 
                    },
                    _ => new AuthResult 
                    { 
                        Success = false, 
                        Message = validationResult.ErrorMessage 
                    }
                };
            }
        }
        catch (Exception ex)
        {
            return new AuthResult 
            { 
                Success = false, 
                Message = $"Authentication failed: {ex.Message}" 
            };
        }
    }

    public void ClearAuthentication()
    {
        _tokenStorage.ClearToken();
    }

    public async Task<string?> GetTokenAsync()
    {
        await Task.CompletedTask; // Make it async-compatible
        
        if (!IsAuthenticated())
            return null;
            
        return _tokenStorage.GetToken();
    }
}