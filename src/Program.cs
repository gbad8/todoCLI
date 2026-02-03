using TodoCLI.Auth;
using TodoCLI.Tasks;
using TodoCLI.Sync;
using TodoCLI.CLI;

namespace TodoCLI;

class Program
{
    static async Task<int> Main(string[] args)
    {
        try
        {
            // Create service instances
            var tokenStorage = new FileTokenStorage();
            var gitHubClient = new HttpGitHubClient();
            var authService = new AuthManager(tokenStorage, gitHubClient);
            
            var hashGenerator = new HashGenerator();
            var taskService = new TaskManager(hashGenerator);
            
            var gistClient = new HttpGistClient();
            var syncService = new SyncManager(authService, gistClient, taskService);
            
            var console = new SystemConsole();
            var commandHandler = new CommandHandler(authService, taskService, syncService, console);

            // Parse command
            var commandParser = new CommandParser();
            var command = commandParser.ParseCommand(args);

            // Execute command
            var result = await commandHandler.HandleAsync(command);

            // Return appropriate exit code
            return result.Success ? 0 : 1;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fatal error: {ex.Message}");
            return 1;
        }
    }
}
