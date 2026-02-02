using TodoCLI.CLI;
using TodoCLI.Tasks;
using TodoCLI.Auth;
using TodoCLI.Sync;
using TodoCLI.Tests.Sync;
using Xunit;

namespace TodoCLI.Tests.CLI;

public class CommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_WithAuthSetupCommand_ShouldCallAuthService()
    {
        // Arrange
        var mockAuthService = new MockAuthService();
        var mockTaskService = new MockTaskService();
        var mockSyncService = new MockSyncService();
        var mockConsole = new MockConsole();
        mockConsole.InputLines.Enqueue("test_token_123"); // Simula user input

        var handler = new CommandHandler(mockAuthService, mockTaskService, mockSyncService, mockConsole);
        var command = new Command(CommandType.Auth, "setup");

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result.Success);
        Assert.Contains("Enter your GitHub personal access token: ", mockConsole.OutputLines);
        Assert.Contains("Authentication successful! You're now ready to sync tasks with GitHub Gist.", mockConsole.OutputLines);
    }

    [Fact]
    public async Task HandleAsync_WithAddCommand_ValidDescription_ShouldAddTask()
    {
        // Arrange
        var mockAuthService = new MockAuthService();
        mockAuthService.SetAuthenticated(true);
        
        var mockTaskService = new MockTaskService();
        var mockSyncService = new MockSyncService();
        var mockConsole = new MockConsole();

        var handler = new CommandHandler(mockAuthService, mockTaskService, mockSyncService, mockConsole);
        var command = new Command(CommandType.Add, "", "Buy groceries");

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(1, mockTaskService.GetTasks().Count());
        var task = mockTaskService.GetTasks().First();
        Assert.Equal("Buy groceries", task.Description);
        Assert.Contains("Task added successfully", mockConsole.OutputLines.Last());
    }

    [Fact]
    public async Task HandleAsync_WithAddCommand_EmptyDescription_ShouldShowError()
    {
        // Arrange
        var mockAuthService = new MockAuthService();
        mockAuthService.SetAuthenticated(true);
        
        var mockTaskService = new MockTaskService();
        var mockSyncService = new MockSyncService();
        var mockConsole = new MockConsole();

        var handler = new CommandHandler(mockAuthService, mockTaskService, mockSyncService, mockConsole);
        var command = new Command(CommandType.Add, "", "");

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Usage: todo add \"task description\"", mockConsole.OutputLines.Last());
    }

    [Fact]
    public async Task HandleAsync_WithListCommand_NoTasks_ShouldShowEmptyMessage()
    {
        // Arrange
        var mockAuthService = new MockAuthService();
        mockAuthService.SetAuthenticated(true);
        
        var mockTaskService = new MockTaskService();
        var mockSyncService = new MockSyncService();
        var mockConsole = new MockConsole();

        var handler = new CommandHandler(mockAuthService, mockTaskService, mockSyncService, mockConsole);
        var command = new Command(CommandType.List);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result.Success);
        Assert.Contains("No tasks found. Use 'todo add \"task\"' to create your first task.", mockConsole.OutputLines);
    }

    [Fact]
    public async Task HandleAsync_WithListCommand_WithTasks_ShouldDisplayTasks()
    {
        // Arrange
        var mockAuthService = new MockAuthService();
        mockAuthService.SetAuthenticated(true);
        
        var mockTaskService = new MockTaskService();
        mockTaskService.AddTask(new TodoTask("abc123", "Buy groceries", TodoStatus.Pending, DateTime.Now));
        mockTaskService.AddTask(new TodoTask("def456", "Walk the dog", TodoStatus.Completed, DateTime.Now.AddMinutes(-30)));
        
        var mockSyncService = new MockSyncService();
        var mockConsole = new MockConsole();

        var handler = new CommandHandler(mockAuthService, mockTaskService, mockSyncService, mockConsole);
        var command = new Command(CommandType.List);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result.Success);
        Assert.Contains("[abc]   Buy groceries", mockConsole.OutputLines); // Note: espaços para status pending
        Assert.Contains("[def] ✓ Walk the dog", mockConsole.OutputLines);
    }

    [Fact]
    public async Task HandleAsync_WithDoneCommand_ValidHash_ShouldCompleteTask()
    {
        // Arrange
        var mockAuthService = new MockAuthService();
        mockAuthService.SetAuthenticated(true);
        
        var mockTaskService = new MockTaskService();
        mockTaskService.AddTask(new TodoTask("abc123", "Buy groceries", TodoStatus.Pending, DateTime.Now));
        
        var mockSyncService = new MockSyncService();
        var mockConsole = new MockConsole();

        var handler = new CommandHandler(mockAuthService, mockTaskService, mockSyncService, mockConsole);
        var command = new Command(CommandType.Done, "", "abc");

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result.Success);
        Assert.Contains("Task completed successfully", mockConsole.OutputLines.Last());
    }

    [Fact]
    public async Task HandleAsync_WithSyncCommand_ShouldCallSyncService()
    {
        // Arrange
        var mockAuthService = new MockAuthService();
        mockAuthService.SetAuthenticated(true);
        
        var mockTaskService = new MockTaskService();
        var mockSyncService = new MockSyncService();
        var mockConsole = new MockConsole();

        var handler = new CommandHandler(mockAuthService, mockTaskService, mockSyncService, mockConsole);
        var command = new Command(CommandType.Sync);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result.Success);
        Assert.True(mockSyncService.SyncAsyncCalled);
        Assert.Contains("Synchronization completed successfully. 0 tasks synced.", mockConsole.OutputLines);
    }

    [Fact]
    public async Task HandleAsync_WithoutAuthentication_ShouldShowAuthError()
    {
        // Arrange
        var mockAuthService = new MockAuthService();
        mockAuthService.SetAuthenticated(false);
        
        var mockTaskService = new MockTaskService();
        var mockSyncService = new MockSyncService();
        var mockConsole = new MockConsole();

        var handler = new CommandHandler(mockAuthService, mockTaskService, mockSyncService, mockConsole);
        var command = new Command(CommandType.Add, "", "Some task");

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Authentication expired. Run 'todo auth setup' to re-authenticate.", mockConsole.OutputLines.Last());
    }

    [Fact]
    public async Task HandleAsync_WithHelpCommand_ShouldShowUsage()
    {
        // Arrange
        var mockAuthService = new MockAuthService();
        var mockTaskService = new MockTaskService();
        var mockSyncService = new MockSyncService();
        var mockConsole = new MockConsole();

        var handler = new CommandHandler(mockAuthService, mockTaskService, mockSyncService, mockConsole);
        var command = new Command(CommandType.Help);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result.Success);
        Assert.Contains("TODO CLI - Task Management Tool", mockConsole.OutputLines);
        Assert.Contains("Commands:", mockConsole.OutputLines);
        Assert.Contains("Examples:", mockConsole.OutputLines);
    }
}

// Mock implementations for testing
public class MockConsole : IConsole
{
    public List<string> OutputLines { get; } = new();
    public Queue<string> InputLines { get; } = new();

    public void WriteLine(string message) => OutputLines.Add(message);
    public void Write(string message) => OutputLines.Add(message);
    public string ReadLine() => InputLines.Count > 0 ? InputLines.Dequeue() : "";
    public ConsoleKeyInfo ReadKey() => new(' ', ConsoleKey.Enter, false, false, false);
}

public class MockSyncService : ISyncService
{
    public bool SyncAsyncCalled { get; private set; }

    public Task<SyncResult> SyncAsync()
    {
        SyncAsyncCalled = true;
        return Task.FromResult(new SyncResult { Success = true, Message = "Mock sync completed" });
    }

    public Task<SyncResult> PushAsync() => throw new NotImplementedException();
    public Task<SyncResult> PullAsync() => throw new NotImplementedException();
}