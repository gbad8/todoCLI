namespace TodoCLI.Tasks;

public class TaskManager : ITaskService
{
  private readonly IHashGenerator _hashGenerator;
  private readonly List<TodoTask> _tasks = new();

  public TaskManager(IHashGenerator hashGenerator)
  {
    _hashGenerator = hashGenerator ?? throw new ArgumentNullException(nameof(hashGenerator));
  }

  public TodoTask AddTask(string description)
  {
    // Validate description first
    if (string.IsNullOrWhiteSpace(description))
      throw new ArgumentException("Description cannot be empty.", nameof(description));

    // Generate unique hash
    var hash = _hashGenerator.GenerateHash();

    // Create task
    var task = new TodoTask(hash, description, TodoStatus.Pending, DateTime.Now);

    // Add to collection
    _tasks.Add(task);

    return task;
  }

  public IEnumerable<TodoTask> ListTasks()
  {
    return _tasks.AsEnumerable();
  }

  public HashResult CompleteTask(string hashPrefix)
  {
    // Use the hash generator to resolve the prefix
    var existingHashes = _tasks.Select(t => t.Hash);
    var hashResult = _hashGenerator.ResolveHashPrefix(hashPrefix, existingHashes);

    if (!hashResult.Success)
      return hashResult;

    // Find the task by full hash and mark as completed
    var task = _tasks.FirstOrDefault(t => t.Hash == hashResult.FullHash);
    if (task != null)
    {
      task.Status = TodoStatus.Completed;
    }

    return hashResult;
  }

  public HashResult RemoveTask(string hashPrefix)
  {
    // Use the hash generator to resolve the prefix
    var existingHashes = _tasks.Select(t => t.Hash);
    var hashResult = _hashGenerator.ResolveHashPrefix(hashPrefix, existingHashes);

    if (!hashResult.Success)
      return hashResult;

    // Find and remove the task by full hash
    var task = _tasks.FirstOrDefault(t => t.Hash == hashResult.FullHash);
    if (task != null)
    {
      _tasks.Remove(task);
    }

    return hashResult;
  }

  public void CompleteAllTasks()
  {
    foreach (var task in _tasks)
    {
      task.Status = TodoStatus.Completed;
    }
  }

  public void RemoveAllTasks()
  {
    _tasks.Clear();
  }

  public TodoTask AddExistingTask(TodoTask task)
  {
    if (task == null)
      throw new ArgumentNullException(nameof(task));

    _tasks.Add(task);
    return task;
  }

  public void SetTasks(IEnumerable<TodoTask> tasks)
  {
    _tasks.Clear();
    if (tasks != null)
    {
      _tasks.AddRange(tasks);
    }
  }
}
