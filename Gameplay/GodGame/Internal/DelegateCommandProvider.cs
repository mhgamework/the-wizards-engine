using System;
using System.Collections.Generic;
using System.Linq;

namespace MHGameWork.TheWizards.GodGame.Internal
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
            addCommand(name, 0, args => action());

        }
        public void addCommand(string name, Func<string, string> action)
        {
            addCommand(name, 1, args => action(args[0]));
        }
        public void addCommand(string name, Func<string, string, string> action)
        {
            addCommand(name, 2, args => action(args[0], args[1]));
        }

        public void addCommand(string name, int numArgs, Func<string[], string> action)
        {
            if (hasCommand(name)) throw new InvalidOperationException("Already a command with the name " + name);
            name = name.ToLower();

            commands[name] = args =>
            {
                if (args.Length != numArgs) return "Invalid number of arguments, expected " + numArgs;

                return action(args);
            };
        }

        public IEnumerable<string> CommandNames { get { return commands.Keys; } }

        public string AutoComplete(string partialCommand)
        {
            return partialCommand;
        }
    }
}