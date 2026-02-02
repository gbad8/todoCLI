using TodoCLI.Auth;

namespace TodoCLI.Tests.Auth;

public class AuthServiceIntegrationTests
{
    [Fact]
    public void AuthManager_ShouldImplementIAuthService()
    {
        // Arrange
        var tokenStorage = new MockTokenStorage();
        var githubClient = new MockGitHubClient();

        // Act - Create via interface
        IAuthService authService = new AuthManager(tokenStorage, githubClient);

        // Assert
        Assert.NotNull(authService);
        Assert.IsAssignableFrom<IAuthService>(authService);
        Assert.IsType<AuthManager>(authService);
    }

    [Fact]
    public async Task IAuthService_AuthenticateAndCheck_ShouldWorkThroughInterface()
    {
        // Arrange
        var tokenStorage = new MockTokenStorage();
        var githubClient = new MockGitHubClient();
        githubClient.SetValidationResult(true);
        IAuthService authService = new AuthManager(tokenStorage, githubClient);
        var token = "ghp_valid_token_12345";

        // Act
        var authResult = await authService.AuthenticateAsync(token);
        var isAuthenticated = authService.IsAuthenticated();

        // Assert
        Assert.True(authResult.Success);
        Assert.True(isAuthenticated);
    }

    [Fact]
    public async Task IAuthService_GetToken_ShouldWorkThroughInterface()
    {
        // Arrange
        var tokenStorage = new MockTokenStorage();
        var githubClient = new MockGitHubClient();
        githubClient.SetValidationResult(true);
        IAuthService authService = new AuthManager(tokenStorage, githubClient);
        var originalToken = "ghp_test_token_67890";

        // Act
        await authService.AuthenticateAsync(originalToken);
        var retrievedToken = await authService.GetTokenAsync();

        // Assert
        Assert.Equal(originalToken, retrievedToken);
    }

    [Fact]
    public async Task IAuthService_ClearAuthentication_ShouldWorkThroughInterface()
    {
        // Arrange
        var tokenStorage = new MockTokenStorage();
        var githubClient = new MockGitHubClient();
        githubClient.SetValidationResult(true);
        IAuthService authService = new AuthManager(tokenStorage, githubClient);
        
        // Authenticate first
        await authService.AuthenticateAsync("some_token");
        Assert.True(authService.IsAuthenticated());

        // Act - Clear via interface
        authService.ClearAuthentication();

        // Assert
        Assert.False(authService.IsAuthenticated());
        var token = await authService.GetTokenAsync();
        Assert.Null(token);
    }

    [Fact]
    public async Task IAuthService_InvalidToken_ShouldWorkThroughInterface()
    {
        // Arrange
        var tokenStorage = new MockTokenStorage();
        var githubClient = new MockGitHubClient();
        githubClient.SetValidationResult(false);
        IAuthService authService = new AuthManager(tokenStorage, githubClient);

        // Act
        var result = await authService.AuthenticateAsync("invalid_token");

        // Assert
        Assert.False(result.Success);
        Assert.False(authService.IsAuthenticated());
        Assert.Contains("Invalid token", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task IAuthService_NetworkError_ShouldWorkThroughInterface()
    {
        // Arrange
        var tokenStorage = new MockTokenStorage();
        var githubClient = new MockGitHubClient();
        githubClient.SetNetworkError(true);
        IAuthService authService = new AuthManager(tokenStorage, githubClient);

        // Act
        var result = await authService.AuthenticateAsync("some_token");

        // Assert
        Assert.False(result.Success);
        Assert.Contains("network", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void IAuthService_DependencyInjection_ShouldAllowMocking()
    {
        // This test demonstrates that IAuthService can be used for dependency injection
        // and potentially mocked for unit testing other components

        // Arrange
        var tokenStorage = new MockTokenStorage();
        var githubClient = new MockGitHubClient();
        IAuthService authService = new AuthManager(tokenStorage, githubClient);

        // Act - Using interface reference
        var initialAuth = authService.IsAuthenticated();

        // Assert - Behavior is accessible through interface
        Assert.False(initialAuth);
    }
}