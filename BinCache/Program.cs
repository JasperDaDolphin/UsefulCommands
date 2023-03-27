using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using UsefulCommands.Common;
using BinCache;

new Command().Run((ILogger logger, CommandLineApplication app) =>
{
    var root = app.Option("-r|--root <STRING>", "Path of .git directory", CommandOptionType.SingleValue);
    root.DefaultValue = ".";
    root.IsRequired(true);

    var binPath = app.Option("-b|--bin <STRING>", "Path of Bin", CommandOptionType.SingleValue);
    binPath.DefaultValue = null;

    var target = app.Option("-t|--target <STRING>", "Target branch", CommandOptionType.SingleValue);
    target.DefaultValue = null;
    target.IsRequired(true);

    app.OnExecute(() =>
    {
        var binCacheCommand = new BinCacheCommand(
            logger, 
            root.Value(), 
            binPath.Value(), 
            target.Value());

        binCacheCommand.Run();
    });
}, args);