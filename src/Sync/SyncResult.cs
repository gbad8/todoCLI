namespace TodoCLI.Sync;

/// <summary>
/// Result of synchronization operations.
/// </summary>
public class SyncResult
{
    /// <summary>
    /// Indicates whether the synchronization was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Human-readable message about the sync result.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Number of tasks that were synchronized.
    /// </summary>
    public int TasksSynced { get; set; }

    /// <summary>
    /// Number of conflicts that were resolved.
    /// </summary>
    public int ConflictsResolved { get; set; }
}