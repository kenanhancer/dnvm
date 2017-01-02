using System.Collections.Generic;
using System.Threading.Tasks;
using DotNet.Assets;

namespace DotNet.Commands
{
    class InstallFromFileCommand : ICommand
    {
        public async Task ExecuteAsync(CommandContext context)
        {
            if (context.ConfigFile == null)
            {
                context.Reporter.Error("No config file could be found.");
                context.Reporter.Output("Try executing `dnvm init` to create a config file, or use a subcommand of `dnvm install`.");
                context.Reporter.Output("See `dnvm install --help` for more info.");
                context.Result = Result.Error;
            }
            else
            {
                var commands = new List<ICommand>();
                if (!string.IsNullOrEmpty(context.ConfigFile.Sdk))
                {
                    commands.Add(new InstallCommand<SdkAsset>(context.ConfigFile.Sdk));
                }

                foreach (var fx in context.ConfigFile.SharedFx)
                {
                    commands.Add(new InstallCommand<SharedFxAsset>(fx));
                }

                var composite = new CompositeCommand(commands);

                await composite.ExecuteAsync(context);
            }
        }
    }
}