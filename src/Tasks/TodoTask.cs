namespace TodoCLI.Tasks;

public enum TodoStatus
{
    Pending,
    Completed
}

public class TodoTask
{
    public string Hash { get; }
    public string Description { get; }
    public TodoStatus Status { get; set; }
    public DateTime CreatedAt { get; }

    public TodoTask(string hash, string description, TodoStatus status, DateTime createdAt)
    {
        if (string.IsNullOrEmpty(hash))
            throw new ArgumentException("Hash cannot be null or empty.", nameof(hash));
        
        if (description is null)
            throw new ArgumentNullException(nameof(description));
        
        if (string.IsNullOrEmpty(description))
            throw new ArgumentException("Description cannot be empty.", nameof(description));

        Hash = hash;
        Description = description;
        Status = status;
        CreatedAt = createdAt;
    }
}