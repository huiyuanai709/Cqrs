using System.CommandLine;
using Cqrs;

var rootCommand = new RootCommand
{
    new InitCommand(),
    new ProjectCommand(),
    new CommandCommand(),
    new QueryCommand()
};

return await rootCommand.InvokeAsync(args);
