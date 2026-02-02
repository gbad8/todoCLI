namespace TodoCLI.Tasks;

/// <summary>
/// Interface for task management operations following Dependency Inversion Principle.
/// This abstraction allows other modules (like CLI) to depend on this interface
/// rather than the concrete TaskManager implementation.
/// </summary>
public interface ITaskService
{
    /// <summary>
    /// Adds a new task with the given description.
    /// </summary>
    /// <param name="description">The task description (cannot be empty)</param>
    /// <returns>The created task with unique hash</returns>
    /// <exception cref="ArgumentException">Thrown when description is null or empty</exception>
    TodoTask AddTask(string description);

    /// <summary>
    /// Retrieves all tasks in the system.
    /// </summary>
    /// <returns>Collection of all tasks (empty if no tasks exist)</returns>
    IEnumerable<TodoTask> ListTasks();

    /// <summary>
    /// Marks a task as completed using its hash prefix.
    /// </summary>
    /// <param name="hashPrefix">Hash prefix (minimum 3 characters)</param>
    /// <returns>Result indicating success/failure with full hash or error message</returns>
    HashResult CompleteTask(string hashPrefix);

    /// <summary>
    /// Removes a task using its hash prefix.
    /// </summary>
    /// <param name="hashPrefix">Hash prefix (minimum 3 characters)</param>
    /// <returns>Result indicating success/failure with full hash or error message</returns>
    HashResult RemoveTask(string hashPrefix);

    /// <summary>
    /// Marks all existing tasks as completed.
    /// </summary>
    void CompleteAllTasks();

    /// <summary>
    /// Removes all tasks from the system.
    /// </summary>
    void RemoveAllTasks();
}