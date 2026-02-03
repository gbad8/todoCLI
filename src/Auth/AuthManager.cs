using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace TodoCLI.Auth;

public class AuthManager : IAuthService
{
    private readonly ITokenStorage _tokenStorage;
    private readonly IGitHubClient _gitHubClient;
    
    // Cache para evitar validação HTTP desnecessária
    private DateTime _lastValidationTime = DateTime.MinValue;
    private bool _lastValidationResult = false;
    private string _lastValidatedToken = "";
    private static readonly TimeSpan CacheValidityPeriod = TimeSpan.FromDays(1);
    
    // Cache persistente no disco
    private static readonly string CacheFile = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "TodoCLI", ".auth_cache");

    public AuthManager(ITokenStorage tokenStorage, IGitHubClient gitHubClient)
    {
        _tokenStorage = tokenStorage ?? throw new ArgumentNullException(nameof(tokenStorage));
        _gitHubClient = gitHubClient ?? throw new ArgumentNullException(nameof(gitHubClient));
        LoadCacheFromDisk();
    }

    public bool IsAuthenticated()
    {
        var token = _tokenStorage.GetToken();
        if (string.IsNullOrEmpty(token))
        {
            _lastValidationResult = false;
            return false;
        }

        // Se o token mudou, cache é inválido
        if (token != _lastValidatedToken)
        {
            _lastValidationTime = DateTime.MinValue;
            _lastValidatedToken = token;
        }

        // Usar cache se ainda válido (24 horas)
        var now = DateTime.UtcNow;
        if (now - _lastValidationTime < CacheValidityPeriod)
        {
            return _lastValidationResult;
        }

        // Cache expirado - validar com GitHub API
        try
        {
            var validationTask = _gitHubClient.ValidateTokenAsync(token);
            validationTask.Wait(); // Convert async to sync for this method
            
            _lastValidationResult = validationTask.Result.IsValid;
            _lastValidationTime = now;
            
            // Salvar cache no disco
            SaveCacheToDisk();
            
            return _lastValidationResult;
        }
        catch
        {
            // If validation fails for any reason, consider it not authenticated
            _lastValidationResult = false;
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
                
                // Atualizar cache com token válido
                _lastValidatedToken = token;
                _lastValidationResult = true;
                _lastValidationTime = DateTime.UtcNow;
                
                // Salvar cache no disco
                SaveCacheToDisk();
                
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
    
    private void LoadCacheFromDisk()
    {
        try
        {
            if (!File.Exists(CacheFile))
                return;
                
            var json = File.ReadAllText(CacheFile);
            var cache = JsonSerializer.Deserialize<AuthCache>(json);
            
            if (cache != null)
            {
                _lastValidationTime = cache.LastValidationTime;
                _lastValidationResult = cache.LastValidationResult;
                _lastValidatedToken = cache.LastValidatedToken ?? "";
            }
        }
        catch
        {
            // Se falhar ao carregar, ignora e usa valores padrão
        }
    }
    
    private void SaveCacheToDisk()
    {
        try
        {
            var directory = Path.GetDirectoryName(CacheFile);
            if (directory != null && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            
            var cache = new AuthCache
            {
                LastValidationTime = _lastValidationTime,
                LastValidationResult = _lastValidationResult,
                LastValidatedToken = _lastValidatedToken
            };
            
            var json = JsonSerializer.Serialize(cache);
            File.WriteAllText(CacheFile, json);
        }
        catch
        {
            // Se falhar ao salvar, continua sem cache persistente
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

// Classe auxiliar para serialização do cache
internal class AuthCache
{
    public DateTime LastValidationTime { get; set; }
    public bool LastValidationResult { get; set; }
    public string? LastValidatedToken { get; set; }
}