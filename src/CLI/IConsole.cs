namespace TodoCLI.CLI;

/// <summary>
/// Result of a command execution
/// </summary>
public class CommandResult
{
    public bool Success { get; }
    public string Message { get; }

    public CommandResult(bool success, string message = "")
    {
        Success = success;
        Message = message;
    }

    public static CommandResult Ok(string message = "") => new(true, message);
    public static CommandResult Error(string message) => new(false, message);
}

/// <summary>
/// Interface for console interactions, enabling testing
/// </summary>
public interface IConsole
{
    void WriteLine(string message);
    void Write(string message);
    string ReadLine();
    ConsoleKeyInfo ReadKey();
}

/// <summary>
/// Production console implementation
/// </summary>
public class SystemConsole : IConsole
{
    public void WriteLine(string message) => Console.WriteLine(message);
    public void Write(string message) => Console.Write(message);
    public string ReadLine() => Console.ReadLine() ?? "";
    public ConsoleKeyInfo ReadKey() => Console.ReadKey();
}