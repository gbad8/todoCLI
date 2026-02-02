using TodoCLI.Tasks;

namespace TodoCLI.Tests.Tasks;

public class TaskTests
{
    [Fact]
    public void Task_Constructor_ShouldCreateTaskWithValidProperties()
    {
        // Arrange
        var hash = "abc123def456";
        var description = "Fix login validation bug";
        var status = TodoStatus.Pending;
        var createdAt = DateTime.Now;

        // Act
        var task = new TodoTask(hash, description, status, createdAt);

        // Assert
        Assert.Equal(hash, task.Hash);
        Assert.Equal(description, task.Description);
        Assert.Equal(status, task.Status);
        Assert.Equal(createdAt, task.CreatedAt);
    }

    [Fact]
    public void Task_Constructor_WithEmptyDescription_ShouldThrowArgumentException()
    {
        // Arrange
        var hash = "abc123def456";
        var description = "";
        var status = TodoStatus.Pending;
        var createdAt = DateTime.Now;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new TodoTask(hash, description, status, createdAt));
    }

    [Fact]
    public void Task_Constructor_WithNullDescription_ShouldThrowArgumentException()
    {
        // Arrange
        var hash = "abc123def456";
        string description = null!;
        var status = TodoStatus.Pending;
        var createdAt = DateTime.Now;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new TodoTask(hash, description, status, createdAt));
    }

    [Fact]
    public void Task_Constructor_WithEmptyHash_ShouldThrowArgumentException()
    {
        // Arrange
        var hash = "";
        var description = "Fix bug";
        var status = TodoStatus.Pending;
        var createdAt = DateTime.Now;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new TodoTask(hash, description, status, createdAt));
    }
}