using DotNet.Assets;
using DotNet.Files;
using Microsoft.Extensions.CommandLineUtils;

namespace DotNet.Commands
{
    partial class CommandLine
    {
        private void InstallCommand(CommandLineApplication c)
        {
            c.Command("fx", "Install a .NET Core runtime framework", InstallFxCommand);

            c.Command("sdk", "Install a .NET Core SDK", InstallSdkCommand);

            c.OnExecute(() =>
            {
                this.Command = new InstallFromFileCommand();
            });

            var dnvm = FileConstants.Config;

            // TODO add dnvm-config with help about the file structure of the .dnvm file
            c.ExtendedHelpText = $@"
Additional Information:
  If executed without arguments, the 'install' command will search for the
  dnvm config file (named '{dnvm}') and attempt to install any assets
  specified in the file.
";
        }

        private void InstallFxCommand(CommandLineApplication fx)
        {
            var argVersion = fx.Argument("version",
                    $"Version of the shared framework to install. Defaults to '{SharedFxAsset.DefaultVersion}'");

            var optSave = fx.Option("-s|--save", $"Save to the 'fx' version of the '{FileConstants.Config}' config file", CommandOptionType.NoValue);

            fx.OnExecute(() =>
            {
                var version = argVersion.Value ?? SharedFxAsset.DefaultVersion;
                ICommand command = new InstallCommand<SharedFxAsset>(version);

                if (optSave.HasValue())
                {
                    command = new CompositeCommand(new[]
                    {
                        command,
                        new EditConfigCommand("fx", version, EditConfigCommand.EditAction.Append)
                    });
                }

                this.Command = command;
            });
        }

        private void InstallSdkCommand(CommandLineApplication sdk)
        {
            var argVersion = sdk.Argument("version", $"Version of the .NET Core SDK to install. Defaults to '{SdkAsset.DefaultVersion}'");
            var optSave = sdk.Option("-s|--save", $"Save as the value of 'sdk' in the '{FileConstants.Config}' config file", CommandOptionType.NoValue);

            sdk.OnExecute(() =>
            {
                var version = argVersion.Value ?? SdkAsset.DefaultVersion;
                ICommand command = new InstallCommand<SdkAsset>(version);
                if (optSave.HasValue())
                {
                    command = new CompositeCommand(new[]
                    {
                        command,
                        new EditConfigCommand("sdk", version, EditConfigCommand.EditAction.Set)
                    });
                }

                this.Command = command;
            });
        }
    }
}