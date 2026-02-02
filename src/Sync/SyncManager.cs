using TodoCLI.Auth;
using TodoCLI.Tasks;

namespace TodoCLI.Sync;

public class SyncManager : ISyncService
{
    private readonly IAuthService _authService;
    private readonly IGistClient _gistClient;
    private readonly ITaskService _taskService;

    public SyncManager(IAuthService authService, IGistClient gistClient, ITaskService taskService)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        _gistClient = gistClient ?? throw new ArgumentNullException(nameof(gistClient));
        _taskService = taskService ?? throw new ArgumentNullException(nameof(taskService));
    }

    public async Task<SyncResult> SyncAsync()
    {
        // 1. Check authentication
        if (!_authService.IsAuthenticated())
        {
            return new SyncResult
            {
                Success = false,
                Message = "Authentication required. Please run auth setup first."
            };
        }

        try
        {
            var token = await _authService.GetTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                return new SyncResult
                {
                    Success = false,
                    Message = "Authentication token not found."
                };
            }

            // 2. Get local tasks
            var localTasks = _taskService.ListTasks().ToList();

            // 3. Try to get remote tasks
            var remoteResult = await _gistClient.GetTasksAsync(token);
            
            if (!remoteResult.Success)
            {
                // No remote gist exists - create one with local tasks
                if (remoteResult.ErrorMessage.Contains("not found", StringComparison.OrdinalIgnoreCase))
                {
                    var createResult = await _gistClient.CreateGistAsync(token, localTasks);
                    if (!createResult.Success)
                    {
                        return new SyncResult
                        {
                            Success = false,
                            Message = $"Failed to create remote gist: {createResult.ErrorMessage}"
                        };
                    }
                    return new SyncResult
                    {
                        Success = true,
                        Message = "Created remote gist with local tasks.",
                        TasksSynced = localTasks.Count
                    };
                }
                else
                {
                    return new SyncResult
                    {
                        Success = false,
                        Message = $"Failed to fetch remote tasks: {remoteResult.ErrorMessage}"
                    };
                }
            }

            var remoteTasks = remoteResult.Data?.ToList() ?? [];

            // 4. Merge tasks (remote wins for conflicts)
            var mergedTasks = new List<TodoTask>();
            var conflictsResolved = 0;

            // Create lookup for efficient comparison
            var localTasksById = localTasks.ToDictionary(t => t.Hash);
            var remoteTasksById = remoteTasks.ToDictionary(t => t.Hash);

            // Process all unique task IDs
            var allTaskIds = localTasksById.Keys.Union(remoteTasksById.Keys);

            foreach (var taskId in allTaskIds)
            {
                var hasLocal = localTasksById.TryGetValue(taskId, out var localTask);
                var hasRemote = remoteTasksById.TryGetValue(taskId, out var remoteTask);

                if (hasLocal && hasRemote)
                {
                    // Both exist - resolve conflict by timestamp (newer wins)
                    if (localTask!.CreatedAt != remoteTask!.CreatedAt || 
                        localTask.Description != remoteTask.Description ||
                        localTask.Status != remoteTask.Status)
                    {
                        conflictsResolved++;
                    }
                    
                    // Use the more recent task (or remote if timestamps are equal)
                    var useRemote = remoteTask.CreatedAt >= localTask.CreatedAt;
                    mergedTasks.Add(useRemote ? remoteTask : localTask);
                }
                else if (hasRemote)
                {
                    // Only in remote - add to local (new remote task)
                    mergedTasks.Add(remoteTask!);
                }
                else if (hasLocal)
                {
                    // Only in local - keep for remote update (new local task)
                    mergedTasks.Add(localTask!);
                }
            }

            // 5. Update local tasks to match merged state
            _taskService.SetTasks(mergedTasks);

            // 6. Update remote with merged tasks
            var updateResult = await _gistClient.UpdateGistAsync(token, mergedTasks);
            if (!updateResult.Success)
            {
                return new SyncResult
                {
                    Success = false,
                    Message = $"Failed to update remote gist: {updateResult.ErrorMessage}"
                };
            }

            return new SyncResult
            {
                Success = true,
                Message = "Synchronization completed successfully.",
                TasksSynced = mergedTasks.Count,
                ConflictsResolved = conflictsResolved
            };
        }
        catch (Exception ex)
        {
            return new SyncResult
            {
                Success = false,
                Message = $"Sync failed with error: {ex.Message}"
            };
        }
    }

    public async Task<SyncResult> PushAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<SyncResult> PullAsync()
    {
        throw new NotImplementedException();
    }
}