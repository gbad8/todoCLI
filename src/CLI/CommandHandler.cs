using System;
using System.Linq;
using System.Threading.Tasks;
using TodoCLI.Auth;
using TodoCLI.Tasks;
using TodoCLI.Sync;

namespace TodoCLI.CLI;

/// <summary>
/// Handles command execution by orchestrating service calls and user interaction
/// </summary>
public class CommandHandler
{
    private readonly IAuthService _authService;
    private readonly ITaskService _taskService;
    private readonly ISyncService _syncService;
    private readonly IConsole _console;

    public CommandHandler(IAuthService authService, ITaskService taskService, ISyncService syncService, IConsole console)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        _taskService = taskService ?? throw new ArgumentNullException(nameof(taskService));
        _syncService = syncService ?? throw new ArgumentNullException(nameof(syncService));
        _console = console ?? throw new ArgumentNullException(nameof(console));
    }

    public async Task<CommandResult> HandleAsync(Command command)
    {
        try
        {
            return command.Type switch
            {
                CommandType.Auth => await HandleAuthAsync(command),
                CommandType.Add => await HandleAddAsync(command),
                CommandType.List => await HandleListAsync(command),
                CommandType.Done => await HandleDoneAsync(command),
                CommandType.Remove => await HandleRemoveAsync(command),
                CommandType.DoneAll => await HandleDoneAllAsync(command),
                CommandType.RemoveAll => await HandleRemoveAllAsync(command),
                CommandType.Sync => await HandleSyncAsync(command),
                CommandType.Help => HandleHelp(),
                CommandType.Unknown => HandleUnknown(command),
                _ => CommandResult.Error("Unknown command")
            };
        }
        catch (Exception ex)
        {
            _console.WriteLine($"Error: {ex.Message}");
            return CommandResult.Error(ex.Message);
        }
    }

    private async Task<CommandResult> HandleAuthAsync(Command command)
    {
        if (command.SubCommand == "setup")
        {
            _console.Write("Enter your GitHub personal access token: ");
            var token = _console.ReadLine();

            if (string.IsNullOrWhiteSpace(token))
            {
                _console.WriteLine("Error: Token cannot be empty");
                return CommandResult.Error("Token cannot be empty");
            }

            var result = await _authService.AuthenticateAsync(token);
            
            if (result.Success)
            {
                _console.WriteLine("Authentication successful! You're now ready to sync tasks with GitHub Gist.");
                return CommandResult.Ok("Authentication successful");
            }
            else
            {
                _console.WriteLine($"Authentication failed: {result.Message}");
                return CommandResult.Error(result.Message);
            }
        }

        _console.WriteLine("Usage: todo auth setup");
        return CommandResult.Error("Invalid auth command");
    }

    private async Task<CommandResult> HandleAddAsync(Command command)
    {
        if (!await EnsureAuthenticationAsync())
            return CommandResult.Error("Authentication required");

        if (string.IsNullOrWhiteSpace(command.Arguments))
        {
            _console.WriteLine("Usage: todo add \"task description\"");
            return CommandResult.Error("Task description required");
        }

        var task = _taskService.AddTask(command.Arguments);
        _console.WriteLine($"Task added successfully: [{task.Hash[..3]}] {task.Description}");
        
        return CommandResult.Ok("Task added successfully");
    }

    private async Task<CommandResult> HandleListAsync(Command command)
    {
        if (!await EnsureAuthenticationAsync())
            return CommandResult.Error("Authentication required");

        var tasks = _taskService.ListTasks().ToList();

        if (!tasks.Any())
        {
            _console.WriteLine("No tasks found. Use 'todo add \"task\"' to create your first task.");
            return CommandResult.Ok("No tasks found");
        }

        _console.WriteLine($"You have {tasks.Count} task(s):");
        _console.WriteLine("");

        foreach (var task in tasks.OrderBy(t => t.CreatedAt))
        {
            var status = task.Status == TodoStatus.Completed ? "[X]" : "[ ]";
            var prefix = task.Hash[..3];
            _console.WriteLine($"{prefix} {status} {task.Description}");
        }

        return CommandResult.Ok($"Listed {tasks.Count} tasks");
    }

    private async Task<CommandResult> HandleDoneAsync(Command command)
    {
        if (!await EnsureAuthenticationAsync())
            return CommandResult.Error("Authentication required");

        if (string.IsNullOrWhiteSpace(command.Arguments))
        {
            _console.WriteLine("Usage: todo done <hash-prefix>");
            return CommandResult.Error("Hash prefix required");
        }

        var result = _taskService.CompleteTask(command.Arguments);
        
        if (result.Success)
        {
            _console.WriteLine($"Task completed successfully: [{result.FullHash![..3]}]");
            return CommandResult.Ok("Task completed successfully");
        }
        else
        {
            _console.WriteLine($"Error: {result.ErrorMessage}");
            return CommandResult.Error(result.ErrorMessage);
        }
    }

    private async Task<CommandResult> HandleRemoveAsync(Command command)
    {
        if (!await EnsureAuthenticationAsync())
            return CommandResult.Error("Authentication required");

        if (string.IsNullOrWhiteSpace(command.Arguments))
        {
            _console.WriteLine("Usage: todo rm <hash-prefix>");
            return CommandResult.Error("Hash prefix required");
        }

        var result = _taskService.RemoveTask(command.Arguments);
        
        if (result.Success)
        {
            _console.WriteLine($"Task removed successfully: [{result.FullHash![..3]}]");
            return CommandResult.Ok("Task removed successfully");
        }
        else
        {
            _console.WriteLine($"Error: {result.ErrorMessage}");
            return CommandResult.Error(result.ErrorMessage);
        }
    }

    private async Task<CommandResult> HandleDoneAllAsync(Command command)
    {
        if (!await EnsureAuthenticationAsync())
            return CommandResult.Error("Authentication required");

        var tasks = _taskService.ListTasks().ToList();
        var pendingTasks = tasks.Where(t => t.Status == TodoStatus.Pending).ToList();

        if (!pendingTasks.Any())
        {
            _console.WriteLine("No pending tasks to complete.");
            return CommandResult.Ok("No pending tasks");
        }

        _taskService.CompleteAllTasks();
        _console.WriteLine($"Completed {pendingTasks.Count} task(s).");
        
        return CommandResult.Ok($"Completed {pendingTasks.Count} tasks");
    }

    private async Task<CommandResult> HandleRemoveAllAsync(Command command)
    {
        if (!await EnsureAuthenticationAsync())
            return CommandResult.Error("Authentication required");

        var tasks = _taskService.ListTasks().ToList();

        if (!tasks.Any())
        {
            _console.WriteLine("No tasks to remove.");
            return CommandResult.Ok("No tasks to remove");
        }

        _taskService.RemoveAllTasks();
        _console.WriteLine($"Removed {tasks.Count} task(s).");
        
        return CommandResult.Ok($"Removed {tasks.Count} tasks");
    }

    private async Task<CommandResult> HandleSyncAsync(Command command)
    {
        if (!await EnsureAuthenticationAsync())
            return CommandResult.Error("Authentication required");

        _console.WriteLine("Synchronizing with GitHub Gist...");
        var result = await _syncService.SyncAsync();

        if (result.Success)
        {
            _console.WriteLine($"Synchronization completed successfully. {result.TasksSynced} tasks synced.");
            if (result.ConflictsResolved > 0)
                _console.WriteLine($"Resolved {result.ConflictsResolved} conflict(s).");
        }
        else
        {
            _console.WriteLine($"Synchronization failed: {result.Message}");
            return CommandResult.Error(result.Message);
        }

        return CommandResult.Ok("Synchronization completed successfully");
    }

    private CommandResult HandleHelp()
    {
        _console.WriteLine("TODO CLI - Task Management Tool");
        _console.WriteLine("");
        _console.WriteLine("Commands:");
        _console.WriteLine("  todo auth setup                 - Set up GitHub authentication");
        _console.WriteLine("  todo add \"task description\"     - Add a new task");
        _console.WriteLine("  todo list                       - List all tasks");
        _console.WriteLine("  todo done <hash-prefix>         - Mark task as completed");
        _console.WriteLine("  todo rm <hash-prefix>           - Remove a task");
        _console.WriteLine("  todo done-all                   - Mark all tasks as completed");
        _console.WriteLine("  todo rm all                     - Remove all tasks");
        _console.WriteLine("  todo sync                       - Sync tasks with GitHub Gist");
        _console.WriteLine("  todo help                       - Show this help message");
        _console.WriteLine("");
        _console.WriteLine("Examples:");
        _console.WriteLine("  todo add \"Buy groceries\"");
        _console.WriteLine("  todo done abc123");
        _console.WriteLine("  todo list");

        return CommandResult.Ok("Help displayed");
    }

    private CommandResult HandleUnknown(Command command)
    {
        _console.WriteLine($"Unknown command: {command.Arguments}");
        _console.WriteLine("Use 'todo help' to see available commands.");
        return CommandResult.Error($"Unknown command: {command.Arguments}");
    }

    private async Task<bool> EnsureAuthenticationAsync()
    {
        if (!_authService.IsAuthenticated())
        {
            _console.WriteLine("Authentication expired. Run 'todo auth setup' to re-authenticate.");
            return false;
        }

        return true;
    }
}