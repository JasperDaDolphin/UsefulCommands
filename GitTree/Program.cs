using GitTree;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using UsefulCommands.Common;

var logger = new ConsoleLogger();

try
{
    var app = new CommandLineApplication();
    app.HelpOption();

    var root = app.Option("-r|--root <PATH>", "Path of .git directory", CommandOptionType.SingleValue);
    root.DefaultValue = ".";

    var path = app.Option("-p|--path <NAME>", "Path tree to start from", CommandOptionType.SingleValue);
    path.DefaultValue = ".";

    app.OnExecute(() =>
    {
        var gitTreePrinter = new GitTreePrinter(logger);
        gitTreePrinter.Run(root.Value(), path.Value());
    });

    app.Execute(args);
}
catch (UnrecognizedCommandParsingException ex)
{
    logger.LogError(ex.Message);
}
catch (Exception)
{
    logger.LogError("Unknown error.");
}