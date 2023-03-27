using GitTree;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using UsefulCommands.Common;

new Command().Run((ILogger logger, CommandLineApplication app) =>
{
    var root = app.Option("-r|--root <STRING>", "Path of .git directory", CommandOptionType.SingleValue);
    root.DefaultValue = ".";

    var path = app.Option("-p|--path <STRING>", "Path tree to start from", CommandOptionType.SingleValue);
    path.DefaultValue = ".";

    var depth = app.Option<int>("-d|--depth <INT>", "Depth of directories", CommandOptionType.SingleValue);
    depth.DefaultValue = 0;

    var hideFiles = app.Option<bool>("-hf|--hideFiles <BOOL>", "Hides files", CommandOptionType.NoValue);

    var showFileCount = app.Option<bool>("-sfc|--showFileCount <BOOL>", "Shows file count", CommandOptionType.NoValue);

    app.OnExecute(() =>
    {
        var gitTreeCommand = new GitTreeCommand(logger,
            root.Value(),
            path.Value(),
            depth.ParsedValue,
            !hideFiles.ParsedValue,
            showFileCount.ParsedValue);

        gitTreeCommand.Run();
    });
}, args);