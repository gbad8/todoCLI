namespace TodoCLI.Tasks;

public enum TodoStatus
{
  Pending,
  Completed
}

public class TodoTask(string hash, string description, TodoStatus status, DateTime createdAt)
{
  public string Hash { get; } = !string.IsNullOrEmpty(hash) ? hash : throw new ArgumentException("Hash cannot be null or empty.", nameof(hash));
  public string Description { get; } = !string.IsNullOrEmpty(description) ? description : throw new ArgumentException("Description cannot be empty.", nameof(description));
  public TodoStatus Status { get; set; } = status;
  public DateTime CreatedAt { get; } = createdAt;
}
