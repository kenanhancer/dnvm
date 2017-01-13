using System;
using System.IO;
using System.Linq;
using DotNet.Files;

namespace DotNet.Commands
{
    public class EditConfigCommand : SyncCommand
    {
        public enum EditAction
        {
            Append,
            Set,
            Remove
        }

        private readonly string _section;
        private readonly string _value;
        private readonly EditAction _action;

        public EditConfigCommand(string section, string value, EditAction action)
        {
            _section = section;
            _value = value;
            _action = action;
        }

        protected override void Execute(CommandContext context)
        {
            switch (_section.ToLowerInvariant())
            {
                case "sdk":
                    if (_action == EditAction.Append)
                    {
                        throw new InvalidOperationException($"Section 'sdk' can only have one value");
                    }
                    context.ConfigFile.Sdk = _value;
                    break;

                case "fx":
                    if (_action == EditAction.Append)
                    {
                        if (!context.ConfigFile.SharedFx.Any(fx => fx == _value))
                        {
                            context.ConfigFile.SharedFx.Add(_value);
                        }
                    }
                    else if (_action == EditAction.Set)
                    {
                        context.ConfigFile.SharedFx.Clear();
                        context.ConfigFile.SharedFx.Add(_value);
                    }
                    else if (_action == EditAction.Remove)
                    {
                        context.ConfigFile.SharedFx.Clear();
                    }
                    break;

                default:
                    throw new InvalidOperationException($"Unrecognized section: {_section}");
            }

            using (var file = new FileStream(context.ConfigFile.FilePath, FileMode.OpenOrCreate))
            using (var writer = new StreamWriter(file))
            {
                new ConfigFileYamlWriter().Write(writer, context.ConfigFile);
            }

            context.Result = Result.Okay;
        }
    }
}