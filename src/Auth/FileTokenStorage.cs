using TodoCLI.Auth;

namespace TodoCLI.Auth;

/// <summary>
/// Simple file-based token storage for development.
/// In production, would use OS keychain/credential manager.
/// </summary>
public class FileTokenStorage : ITokenStorage
{
    private readonly string _tokenFilePath;

    public FileTokenStorage()
    {
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var appDir = Path.Combine(appDataPath, "TodoCLI");
        Directory.CreateDirectory(appDir);
        _tokenFilePath = Path.Combine(appDir, ".token");
    }

    public string? GetToken()
    {
        try
        {
            if (!File.Exists(_tokenFilePath))
                return null;

            return File.ReadAllText(_tokenFilePath);
        }
        catch
        {
            return null;
        }
    }

    public void StoreToken(string token)
    {
        File.WriteAllText(_tokenFilePath, token);
    }

    public void ClearToken()
    {
        if (File.Exists(_tokenFilePath))
        {
            File.Delete(_tokenFilePath);
        }
    }
}