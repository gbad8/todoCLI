using TodoCLI.CLI;
using Xunit;

namespace TodoCLI.Tests.CLI;

public class CommandParserTests
{
    [Fact]
    public void ParseCommand_WithAuthSetup_ShouldReturnAuthCommand()
    {
        // Arrange
        var parser = new CommandParser();
        var args = new[] { "auth", "setup" };

        // Act
        var result = parser.ParseCommand(args);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(CommandType.Auth, result.Type);
        Assert.Equal("setup", result.SubCommand);
        Assert.Empty(result.Arguments);
    }

    [Fact]
    public void ParseCommand_WithAddTask_ShouldReturnAddCommand()
    {
        // Arrange
        var parser = new CommandParser();
        var args = new[] { "add", "Buy groceries" };

        // Act
        var result = parser.ParseCommand(args);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(CommandType.Add, result.Type);
        Assert.Empty(result.SubCommand);
        Assert.Equal("Buy groceries", result.Arguments);
    }

    [Fact]
    public void ParseCommand_WithList_ShouldReturnListCommand()
    {
        // Arrange
        var parser = new CommandParser();
        var args = new[] { "list" };

        // Act
        var result = parser.ParseCommand(args);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(CommandType.List, result.Type);
        Assert.Empty(result.SubCommand);
        Assert.Empty(result.Arguments);
    }

    [Fact]
    public void ParseCommand_WithDone_ShouldReturnDoneCommand()
    {
        // Arrange
        var parser = new CommandParser();
        var args = new[] { "done", "abc123" };

        // Act
        var result = parser.ParseCommand(args);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(CommandType.Done, result.Type);
        Assert.Empty(result.SubCommand);
        Assert.Equal("abc123", result.Arguments);
    }

    [Fact]
    public void ParseCommand_WithRemove_ShouldReturnRemoveCommand()
    {
        // Arrange
        var parser = new CommandParser();
        var args = new[] { "rm", "def456" };

        // Act
        var result = parser.ParseCommand(args);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(CommandType.Remove, result.Type);
        Assert.Empty(result.SubCommand);
        Assert.Equal("def456", result.Arguments);
    }

    [Fact]
    public void ParseCommand_WithDoneAll_ShouldReturnDoneAllCommand()
    {
        // Arrange
        var parser = new CommandParser();
        var args = new[] { "done-all" };

        // Act
        var result = parser.ParseCommand(args);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(CommandType.DoneAll, result.Type);
        Assert.Empty(result.SubCommand);
        Assert.Empty(result.Arguments);
    }

    [Fact]
    public void ParseCommand_WithRemoveAll_ShouldReturnRemoveAllCommand()
    {
        // Arrange
        var parser = new CommandParser();
        var args = new[] { "rm", "all" };

        // Act
        var result = parser.ParseCommand(args);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(CommandType.RemoveAll, result.Type);
        Assert.Empty(result.SubCommand);
        Assert.Empty(result.Arguments);
    }

    [Fact]
    public void ParseCommand_WithSync_ShouldReturnSyncCommand()
    {
        // Arrange
        var parser = new CommandParser();
        var args = new[] { "sync" };

        // Act
        var result = parser.ParseCommand(args);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(CommandType.Sync, result.Type);
        Assert.Empty(result.SubCommand);
        Assert.Empty(result.Arguments);
    }

    [Fact]
    public void ParseCommand_WithInvalidCommand_ShouldReturnUnknownCommand()
    {
        // Arrange
        var parser = new CommandParser();
        var args = new[] { "invalid" };

        // Act
        var result = parser.ParseCommand(args);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(CommandType.Unknown, result.Type);
        Assert.Empty(result.SubCommand);
        Assert.Equal("invalid", result.Arguments);
    }

    [Fact]
    public void ParseCommand_WithEmptyArgs_ShouldReturnHelpCommand()
    {
        // Arrange
        var parser = new CommandParser();
        var args = Array.Empty<string>();

        // Act
        var result = parser.ParseCommand(args);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(CommandType.Help, result.Type);
        Assert.Empty(result.SubCommand);
        Assert.Empty(result.Arguments);
    }

    [Fact]
    public void ParseCommand_WithHelp_ShouldReturnHelpCommand()
    {
        // Arrange
        var parser = new CommandParser();
        var args = new[] { "help" };

        // Act
        var result = parser.ParseCommand(args);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(CommandType.Help, result.Type);
        Assert.Empty(result.SubCommand);
        Assert.Empty(result.Arguments);
    }
}