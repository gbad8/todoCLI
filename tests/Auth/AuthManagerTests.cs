using TodoCLI.Auth;

namespace TodoCLI.Tests.Auth;

public class AuthManagerTests
{
    [Fact]
    public void IsAuthenticated_WithNoStoredToken_ShouldReturnFalse()
    {
        // Arrange
        var tokenStorage = new MockTokenStorage();
        var githubClient = new MockGitHubClient();
        var authManager = new AuthManager(tokenStorage, githubClient);

        // Act
        var isAuthenticated = authManager.IsAuthenticated();

        // Assert
        Assert.False(isAuthenticated);
    }

    [Fact]
    public void IsAuthenticated_WithValidStoredToken_ShouldReturnTrue()
    {
        // Arrange
        var tokenStorage = new MockTokenStorage();
        tokenStorage.SetToken("valid_token_12345");
        var githubClient = new MockGitHubClient();
        githubClient.SetValidationResult(true);
        var authManager = new AuthManager(tokenStorage, githubClient);

        // Act
        var isAuthenticated = authManager.IsAuthenticated();

        // Assert
        Assert.True(isAuthenticated);
    }

    [Fact]
    public void IsAuthenticated_WithExpiredToken_ShouldReturnFalse()
    {
        // Arrange
        var tokenStorage = new MockTokenStorage();
        tokenStorage.SetToken("expired_token_67890");
        var githubClient = new MockGitHubClient();
        githubClient.SetValidationResult(false); // Token expired/invalid
        var authManager = new AuthManager(tokenStorage, githubClient);

        // Act
        var isAuthenticated = authManager.IsAuthenticated();

        // Assert
        Assert.False(isAuthenticated);
    }

    [Fact]
    public async Task AuthenticateAsync_WithValidToken_ShouldStoreTokenAndReturnSuccess()
    {
        // Arrange
        var tokenStorage = new MockTokenStorage();
        var githubClient = new MockGitHubClient();
        githubClient.SetValidationResult(true);
        var authManager = new AuthManager(tokenStorage, githubClient);
        var validToken = "ghp_valid_token_12345";

        // Act
        var result = await authManager.AuthenticateAsync(validToken);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Authentication successful.", result.Message);
        Assert.Equal(validToken, tokenStorage.GetToken());
    }

    [Fact]
    public async Task AuthenticateAsync_WithInvalidToken_ShouldReturnFailure()
    {
        // Arrange
        var tokenStorage = new MockTokenStorage();
        var githubClient = new MockGitHubClient();
        githubClient.SetValidationResult(false);
        var authManager = new AuthManager(tokenStorage, githubClient);
        var invalidToken = "invalid_token";

        // Act
        var result = await authManager.AuthenticateAsync(invalidToken);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Invalid token", result.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Null(tokenStorage.GetToken()); // Should not store invalid token
    }

    [Fact]
    public async Task AuthenticateAsync_WithEmptyToken_ShouldThrowArgumentException()
    {
        // Arrange
        var tokenStorage = new MockTokenStorage();
        var githubClient = new MockGitHubClient();
        var authManager = new AuthManager(tokenStorage, githubClient);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            authManager.AuthenticateAsync(""));
        await Assert.ThrowsAsync<ArgumentException>(() => 
            authManager.AuthenticateAsync("   "));
        await Assert.ThrowsAsync<ArgumentException>(() => 
            authManager.AuthenticateAsync(null!));
    }

    [Fact]
    public async Task AuthenticateAsync_WithNetworkError_ShouldReturnNetworkFailure()
    {
        // Arrange
        var tokenStorage = new MockTokenStorage();
        var githubClient = new MockGitHubClient();
        githubClient.SetNetworkError(true);
        var authManager = new AuthManager(tokenStorage, githubClient);
        var token = "some_token";

        // Act
        var result = await authManager.AuthenticateAsync(token);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("network", result.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Null(tokenStorage.GetToken()); // Should not store on network error
    }

    [Fact]
    public async Task AuthenticateAsync_WithInsufficientPermissions_ShouldReturnPermissionFailure()
    {
        // Arrange
        var tokenStorage = new MockTokenStorage();
        var githubClient = new MockGitHubClient();
        githubClient.SetPermissionError(true);
        var authManager = new AuthManager(tokenStorage, githubClient);
        var token = "token_without_gist_scope";

        // Act
        var result = await authManager.AuthenticateAsync(token);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("permission", result.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("gist", result.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Null(tokenStorage.GetToken()); // Should not store token without permissions
    }

    [Fact]
    public void ClearAuthentication_ShouldRemoveStoredToken()
    {
        // Arrange
        var tokenStorage = new MockTokenStorage();
        tokenStorage.SetToken("existing_token");
        var githubClient = new MockGitHubClient();
        var authManager = new AuthManager(tokenStorage, githubClient);

        // Act
        authManager.ClearAuthentication();

        // Assert
        Assert.Null(tokenStorage.GetToken());
        Assert.False(authManager.IsAuthenticated());
    }

    [Fact]
    public async Task GetTokenAsync_WhenAuthenticated_ShouldReturnValidToken()
    {
        // Arrange
        var tokenStorage = new MockTokenStorage();
        var githubClient = new MockGitHubClient();
        githubClient.SetValidationResult(true);
        var authManager = new AuthManager(tokenStorage, githubClient);
        var originalToken = "valid_token_12345";
        
        await authManager.AuthenticateAsync(originalToken);

        // Act
        var retrievedToken = await authManager.GetTokenAsync();

        // Assert
        Assert.Equal(originalToken, retrievedToken);
    }

    [Fact]
    public async Task GetTokenAsync_WhenNotAuthenticated_ShouldReturnNull()
    {
        // Arrange
        var tokenStorage = new MockTokenStorage();
        var githubClient = new MockGitHubClient();
        var authManager = new AuthManager(tokenStorage, githubClient);

        // Act
        var token = await authManager.GetTokenAsync();

        // Assert
        Assert.Null(token);
    }
}

// Mock implementations for testing
public class MockTokenStorage : ITokenStorage
{
    private string? _token;

    public void SetToken(string token) => _token = token;
    public string? GetToken() => _token;
    public void StoreToken(string token) => _token = token;
    public void ClearToken() => _token = null;
}

public class MockGitHubClient : IGitHubClient
{
    private bool _validationResult = true;
    private bool _networkError = false;
    private bool _permissionError = false;

    public void SetValidationResult(bool isValid) => _validationResult = isValid;
    public void SetNetworkError(bool hasError) => _networkError = hasError;
    public void SetPermissionError(bool hasError) => _permissionError = hasError;

    public async Task<GitHubValidationResult> ValidateTokenAsync(string token)
    {
        await Task.Delay(1); // Simulate async operation

        if (_networkError)
            return new GitHubValidationResult 
            { 
                IsValid = false, 
                ErrorType = GitHubErrorType.NetworkError,
                ErrorMessage = "Network connection failed" 
            };

        if (_permissionError)
            return new GitHubValidationResult 
            { 
                IsValid = false, 
                ErrorType = GitHubErrorType.InsufficientPermissions,
                ErrorMessage = "Token lacks required 'gist' permissions" 
            };

        return new GitHubValidationResult 
        { 
            IsValid = _validationResult,
            ErrorType = _validationResult ? GitHubErrorType.None : GitHubErrorType.InvalidToken,
            ErrorMessage = _validationResult ? "" : "Invalid or expired token"
        };
    }
}