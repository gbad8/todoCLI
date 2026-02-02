using TodoCLI.Tasks;

namespace TodoCLI.Tests.Tasks;

public class TaskServiceIntegrationTests
{
    [Fact]
    public void TaskManager_ShouldImplementITaskService()
    {
        // Arrange
        var hashGenerator = new HashGenerator();

        // Act - Create via interface
        ITaskService taskService = new TaskManager(hashGenerator);

        // Assert
        Assert.NotNull(taskService);
        Assert.IsAssignableFrom<ITaskService>(taskService);
        Assert.IsType<TaskManager>(taskService);
    }

    [Fact]
    public void ITaskService_AddAndListTasks_ShouldWorkThroughInterface()
    {
        // Arrange
        var hashGenerator = new HashGenerator();
        ITaskService taskService = new TaskManager(hashGenerator);

        // Act
        var task1 = taskService.AddTask("Task via interface 1");
        var task2 = taskService.AddTask("Task via interface 2");
        var tasks = taskService.ListTasks().ToList();

        // Assert
        Assert.Equal(2, tasks.Count);
        Assert.Contains(task1, tasks);
        Assert.Contains(task2, tasks);
    }

    [Fact]
    public void ITaskService_CompleteTask_ShouldWorkThroughInterface()
    {
        // Arrange
        var hashGenerator = new HashGenerator();
        ITaskService taskService = new TaskManager(hashGenerator);
        var task = taskService.AddTask("Complete via interface");
        var hashPrefix = task.Hash[..3];

        // Act
        var result = taskService.CompleteTask(hashPrefix);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(task.Hash, result.FullHash);
        Assert.Equal(TodoStatus.Completed, task.Status);
    }

    [Fact]
    public void ITaskService_RemoveTask_ShouldWorkThroughInterface()
    {
        // Arrange
        var hashGenerator = new HashGenerator();
        ITaskService taskService = new TaskManager(hashGenerator);
        var task = taskService.AddTask("Remove via interface");
        var hashPrefix = task.Hash[..3];

        // Act
        var result = taskService.RemoveTask(hashPrefix);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(task.Hash, result.FullHash);
        Assert.DoesNotContain(task, taskService.ListTasks());
    }

    [Fact]
    public void ITaskService_BulkOperations_ShouldWorkThroughInterface()
    {
        // Arrange
        var hashGenerator = new HashGenerator();
        ITaskService taskService = new TaskManager(hashGenerator);
        
        taskService.AddTask("Task 1");
        taskService.AddTask("Task 2");
        taskService.AddTask("Task 3");

        // Act - Complete all via interface
        taskService.CompleteAllTasks();
        var tasksAfterComplete = taskService.ListTasks();

        // Assert - All completed
        Assert.All(tasksAfterComplete, task => Assert.Equal(TodoStatus.Completed, task.Status));

        // Act - Remove all via interface
        taskService.RemoveAllTasks();
        var tasksAfterRemove = taskService.ListTasks();

        // Assert - All removed
        Assert.Empty(tasksAfterRemove);
    }

    [Fact]
    public void ITaskService_DependencyInjection_ShouldAllowMocking()
    {
        // This test demonstrates that ITaskService can be used for dependency injection
        // and potentially mocked for unit testing other components

        // Arrange
        var hashGenerator = new HashGenerator();
        ITaskService taskService = new TaskManager(hashGenerator);

        // Act - Using interface reference
        var originalCount = taskService.ListTasks().Count();
        taskService.AddTask("DI Test Task");
        var newCount = taskService.ListTasks().Count();

        // Assert - Behavior is accessible through interface
        Assert.Equal(originalCount + 1, newCount);
    }
}