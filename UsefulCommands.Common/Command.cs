using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulCommands.Common
{
    public class Command
    {
        private readonly IConfiguration _configuration;

        public Command()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("logConfig.json", optional: false, reloadOnChange: true)
                .Build();
        }

        public void Run(Action<ILogger, CommandLineApplication> command, string[] args)
        {
            var loggerOptions = _configuration.Get<LoggerOptions>();
            var logger = new ConsoleLogger(loggerOptions);
            try
            {
                var app = new CommandLineApplication();
                app.HelpOption();

                command.Invoke(logger, app);

                app.Execute(args);
            }
            catch (UnrecognizedCommandParsingException ex)
            {
                logger.LogError(ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError("Unknown error." + ex);
            }
        }
    }
}
