using System;
using System.Linq;

namespace TodoCLI.CLI;

public enum CommandType
{
    Help,
    Auth,
    Add,
    List,
    Done,
    Remove,
    DoneAll,
    RemoveAll,
    Sync,
    Unknown
}

public class Command
{
    public CommandType Type { get; }
    public string SubCommand { get; }
    public string Arguments { get; }

    public Command(CommandType type, string subCommand = "", string arguments = "")
    {
        Type = type;
        SubCommand = subCommand;
        Arguments = arguments;
    }
}

public class CommandParser
{
    public Command ParseCommand(string[] args)
    {
        if (args == null || args.Length == 0)
            return new Command(CommandType.Help);

        var command = args[0].ToLowerInvariant();

        return command switch
        {
            "auth" => ParseAuthCommand(args),
            "add" => ParseAddCommand(args),
            "list" => new Command(CommandType.List),
            "done" => ParseDoneCommand(args),
            "done-all" => new Command(CommandType.DoneAll),
            "rm" => ParseRemoveCommand(args),
            "sync" => new Command(CommandType.Sync),
            "help" => new Command(CommandType.Help),
            _ => new Command(CommandType.Unknown, "", command)
        };
    }

    private Command ParseAuthCommand(string[] args)
    {
        if (args.Length > 1)
            return new Command(CommandType.Auth, args[1]);
        
        return new Command(CommandType.Auth);
    }

    private Command ParseAddCommand(string[] args)
    {
        if (args.Length > 1)
        {
            var description = string.Join(" ", args.Skip(1));
            return new Command(CommandType.Add, "", description);
        }
        
        return new Command(CommandType.Add);
    }

    private Command ParseDoneCommand(string[] args)
    {
        if (args.Length > 1)
            return new Command(CommandType.Done, "", args[1]);
        
        return new Command(CommandType.Done);
    }

    private Command ParseRemoveCommand(string[] args)
    {
        if (args.Length > 1)
        {
            if (args[1].ToLowerInvariant() == "all")
                return new Command(CommandType.RemoveAll);
            
            return new Command(CommandType.Remove, "", args[1]);
        }
        
        return new Command(CommandType.Remove);
    }
}