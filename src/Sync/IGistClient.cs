using TodoCLI.Tasks;

namespace TodoCLI.Sync;

/// <summary>
/// Interface for GitHub Gist client operations following DIP.
/// Abstracts GitHub Gist API communication for testing and different implementations.
/// </summary>
public interface IGistClient
{
    /// <summary>
    /// Retrieves tasks from the GitHub Gist.
    /// </summary>
    /// <param name="token">GitHub authentication token</param>
    /// <returns>Result containing tasks or error information</returns>
    Task<GistResult<IEnumerable<TodoTask>>> GetTasksAsync(string token);

    /// <summary>
    /// Creates a new GitHub Gist with the provided tasks.
    /// </summary>
    /// <param name="token">GitHub authentication token</param>
    /// <param name="tasks">Tasks to store in the new Gist</param>
    /// <returns>Result indicating success/failure</returns>
    Task<GistResult> CreateGistAsync(string token, IEnumerable<TodoTask> tasks);

    /// <summary>
    /// Updates the existing GitHub Gist with the provided tasks.
    /// </summary>
    /// <param name="token">GitHub authentication token</param>
    /// <param name="tasks">Tasks to update in the Gist</param>
    /// <returns>Result indicating success/failure</returns>
    Task<GistResult> UpdateGistAsync(string token, IEnumerable<TodoTask> tasks);
}