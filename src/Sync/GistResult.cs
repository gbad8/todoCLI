namespace TodoCLI.Sync;

/// <summary>
/// Result of GitHub Gist operations.
/// </summary>
public class GistResult
{
    /// <summary>
    /// Indicates whether the operation was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Error message if the operation failed.
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;
}

/// <summary>
/// Result of GitHub Gist operations that return data.
/// </summary>
/// <typeparam name="T">Type of data returned</typeparam>
public class GistResult<T> : GistResult
{
    /// <summary>
    /// Data returned from the operation if successful.
    /// </summary>
    public T? Data { get; set; }
}