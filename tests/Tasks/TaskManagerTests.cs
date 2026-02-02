using TodoCLI.Tasks;

namespace TodoCLI.Tests.Tasks;

public class TaskManagerTests
{
    [Fact]
    public void AddTask_WithValidDescription_ShouldAddTaskAndReturnIt()
    {
        // Arrange
        var hashGenerator = new HashGenerator();
        var taskManager = new TaskManager(hashGenerator);
        var description = "Fix authentication bug";

        // Act
        var result = taskManager.AddTask(description);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(description, result.Description);
        Assert.Equal(TodoStatus.Pending, result.Status);
        Assert.NotEmpty(result.Hash);
        Assert.True((DateTime.Now - result.CreatedAt).TotalSeconds < 2); // Created recently
    }

    [Fact]
    public void AddTask_WithEmptyDescription_ShouldThrowArgumentException()
    {
        // Arrange
        var hashGenerator = new HashGenerator();
        var taskManager = new TaskManager(hashGenerator);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => taskManager.AddTask(""));
        Assert.Throws<ArgumentException>(() => taskManager.AddTask("   ")); // Whitespace only
        Assert.Throws<ArgumentException>(() => taskManager.AddTask(null!));
    }

    [Fact]
    public void ListTasks_WhenEmpty_ShouldReturnEmptyCollection()
    {
        // Arrange
        var hashGenerator = new HashGenerator();
        var taskManager = new TaskManager(hashGenerator);

        // Act
        var tasks = taskManager.ListTasks();

        // Assert
        Assert.NotNull(tasks);
        Assert.Empty(tasks);
    }

    [Fact]
    public void ListTasks_WithMultipleTasks_ShouldReturnAllTasks()
    {
        // Arrange
        var hashGenerator = new HashGenerator();
        var taskManager = new TaskManager(hashGenerator);
        
        var task1 = taskManager.AddTask("Task 1");
        var task2 = taskManager.AddTask("Task 2");
        var task3 = taskManager.AddTask("Task 3");

        // Act
        var tasks = taskManager.ListTasks().ToList();

        // Assert
        Assert.NotNull(tasks);
        Assert.Equal(3, tasks.Count);
        Assert.Contains(task1, tasks);
        Assert.Contains(task2, tasks);
        Assert.Contains(task3, tasks);
    }

    [Fact]
    public void CompleteTask_WithValidHashPrefix_ShouldMarkTaskAsCompleted()
    {
        // Arrange
        var hashGenerator = new HashGenerator();
        var taskManager = new TaskManager(hashGenerator);
        var task = taskManager.AddTask("Complete this task");
        var hashPrefix = task.Hash[..3]; // First 3 chars

        // Act
        var result = taskManager.CompleteTask(hashPrefix);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(task.Hash, result.FullHash);
        Assert.Equal(TodoStatus.Completed, task.Status);
    }

    [Fact]
    public void CompleteTask_WithNonExistentHash_ShouldReturnFailure()
    {
        // Arrange
        var hashGenerator = new HashGenerator();
        var taskManager = new TaskManager(hashGenerator);
        taskManager.AddTask("Some task");

        // Act
        var result = taskManager.CompleteTask("zzz");

        // Assert
        Assert.False(result.Success);
        Assert.Contains("No task found", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CompleteTask_WithAmbiguousHashPrefix_ShouldReturnFailure()
    {
        // Arrange
        var hashGenerator = new HashGenerator();
        var taskManager = new TaskManager(hashGenerator);
        
        // Add tasks until we get two with same prefix (or mock this scenario)
        var tasks = new List<TodoTask>();
        for (int i = 0; i < 100; i++) // Try to get collision
        {
            tasks.Add(taskManager.AddTask($"Task {i}"));
        }
        
        // Find two tasks with same 3-char prefix
        var groupedByPrefix = tasks.GroupBy(t => t.Hash[..3]).FirstOrDefault(g => g.Count() > 1);
        
        if (groupedByPrefix != null)
        {
            var prefix = groupedByPrefix.Key;
            
            // Act
            var result = taskManager.CompleteTask(prefix);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("multiple", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
        }
    }

    [Fact]
    public void RemoveTask_WithValidHashPrefix_ShouldRemoveTask()
    {
        // Arrange
        var hashGenerator = new HashGenerator();
        var taskManager = new TaskManager(hashGenerator);
        var task = taskManager.AddTask("Remove this task");
        var hashPrefix = task.Hash[..3];
        var initialCount = taskManager.ListTasks().Count();

        // Act
        var result = taskManager.RemoveTask(hashPrefix);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(task.Hash, result.FullHash);
        Assert.Equal(initialCount - 1, taskManager.ListTasks().Count());
        Assert.DoesNotContain(task, taskManager.ListTasks());
    }

    [Fact]
    public void RemoveTask_WithNonExistentHash_ShouldReturnFailure()
    {
        // Arrange
        var hashGenerator = new HashGenerator();
        var taskManager = new TaskManager(hashGenerator);
        taskManager.AddTask("Some task");

        // Act
        var result = taskManager.RemoveTask("zzz");

        // Assert
        Assert.False(result.Success);
        Assert.Contains("No task found", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CompleteAllTasks_WithMultipleTasks_ShouldCompleteAll()
    {
        // Arrange
        var hashGenerator = new HashGenerator();
        var taskManager = new TaskManager(hashGenerator);
        
        taskManager.AddTask("Task 1");
        taskManager.AddTask("Task 2");
        taskManager.AddTask("Task 3");

        // Act
        taskManager.CompleteAllTasks();

        // Assert
        var tasks = taskManager.ListTasks();
        Assert.All(tasks, task => Assert.Equal(TodoStatus.Completed, task.Status));
    }

    [Fact]
    public void CompleteAllTasks_WithEmptyList_ShouldNotThrow()
    {
        // Arrange
        var hashGenerator = new HashGenerator();
        var taskManager = new TaskManager(hashGenerator);

        // Act & Assert
        var exception = Record.Exception(() => taskManager.CompleteAllTasks());
        Assert.Null(exception);
    }

    [Fact]
    public void RemoveAllTasks_WithMultipleTasks_ShouldRemoveAll()
    {
        // Arrange
        var hashGenerator = new HashGenerator();
        var taskManager = new TaskManager(hashGenerator);
        
        taskManager.AddTask("Task 1");
        taskManager.AddTask("Task 2");
        taskManager.AddTask("Task 3");

        // Act
        taskManager.RemoveAllTasks();

        // Assert
        var tasks = taskManager.ListTasks();
        Assert.Empty(tasks);
    }

    [Fact]
    public void RemoveAllTasks_WithEmptyList_ShouldNotThrow()
    {
        // Arrange
        var hashGenerator = new HashGenerator();
        var taskManager = new TaskManager(hashGenerator);

        // Act & Assert
        var exception = Record.Exception(() => taskManager.RemoveAllTasks());
        Assert.Null(exception);
    }
}