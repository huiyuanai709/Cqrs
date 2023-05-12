using System.CommandLine;
using Cqrs;

var rootCommand = new RootCommand();
var initCommand = new InitCommand();
var projectCommand = new ProjectCommand();
var commandCommand = new CommandCommand();
var queryCommand = new QueryCommand();
rootCommand.Add(initCommand);
rootCommand.Add(projectCommand);
rootCommand.Add(commandCommand);
rootCommand.Add(queryCommand);

return await rootCommand.InvokeAsync(args);
