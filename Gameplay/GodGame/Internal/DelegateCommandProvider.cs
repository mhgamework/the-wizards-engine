using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.GodGame.Internal;
using System.Linq;

namespace MHGameWork.TheWizards.GodGame.DeveloperCommands
{
    public class DelegateCommandProvider : ICommandProvider
    {
        private Dictionary<string, Func<string[], string>> commands = new Dictionary<string, Func<string[], string>>();

        public DelegateCommandProvider()
        {
        }

        public string Execute(string command)
        {
            var parts = command.Split(' ');
            var cmd = parts[0].ToLower();
            var args = parts.Skip(1).ToArray();

            string result;
            if (!hasCommand(cmd))
                result = "Unknown command '" + cmd + "'";
            else
                result = runCommand(cmd, args);


            return ">" + command + "\n" + result;
        }

        private string runCommand(string cmd, string[] args)
        {
            return commands[cmd](args);
        }

        private bool hasCommand(string cmd)
        {
            return commands.ContainsKey(cmd);
        }

        public void addCommand(string name, Func<string> action)
        {
            name = name.ToLower();

            commands[name] = args =>
            {
                if (args.Length != 0) return "Invalid number of arguments, expected 0";

                return action();
            };
        }
        public void addCommand(string name, Func<string, string> action)
        {
            name = name.ToLower();

            commands[name] = args =>
                {
                    if (args.Length != 1) return "Invalid number of arguments, expected 1";

                    return action(args[0]);
                };
        }

        public IEnumerable<string> CommandNames { get { return commands.Keys; } }

        public string AutoComplete(string partialCommand)
        {
            return partialCommand;
        }
    }
}