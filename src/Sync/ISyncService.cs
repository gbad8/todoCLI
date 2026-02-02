namespace TodoCLI.Sync;

/// <summary>
/// Interface for synchronization operations following DIP.
/// This abstraction allows other modules (like CLI) to depend on this interface
/// rather than the concrete SyncManager implementation.
/// </summary>
public interface ISyncService
{
    /// <summary>
    /// Synchronizes local tasks with remote GitHub Gist.
    /// Uses "remote wins" strategy for conflict resolution.
    /// </summary>
    /// <returns>Result indicating success/failure and sync statistics</returns>
    Task<SyncResult> SyncAsync();

    /// <summary>
    /// Pushes local tasks to remote GitHub Gist.
    /// </summary>
    /// <returns>Result indicating success/failure</returns>
    Task<SyncResult> PushAsync();

    /// <summary>
    /// Pulls tasks from remote GitHub Gist to local storage.
    /// </summary>
    /// <returns>Result indicating success/failure</returns>
    Task<SyncResult> PullAsync();
}