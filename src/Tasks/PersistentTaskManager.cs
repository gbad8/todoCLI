using System.Text.Json;
using TodoCLI.Tasks;

namespace TodoCLI.Tasks;

/// <summary>
/// Task manager with local file persistence
/// </summary>
public class PersistentTaskManager : ITaskService
{
    private readonly IHashGenerator _hashGenerator;
    private readonly string _tasksFilePath;
    private List<TodoTask> _tasks = new();

    public PersistentTaskManager(IHashGenerator hashGenerator)
    {
        _hashGenerator = hashGenerator ?? throw new ArgumentNullException(nameof(hashGenerator));
        
        // Create local data directory
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var appDir = Path.Combine(appDataPath, "TodoCLI");
        Directory.CreateDirectory(appDir);
        _tasksFilePath = Path.Combine(appDir, "tasks.json");
        
        // Load existing tasks
        LoadTasks();
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

        // Save to file
        SaveTasks();

        return task;
    }

    public IEnumerable<TodoTask> ListTasks()
    {
        // Always reload from file to get latest data
        LoadTasks();
        return _tasks.AsEnumerable();
    }

    public HashResult CompleteTask(string hashPrefix)
    {
        // Validate hash prefix
        if (string.IsNullOrWhiteSpace(hashPrefix) || hashPrefix.Length < 3)
        {
            return new HashResult
            {
                Success = false,
                ErrorMessage = "Hash prefix must be at least 3 characters long."
            };
        }

        // Load latest data
        LoadTasks();

        // Try to resolve hash
        var hashResult = _hashGenerator.ResolveHashPrefix(hashPrefix, _tasks.Select(t => t.Hash));
        
        if (!hashResult.Success)
        {
            return new HashResult
            {
                Success = false,
                ErrorMessage = hashResult.ErrorMessage
            };
        }

        // Find and complete the task
        var task = _tasks.FirstOrDefault(t => t.Hash == hashResult.FullHash);
        if (task != null)
        {
            var index = _tasks.IndexOf(task);
            _tasks[index] = new TodoTask(task.Hash, task.Description, TodoStatus.Completed, task.CreatedAt);
            SaveTasks();
        }

        return hashResult;
    }

    public HashResult RemoveTask(string hashPrefix)
    {
        // Validate hash prefix
        if (string.IsNullOrWhiteSpace(hashPrefix) || hashPrefix.Length < 3)
        {
            return new HashResult
            {
                Success = false,
                ErrorMessage = "Hash prefix must be at least 3 characters long."
            };
        }

        // Load latest data
        LoadTasks();

        // Try to resolve hash
        var hashResult = _hashGenerator.ResolveHashPrefix(hashPrefix, _tasks.Select(t => t.Hash));
        
        if (!hashResult.Success)
        {
            return new HashResult
            {
                Success = false,
                ErrorMessage = hashResult.ErrorMessage
            };
        }

        // Find and remove the task
        var task = _tasks.FirstOrDefault(t => t.Hash == hashResult.FullHash);
        if (task != null)
        {
            _tasks.Remove(task);
            SaveTasks();
        }

        return hashResult;
    }

    public void CompleteAllTasks()
    {
        LoadTasks();
        
        for (int i = 0; i < _tasks.Count; i++)
        {
            var task = _tasks[i];
            if (task.Status == TodoStatus.Pending)
            {
                _tasks[i] = new TodoTask(task.Hash, task.Description, TodoStatus.Completed, task.CreatedAt);
            }
        }
        
        SaveTasks();
    }

    public void RemoveAllTasks()
    {
        _tasks.Clear();
        SaveTasks();
    }

    public TodoTask AddExistingTask(TodoTask task)
    {
        if (task == null)
            throw new ArgumentNullException(nameof(task));

        LoadTasks();
        _tasks.Add(task);
        SaveTasks();
        return task;
    }

    public void SetTasks(IEnumerable<TodoTask> tasks)
    {
        _tasks.Clear();
        if (tasks != null)
        {
            _tasks.AddRange(tasks);
        }
        SaveTasks();
    }

    private void LoadTasks()
    {
        try
        {
            if (File.Exists(_tasksFilePath))
            {
                var json = File.ReadAllText(_tasksFilePath);
                var taskData = JsonSerializer.Deserialize<TaskData[]>(json);
                
                _tasks.Clear();
                if (taskData != null)
                {
                    foreach (var data in taskData)
                    {
                        if (Enum.TryParse<TodoStatus>(data.Status, out var status))
                        {
                            var task = new TodoTask(data.Hash, data.Description, status, data.CreatedAt);
                            _tasks.Add(task);
                        }
                    }
                }
            }
        }
        catch (Exception)
        {
            // If file is corrupted, start with empty list
            _tasks.Clear();
        }
    }

    private void SaveTasks()
    {
        try
        {
            var taskData = _tasks.Select(t => new TaskData
            {
                Hash = t.Hash,
                Description = t.Description,
                Status = t.Status.ToString(),
                CreatedAt = t.CreatedAt
            }).ToArray();

            var json = JsonSerializer.Serialize(taskData, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_tasksFilePath, json);
        }
        catch (Exception)
        {
            // Silently fail to avoid breaking the application
            // In production, would log this error
        }
    }

    private class TaskData
    {
        public string Hash { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}