using Microsoft.Extensions.CommandLineUtils;

namespace DotNet.Commands
{
    partial class CommandLineOptions
    {
        private void InfoCommand(CommandLineApplication app)
        {
            app.Command("info", "Display information about the current dotnet environment", c =>
            {
                c.OnExecute(() =>
                {
                    this.Command = new InfoCommand();
                });
            });
        }
    }
}